namespace BlazorApp.Client.Services
{
	public class MessageBusService
	{
		public event Action<char> OnKeyRefresh = default!;

		public event Action OnBoardGameRefresh = default!;

		public event Action OnStatsTrigger = default!;

		public event Action<int> OnCurrentLineCheckedSolution = default!;

		public event Action<int> OnBoardLineWrongSolution = default!;

		public void RefreshBoardGame() => OnBoardGameRefresh?.Invoke();

		public void RefreshKey(char key) => OnKeyRefresh?.Invoke(key);

		public void TriggerStats() => OnStatsTrigger?.Invoke();

		public void TriggerCurrentLineCheckedSolution(int currentRow) => OnCurrentLineCheckedSolution?.Invoke(currentRow);

		public void TriggerBoardLineWrongSolution(int currentRow) => OnBoardLineWrongSolution?.Invoke(currentRow);
	}
}