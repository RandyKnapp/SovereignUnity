using System.Collections.Generic;

namespace Sovereign
{
	public class VillageManager : ICommandHandler, IGameFlowHandler
	{
		private readonly Dictionary<string, Village> villages = new Dictionary<string, Village>();
		 
		public bool CanHandleCommand(Player player, string command, List<string> args)
		{
			return false;
		}

		public void HandleCommand(Player player, string command, List<string> args)
		{
		}

		public void CreateNewVillage(Player player, string name)
		{
			if (GetVillageForPlayer(player) == null)
			{
				Village village = new Village(player, name);
				villages.Add(player.Id, village);
			}
		}

		private Village GetVillageForPlayer(Player player)
		{
			Village resultVillage;
			villages.TryGetValue(player.Id, out resultVillage);
			return resultVillage;
		}

		public void NewGame()
		{
			villages.Clear();
		}

		public void BeginTurn(int turnIndex)
		{
		}

		public void EndTurn(int turnIndex)
		{
		}

		public void EndGame()
		{
		}
	}
}

