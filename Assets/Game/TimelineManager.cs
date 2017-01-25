using System.Collections.Generic;

namespace Sovereign
{
	public enum Season
	{
		Summer,
		Winter
	}

	public sealed class TimelineManager
	{
		private const int BaseYear = 700;

		private int turnCounter;

		public int Turn { get { return turnCounter; } }
		public int Year { get { return BaseYear + (turnCounter / 2); } }
		public Season Season { get { return (Season)(turnCounter % 2); } }
		public bool IsSummer { get { return Season == Season.Summer; } }
		public bool IsWinter { get { return Season == Season.Winter; } }

		public TimelineManager()
		{
			Commands.Add("-get-turn", OnCommandGetTurn, 0, "-get-turn", new[] { "-gt", "-turn-info", "-current-turn" });
		}

		public void NewGame()
		{
			turnCounter = 0;
		}

		public void EndTurn()
		{
			turnCounter++;
		}

		public string GetStatusText()
		{
			return "Turn: " + Turn + ", Year: " + Season + ", " + Year + " AD";
		}

		private void OnCommandGetTurn(Player player, string command, List<string> args)
		{
			Messenger.PostMessageToPlayer(player, GetStatusText());
		}
	}
}