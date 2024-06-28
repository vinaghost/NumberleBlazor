namespace BlazorApp.Client.Services
{
    public class BrowserLocalStorageService
    {
        public DateTime GameStarted { get; set; }

        public DateTime LastGamePlayedDate { get; set; }

        private readonly ILocalStorageService _localStorage;

        public BrowserLocalStorageService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<string>?> LoadGameStateFromLocalStorage()
        {
            DateTime localStorageLastDayPlayed = await _localStorage.GetItemAsync<DateTime>(nameof(LastGamePlayedDate));
            var today = DateTime.Now.Date;

            if (localStorageLastDayPlayed == today)
            {
                LastGamePlayedDate = localStorageLastDayPlayed;

                var board = await _localStorage.GetItemAsync<List<string>>("BoardGrid");

                if (board != null)
                {
                    return board;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                LastGamePlayedDate = GameStarted.Date;
                await _localStorage.SetItemAsync(nameof(LastGamePlayedDate), GameStarted.Date);
                await _localStorage.RemoveItemAsync("BoardGrid");

                return null;
            }
        }

        public async Task SaveCurrentBoardToLocalStorage(List<string> boardGridWords)
        {
            await _localStorage.SetItemAsync("BoardGrid", boardGridWords);
        }

        public async Task UpdateGameStats(GameState gameState, int currentRow)
        {
            var lastGameFinishedDate = await _localStorage.GetItemAsync<DateTime>("lastGameFinishedDate");
            var today = DateTime.Now.Date;

            if (lastGameFinishedDate != today)
            {
                var stats = await _localStorage.GetItemAsync<Stats>(nameof(Stats));
                if (stats == null)
                {
                    stats = new Stats();
                }

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
            }
        }

        public async Task SaveLastGameFinishedDate()
        {
            await _localStorage.SetItemAsync("lastGameFinishedDate", LastGamePlayedDate);
        }
    }
}