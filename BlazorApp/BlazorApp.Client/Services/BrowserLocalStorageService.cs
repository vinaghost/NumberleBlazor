namespace BlazorApp.Client.Services
{
    public class BrowserLocalStorageService
    {
        private readonly ILocalStorageService _localStorage;

        public BrowserLocalStorageService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<(string?, List<string>?)> LoadGameStateFromLocalStorage()
        {
            var localStorageSolutionKey = await _localStorage.GetItemAsync<string>("SolutionKey");
            var board = await _localStorage.GetItemAsync<List<string>>("BoardGrid");

            return (localStorageSolutionKey, board);
        }

        public async Task SaveCurrentBoardToLocalStorage(List<string> boardGridWords)
        {
            await _localStorage.SetItemAsync("BoardGrid", boardGridWords);
        }

        public async Task SaveSolutionKeyToLocalStorage(string solutionKey)
        {
            await _localStorage.SetItemAsync("SolutionKey", solutionKey);
        }

        public async Task UpdateGameStats(GameState gameState, int currentRow)
        {
            var stats = await _localStorage.GetItemAsync<Stats>(nameof(Stats));
            stats ??= new Stats();

            stats.GamesPlayed++;

            if (gameState == GameState.Win)
            {
                stats.GamesWon++;
                stats.CurrentStreak++;

                if (stats.CurrentStreak > stats.BestStreak)
                    stats.BestStreak = stats.CurrentStreak;

                stats.GamesResultDistribution[currentRow + 1]++;
            }
            else
            {
                stats.CurrentStreak = 0;
                stats.GamesResultDistribution[-1]++;
            }

            await _localStorage.SetItemAsync(nameof(Stats), stats);

            await _localStorage.RemoveItemAsync("SolutionKey");
            await _localStorage.RemoveItemAsync("BoardGrid");
        }
    }
}