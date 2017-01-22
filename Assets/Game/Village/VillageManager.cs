using System;
using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	public class VillageManager : IGameFlowHandler
	{
		private static readonly List<string> VillageNames = new List<string> {
			"Aebeltoft", "Kalbaek", "Breiðhatóftir", "Laugarbrekka", "Búðir",
			"Ravndal", "Djúpidalur", "Essetofte", "Aeblegården", "Hulgade", "Højtoft", 
			"Innrihólmur", "Holbaek", "Stenhus", "Kallekot", "Kirkeby", "Klibo", "Langatóftir", 
			"Lund", "Akranes", "Sandvik", "Haraldssun", "Thornby", "Torp", "Bregentved", 
			"Tóftir", "Ebeltoft", "Kvívik" 
		};

		private readonly Dictionary<string, Village> villages = new Dictionary<string, Village>();
		private static readonly Random rand = new Random();

		public VillageManager()
		{

		}

		public void CreateNewVillage(Player player)
		{
			if (GetVillage(player) == null)
			{
				Village village = new Village(player, GetRandomUniqueName());
				villages.Add(player.Id, village);
			}
		}

		private string GetRandomUniqueName()
		{
			var uniqueNames = VillageNames.Where(name => !VillageUsesName(name)).ToList();
			return uniqueNames[rand.Next(0, uniqueNames.Count)];
		}

		private bool VillageUsesName(string name)
		{
			return villages.Count(villageEntry => villageEntry.Value.Name == name) > 0;
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
			foreach (Village village in villages.Values)
			{
				village.BeginTurn(turnIndex);
			}
		}

		public void EndTurn(int turnIndex)
		{
			foreach (Village village in villages.Values)
			{
				village.EndTurn(turnIndex);
			}
		}

		public void EndGame()
		{
		}
	}
}

