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

			Commands.Add("-d", OnCommandDebugStart, 0, "-d", new[] { "-debug-start" });
			Commands.Add("-new-game", OnCommandNewGame, 0, "-new-game", new[] { "-newgame", "-ng" });
			Commands.Add("-join-game", OnCommandJoinGame, 0, "-join-game", new[] { "-joingame", "-join", "-jg"});
			Commands.Add("-end-turn", OnCommandEndTurn, 0, "-end-turn", new[] { "-end", "-et" });

			gameFlowHandlers.Add(villageManager);
		}

		public void HandleCommand(Player player, string command, List<string> args)
		{
			Commands.Call(player, command, args);
		}

		public string GetDebugString()
		{
			string message = "Sovereign - ";
			message += inGame ? "Game Started" : "No game";
			message += " - Players: " + players.Count;
			message += "\nTurn: " + (turnCounter + 1);
			return message;
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

			BeginTurn(turnCounter);
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

		private void OnCommandDebugStart(Player player, string command, List<string> args)
		{
			OnCommandNewGame(player, "-ng", null);
			OnCommandJoinGame(player, "-jg", null);
		}

		private void OnCommandNewGame(Player player, string command, List<string> args)
		{
			NewGame();
		}

		private void OnCommandJoinGame(Player player, string command, List<string> args)
		{
			if (!players.Contains(player))
			{
				players.Add(player);

				villageManager.CreateNewVillage(player);
			}
		}

		private void OnCommandEndTurn(Player player, string command, List<string> args)
		{
			EndTurn(turnCounter);
			turnCounter++;
			BeginTurn(turnCounter);
		}

	}
}


