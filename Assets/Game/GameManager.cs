using System.Collections.Generic;

namespace Sovereign
{
	public class GameManager : ICommandHandler
	{
		private readonly VillageManager villageManager = new VillageManager();
		private readonly List<ICommandHandler> commandHandlers = new List<ICommandHandler>();

		public GameManager()
		{
			commandHandlers.Add(villageManager);
		}

		public bool CanHandleCommand(Player player, string command, List<string> args)
		{
			return true;
		}

		public void HandleCommand(Player player, string command, List<string> args)
		{
			foreach (ICommandHandler commandHandler in commandHandlers)
			{
				if (commandHandler.CanHandleCommand(player, command, args))
				{
					commandHandler.HandleCommand(player, command, args);
					break;
				}
			}
		}
	}
}


