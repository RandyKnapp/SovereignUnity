namespace Sovereign
{
	public interface IGameFlowHandler
	{
		void NewGame();
		void BeginTurn(int turnIndex);
		void EndTurn(int turnIndex);
		void EndGame();
	}
}
