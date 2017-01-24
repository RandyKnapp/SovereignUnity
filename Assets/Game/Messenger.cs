namespace Sovereign
{
	public sealed class Messenger
	{
		private static IPlayerMessenger internalMessenger;

		public static void Initialize(IPlayerMessenger messenger)
		{
			internalMessenger = messenger;
		}

		public static void PostMessageToPlayer(Player player, string message)
		{
			internalMessenger.PostMessageToPlayer(player, message);
		}

		public static void PostMessageToAllPlayers(string message)
		{
			internalMessenger.PostMessageToAllPlayers(message);
		}
	}
}