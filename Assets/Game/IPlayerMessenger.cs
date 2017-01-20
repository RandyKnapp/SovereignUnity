namespace Sovereign
{
	public interface IPlayerMessenger
	{
		void PostMessageToPlayer(Player player, string message);
		void PostMessageToAllPlayers(string message);
	}
}