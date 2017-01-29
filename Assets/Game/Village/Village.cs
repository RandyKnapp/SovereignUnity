using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sovereign
{
	public class Village : GameObject, IGameFlowHandler
	{
		private const int StartingPopulation = 10;
		private const int StartingChildren = 10;
		private const int StartingSlaves = 4;
		private const int StartingFamilies = 3;
		private static readonly Food StartingFood = new Food(15);

		private string name;
		private Player player;
		private readonly Random rand = new Random();
		private readonly List<Person> population = new List<Person>();
		private readonly List<Person> graveyard = new List<Person>();
		private readonly ResourcePack resources = new ResourcePack();
		private readonly List<Person> births = new List<Person>();

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }
		public List<Person> Population { get { return population; } }
		public List<Person> Graveyard { get { return graveyard; } }
		public List<Resource> Resources { get { return resources.Resources; } }

		public Village(Player player, string name)
		{
			Commands.Add("-view-person", OnCommandViewPerson, 1, "-view-person <person-uid>", new[] { "-vp" });
			Commands.Add("-view-family", OnCommandViewFamily, 1, "-view-family <person-uid>", new[] { "-vf" });
			Commands.Add("-vkill-person", OnCommandKillPerson, 1, "-kill-person <person-uid>", new[] { "-kill" });

			OwnerPlayer = player;
			Name = name;
			resources.Add(StartingFood);
			population.Clear();

			AddPerson(Person.GenerateStartingChief(this));
			for (int i = 0; i < StartingPopulation; ++i)
			{
				Person person = Person.GenerateStartingPerson(this);
				person.Sex = i % 2 == 0 ? Sex.Male : Sex.Female;
				AddPerson(person);
			}

			GenerateStartingFamilies();

			for (int i = 0; i < StartingChildren; ++i)
			{
				Person child = Person.GenerateStartingChild(this);
				AddChildToRandomFamilyWithTwoParents(child);
				AddPerson(child);
			}
			for (int i = 0; i < StartingSlaves; ++i)
			{
				Person slave = Person.GenerateStartingPerson(this);
				slave.Class = new Slave();
				AddSlaveToRandomFamily(slave);
				AddPerson(slave);
			}
		}

		private void GenerateStartingFamilies()
		{
			List<Person> freeAdults = new List<Person>(population);
			List<Person> headsOfFamily = freeAdults.Where(p => p.Sex == Sex.Male || p.IsChief).ToList();

			for (int i = 0; i < StartingFamilies; ++i)
			{
				Person parentA = GetRandomPersonAndRemoveFromList(headsOfFamily);
				freeAdults.Remove(parentA);
				var oppositeSexFreeAdults = freeAdults.Where(p => p.Sex != parentA.Sex && !p.IsChief).ToList();
				if (oppositeSexFreeAdults.Count == 0)
				{
					break;
				}

				Person parentB = oppositeSexFreeAdults.ElementAt(rand.Next(0, oppositeSexFreeAdults.Count));
				freeAdults.Remove(parentB);
				headsOfFamily.Remove(parentB);

				parentA.Family.AddSpouse(parentB);
				parentB.Family.AddSpouse(parentA);
			}

			for (int i = 0; i < headsOfFamily.Count; ++i)
			{
				Person person = freeAdults[i];

				if ((i + 1) < freeAdults.Count && rand.NextDouble() < 0.3)
				{
					++i;
					Person sibling = freeAdults[i];
					person.Family.AddSibling(sibling);
					sibling.Family.AddSibling(person);
				}
			}
		}

		private void AddChildToRandomFamilyWithTwoParents(Person child)
		{
			List<Person> marriedAdults = population.Where(p => !p.IsChild && !p.IsSlave && p.Family.HasSpouse() && p.Family.NumberOfChildren() < 4).ToList();
			Person newParent = marriedAdults[rand.Next(0, marriedAdults.Count)];
			Family.AddChildToFamily(newParent, child);
		}

		private void AddSlaveToRandomFamily(Person slave)
		{
			List<Person> adultsWithFewSlaves = population.Where(p => !p.IsChild && !p.IsSlave && p.Family.NumberOfSlaves() < 2).ToList();
			Person newOwner = adultsWithFewSlaves[rand.Next(0, adultsWithFewSlaves.Count)];
			Family.AddSlaveToOwner(newOwner, slave);
		}

		private Person GetRandomPersonAndRemoveFromList(List<Person> list)
		{
			Person result = list.ElementAt(rand.Next(0, list.Count));
			list.Remove(result);
			return result;
		}

		private void AddPerson(Person person)
		{
			person.OnDeath += OnPersonDeath;
			person.OnComingOfAge += OnPersonComesOfAge;
			person.OnHaveBaby += OnPersonHasBaby;
			population.Add(person);
		}

		private void OnPersonDeath(Person person)
		{
			if (person.IsChief)
			{
				SelectNewChief(person);
			}
		}

		private void OnPersonComesOfAge(Person person)
		{
		}

		private void OnPersonHasBaby(Person parent, Person baby)
		{
			births.Add(baby);
		}

		public void NewGame() { }
		public void EndGame() { }

		public void BeginTurn(int turnIndex)
		{
			foreach (Person person in population)
			{
				person.AgeOneSeason();
			}
			AddBabiesToPopulation();
		}

		public void EndTurn(int turnIndex)
		{
			GatherProduction();
			FeedPeople();
			BuryDead();
		}

		private void AddBabiesToPopulation()
		{
			foreach (Person p in births)
			{
				AddPerson(p);
			}
			births.Clear();
		}

		private void GatherProduction()
		{
			foreach (Person person in population)
			{
				resources.Add(person.Produce());
			}
		}

		private void FeedPeople()
		{
			foreach (Person person in population)
			{
				FeedPerson(person);
			}
		}

		private void FeedPerson(Person person)
		{
			Food requiredFood = person.GetFoodRequired();
			if (resources.CanRemove(requiredFood))
			{
				resources.Remove(requiredFood);
			}
			else
			{
				person.GoHungry();
			}
		}

		private void BuryDead()
		{
			var theDead = population.Where(person => person.Dead).ToList();
			population.RemoveAll(person => person.Dead);
			graveyard.AddRange(theDead);
		}

		private void SelectNewChief(Person oldChief)
		{
			Person newChief = oldChief.Family.GetHeir();
			if (newChief != null)
			{
				if (newChief.Family.IsChildOf(oldChief))
				{
					Messenger.PostMessageToPlayer(OwnerPlayer, oldChief.DisplayName + "'s oldest " + (newChief.Sex == Sex.Male ? "son" : "daughter") + " " + newChief.DisplayName + " has become chief!");
				}
				else if (newChief.Family.IsSpouse(oldChief))
				{
					Messenger.PostMessageToPlayer(OwnerPlayer, oldChief.DisplayName + "'s " + Person.GetSpouseTypeName(newChief) + " " + newChief.DisplayName + " has become chief!");
				}
				else if (newChief.Family.IsSibling(oldChief))
				{
					Messenger.PostMessageToPlayer(OwnerPlayer, oldChief.DisplayName + "'s brother " + newChief.DisplayName + " has become chief!");
				}
			}

			if (newChief == null)
			{
				List<Person> eligiblePeople = population.Where(p => !p.IsChild && !p.IsSlave && !p.IsChief).ToList();
				if (eligiblePeople.Count > 0)
				{
					// TODO: Fight for chief
					newChief = eligiblePeople[rand.Next(0, eligiblePeople.Count)];
					Messenger.PostMessageToPlayer(OwnerPlayer, "After the old chief died without heirs, those who deemed themselves worthy fought for the title. "
						+ newChief.DisplayName + " emerged victorious and was named the new chief!");
				}
			}

			if (newChief == null)
			{
				newChief = Person.GenerateStartingChief(this);
				// TODO: Generate new chief's entourage
				Messenger.PostMessageToPlayer(OwnerPlayer, "The old chief died without heirs and the village has become too small to support itself. A foreign chief claims it for themselves. "
					+ newChief.DisplayName + " is now the chief of the village.");
			}
			else
			{
				newChief.IsChief = true;
				newChief.Title = oldChief.Title;
			}
		}

		private void OnCommandViewPerson(Player player, string command, List<string> args)
		{
			if (player != OwnerPlayer)
			{
				return;
			}

			Person person = PersonCommandHelper(args);
			if (person != null)
			{
				Messenger.PostMessageToPlayer(player, person.GetDebugString());
			}
		}

		private void OnCommandViewFamily(Player player, string command, List<string> args)
		{
			if (player != OwnerPlayer)
			{
				return;
			}

			Person person = PersonCommandHelper(args);
			if (person != null)
			{
				Messenger.PostMessageToPlayer(player, person.Family.GetDebugString());
			}
		}

		private void OnCommandKillPerson(Player player, string command, List<string> args)
		{
			if (player != OwnerPlayer)
			{
				return;
			}

			Person person = PersonCommandHelper(args);
			if (person != null)
			{
				person.Die();
			}
		}

		private Person PersonCommandHelper(List<string> args)
		{
			uint uid = Convert.ToUInt32(args[0]);
			Person person = GameObject.GetGameObject<Person>(uid);
			if (person != null)
			{
				return person;
			}
			else
			{
				Messenger.PostMessageToPlayer(player, "Could not find player with uid: " + uid);
				return null;
			}
		}

		public Person GetChief()
		{
			return population.Find(p => p.IsChief && !p.Dead);
		}
	}
}
