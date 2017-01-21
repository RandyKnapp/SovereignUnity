using System.Collections.Generic;

namespace Sovereign
{
	public class VillageManager : IGameFlowHandler
	{
		private readonly Dictionary<string, Village> villages = new Dictionary<string, Village>();

		public VillageManager()
		{

		}

		public void CreateNewVillage(Player player, string name)
		{
			if (GetVillage(player) == null)
			{
				Village village = new Village(player, name);
				villages.Add(player.Id, village);
			}
		}

		public Village GetVillage(Player player)
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

