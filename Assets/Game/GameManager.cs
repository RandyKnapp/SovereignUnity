using System;
using System.Collections.Generic;

namespace Sovereign
{
	public class GameManager : ICommandHandler, IGameFlowHandler
	{
		private readonly List<ICommandHandler> commandHandlers = new List<ICommandHandler>();
		private readonly List<IGameFlowHandler> gameFlowHandlers = new List<IGameFlowHandler>();

		private readonly IPlayerMessenger messenger;
		private readonly VillageManager villageManager = new VillageManager();
		private int turnCounter;

		public GameManager(IPlayerMessenger messenger)
		{
			this.messenger = messenger;
			commandHandlers.Add(villageManager);
			gameFlowHandlers.Add(villageManager);
		}

		public bool CanHandleCommand(Player player, string command, List<string> args)
		{
			return true;
		}

		public void HandleCommand(Player player, string command, List<string> args)
		{
			command = PreprocessCommand(command, args);
			foreach (ICommandHandler commandHandler in commandHandlers)
			{
				if (commandHandler.CanHandleCommand(player, command, args))
				{
					commandHandler.HandleCommand(player, command, args);
					break;
				}
			}
			messenger.PostMessageToPlayer(player, command + "(" + string.Join(", ", args.ToArray()) + ")");
		}

		private string PreprocessCommand(string command, List<string> args)
		{
			for (int i = 0; i < args.Count; i++)
			{
				args[i] = args[i].ToLowerInvariant();
			}
			return command.ToLowerInvariant();
		}

		public void NewGame()
		{
			turnCounter = 0;
			foreach (IGameFlowHandler gameFlowHandler in gameFlowHandlers)
			{
				gameFlowHandler.NewGame();
			}
		}

		public void BeginTurn(int turnIndex)
		{
			foreach (IGameFlowHandler gameFlowHandler in gameFlowHandlers)
			{
				gameFlowHandler.BeginTurn(turnIndex);
			}
		}

		public void EndTurn(int turnIndex)
		{
			foreach (IGameFlowHandler gameFlowHandler in gameFlowHandlers)
			{
				gameFlowHandler.EndTurn(turnIndex);
			}
		}

		public void EndGame()
		{
			foreach (IGameFlowHandler gameFlowHandler in gameFlowHandlers)
			{
				gameFlowHandler.EndGame();
			}
		}
	}
}


