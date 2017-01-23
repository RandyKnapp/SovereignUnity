using System;
using System.Linq;
using System.Collections.Generic;

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
		private readonly FamilyManager familyManager = new FamilyManager();

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }
		public List<Person> Population { get { return population; } }
		public List<Person> Graveyard { get { return graveyard; } }
		public List<Resource> Resources { get { return resources.Resources; } }
		public FamilyManager FamilyManager { get { return familyManager; } }
		public List<Family> Families { get { return familyManager.Families; } }

		public Village(Player player, string name)
		{
			OwnerPlayer = player;
			Name = name;
			resources.Add(StartingFood);
			population.Clear();

			AddPerson(Person.GenerateStartingChief(this));
			for (int i = 0; i < StartingPopulation; ++i)
			{
				Person person = Person.GenerateStartingPerson(this);
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
			List<Person> headsOfFamily = freeAdults.Where(p => p.Sex == "Male" || p.IsChief).ToList();

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

				Family family = new Family();
				family.AddHeadOfFamily(parentA);
				family.AddSpouse(parentA, parentB);

				familyManager.AddFamily(family);
			}

			for (int i = 0; i < headsOfFamily.Count; ++i)
			{
				Person person = freeAdults[i];

				Family family = new Family();
				family.AddHeadOfFamily(person);
				if ((i + 1) < freeAdults.Count && rand.NextDouble() < 0.3)
				{
					++i;
					Person sibling = freeAdults[i];
					family.AddSibling(person, sibling);
				}
				familyManager.AddFamily(family);
			}
		}

		private void AddChildToRandomFamilyWithTwoParents(Person child)
		{
			List<Family> marriedCouples = familyManager.Families.Where(f => f.HasSpouse(f.HeadOfFamily) && f.NumberOfChildren(f.HeadOfFamily) < 4).ToList();
			Family newFamily = marriedCouples[rand.Next(0, marriedCouples.Count)];

			Person head = newFamily.HeadOfFamily;
			Person spouse = newFamily.GetSpouse(head);
			newFamily.AddChild(head, spouse, child);
		}

		private void AddSlaveToRandomFamily(Person slave)
		{
			List<Family> familiesWithFewSlaves = familyManager.Families.Where(f => f.NumberOfSlaves(f.HeadOfFamily) < 2).ToList();
			Family newFamily = familiesWithFewSlaves[rand.Next(0, familiesWithFewSlaves.Count)];

			Person head = newFamily.HeadOfFamily;
			newFamily.AddSlave(head, slave);
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
			population.Add(person);
		}

		private void OnPersonDeath(Person person)
		{
		}

		public void NewGame() { }
		public void EndGame() { }

		public void BeginTurn(int turnIndex)
		{
		}

		public void EndTurn(int turnIndex)
		{
			GatherProduction();
			FeedPeople();
			BuryDead();
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
	}
}
