using System;
using System.Collections.Generic;

namespace Sovereign
{
	public class GameManager : IGameFlowHandler
	{
		private readonly List<IGameFlowHandler> gameFlowHandlers = new List<IGameFlowHandler>();

		private readonly IPlayerMessenger messenger;
		private readonly VillageManager villageManager = new VillageManager();
		private readonly List<Player> players = new List<Player>();
		private bool inGame;
		private int turnCounter;

		public VillageManager Villages { get { return villageManager; } }

		public GameManager(IPlayerMessenger messenger)
		{
			this.messenger = messenger;

			Commands.Add("-new-game", OnCommandNewGame, 0, "-new-game", new[] { "-newgame", "-ng" });
			Commands.Add("-join-game", OnCommandJoinGame, 2, "-new-game <playerTitle> <villageName>", new[] { "-joingame", "-join", "-jg"});

			gameFlowHandlers.Add(villageManager);
		}

		public void HandleCommand(Player player, string command, List<string> args)
		{
			Commands.Call(player, command, args);
		}

		public void NewGame()
		{
			turnCounter = 0;
			inGame = true;
			players.Clear();
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

		private void OnCommandNewGame(Player player, string command, List<string> args)
		{
			NewGame();
		}

		private void OnCommandJoinGame(Player player, string command, List<string> args)
		{
			string playerTitle = args[0];
			string villageName = args[1];

			if (!players.Contains(player))
			{
				player.Title = playerTitle;
				players.Add(player);

				villageManager.CreateNewVillage(player, villageName);
			}
		}

		public string GetDebugString()
		{
			string message = "Sovereign - ";
			message += inGame ? "Game Started" : "No game";
			message += " - Players: " + players.Count;
			return message;
		}
	}
}


