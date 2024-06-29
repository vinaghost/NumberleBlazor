using System.Net.Http.Json;
using System.Text;
using BoardCell = BlazorApp.Client.Model.BoardCell;

namespace BlazorApp.Client.Services
{
    public class GameManagerService
    {
        private const string solutionPath = "api/solution";
        private const string checkPath = "api/check";

        public static readonly int RowSize = 6;
        public static readonly int ColumnSize = 5;
        public BoardCell[,] BoardGrid { get; private set; } = new BoardCell[RowSize, ColumnSize];

        public string SolutionKey { get; private set; } = "";
        public string Solution { get; private set; } = "";
        public Dictionary<char, KeyState> UsedKeys { get; private set; } = [];

        public GameState GameState { get; private set; }

        public event Action<int> OnBoardLineWrongSolution = default!;

        public event Action<int> OnCurrentLineCheckedSolution = default!;

        private readonly HttpClient _httpClient;
        private readonly ToastNotificationService _toastNotificationService;
        private readonly BrowserLocalStorageService _localStorageService;
        private readonly LocalizationService _localizationService;

        private int _currentRow;
        private int _currentColumn;

        public GameManagerService(HttpClient httpClient, ToastNotificationService toastNotificationService, BrowserLocalStorageService localStorage, LocalizationService loc)
        {
            _httpClient = httpClient;
            _toastNotificationService = toastNotificationService;
            _localStorageService = localStorage;
            _localizationService = loc;

            PopulateBoard();
        }

        public async Task StartGame()
        {
            GameState = GameState.Playing;

            var (storedSolutionKey, storedWords) = await _localStorageService.LoadGameStateFromLocalStorage();

            if (!string.IsNullOrEmpty(storedSolutionKey))
            {
                SolutionKey = storedSolutionKey;

                if (storedWords != null)
                    await SetBoardGridWords(storedWords);
            }
            else
            {
                SolutionKey = await GetNewKey();
                await _localStorageService.SaveSolutionKeyToLocalStorage(SolutionKey);
            }
        }

        public void EnterNextValue(char value)
        {
            if (GameState == GameState.Playing)
            {
                if (_currentColumn == ColumnSize - 1 && BoardGrid[_currentRow, _currentColumn].Value != null)
                    return;

                BoardGrid[_currentRow, _currentColumn].Value = value;
                BoardGrid[_currentRow, _currentColumn].State = BoardCellState.Typing;

                if (_currentColumn < ColumnSize - 1)
                    _currentColumn++;
            }
        }

        public void RemoveLastValue()
        {
            if (GameState == GameState.Playing)
            {
                if (_currentColumn == 0 && BoardGrid[_currentRow, _currentColumn].Value == null)
                    return;

                if (_currentColumn <= ColumnSize - 1 && BoardGrid[_currentRow, _currentColumn].Value == null)
                {
                    BoardGrid[_currentRow, _currentColumn - 1].Value = null;
                    BoardGrid[_currentRow, _currentColumn - 1].State = BoardCellState.Empty;
                    _currentColumn--;
                }
                else
                {
                    BoardGrid[_currentRow, _currentColumn].Value = null;
                    BoardGrid[_currentRow, _currentColumn].State = BoardCellState.Empty;
                }
            }
        }

        public async Task CheckCurrentLineSolution()
        {
            if (GameState == GameState.Playing)
            {
                StringBuilder currentLineBuilder = new();

                for (int i = 0; i < ColumnSize; i++)
                {
                    currentLineBuilder.Append(BoardGrid[_currentRow, i].Value);
                }

                string currentLine = currentLineBuilder.ToString();

                if (currentLine.Length != ColumnSize)
                {
                    _toastNotificationService.ShowToast(_localizationService["GameManagerNotEnoughLetters"]);
                    OnBoardLineWrongSolution.Invoke(_currentRow);
                    return;
                }

                var result = await _httpClient.GetFromJsonAsync<BoardCellState[]>($"{checkPath}?key={SolutionKey}&guess={currentLine}");
                if (result == null)
                {
                    _toastNotificationService.ShowToast(_localizationService["GameManagerNotRespond"]);
                    OnBoardLineWrongSolution.Invoke(_currentRow);
                    return;
                }

                for (int i = 0; i < currentLine.Length; i++)
                {
                    BoardGrid[_currentRow, i].State = result[i];
                    switch (result[i])
                    {
                        case BoardCellState.Correct:
                            if (!UsedKeys.TryAdd(currentLine[i], KeyState.Correct))
                                UsedKeys[currentLine[i]] = KeyState.Correct;
                            break;

                        case BoardCellState.IncorrectPosition:
                            UsedKeys.TryAdd(currentLine[i], KeyState.IncorrectPosition);
                            break;

                        case BoardCellState.Wrong:
                            UsedKeys.TryAdd(currentLine[i], KeyState.Wrong);
                            break;

                        default:
                            break;
                    }
                }

                OnCurrentLineCheckedSolution.Invoke(_currentRow);

                if (Array.TrueForAll(result, x => x == BoardCellState.Correct))
                {
                    GameState = GameState.Win;
                }
                else if (_currentRow < RowSize - 1)
                {
                    _currentRow++;
                    _currentColumn = 0;
                }
                else
                {
                    GameState = GameState.GameOver;
                }

                await _localStorageService.SaveCurrentBoardToLocalStorage(GetBoardGridWords());

                if (GameState == GameState.Win || GameState == GameState.GameOver)
                {
                    Solution = await GetSolution();
                    await _localStorageService.UpdateGameStats(GameState, _currentRow);
                }
            }
        }

        public int GetCurrentRow()
        {
            return _currentRow;
        }

        private async Task<string> GetSolution()
        {
            var solution = await _httpClient.GetFromJsonAsync<string>($"{solutionPath}/{SolutionKey}");
            return solution ?? "";
        }

        private async Task<string> GetNewKey()
        {
            var solutionKey = await _httpClient.GetFromJsonAsync<string>(solutionPath);
            return solutionKey ?? "";
        }

        private void PopulateBoard()
        {
            for (int i = 0; i < RowSize; i++)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    BoardGrid[i, j] = new BoardCell();
                }
            }
        }

        private List<string> GetBoardGridWords()
        {
            var wordList = new List<string>();
            var sb = new StringBuilder();
            for (int i = 0; i <= _currentRow; i++)
            {
                if (BoardGrid[i, 0].State != BoardCellState.Empty && BoardGrid[i, 0].State != BoardCellState.Typing)
                {
                    sb.Clear();

                    for (int j = 0; j < ColumnSize; j++)
                    {
                        sb.Append(BoardGrid[i, j].Value);
                    }

                    wordList.Add(sb.ToString());
                }
            }

            return wordList;
        }

        private async Task SetBoardGridWords(List<string> words)
        {
            foreach (var word in words)
            {
                int col = 0;

                foreach (var c in word)
                {
                    BoardGrid[_currentRow, col].State = BoardCellState.Typing;
                    BoardGrid[_currentRow, col].Value = c;

                    col++;
                }

                await CheckCurrentLineSolution();
            }
        }
    }
}