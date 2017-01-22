using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	public class Village : IGameFlowHandler
	{
		private const int StartingPopulation = 6;
		private const int StartingChildren = 4;
		private static readonly Food StartingFood = new Food(15);

		private string name;
		private Player player;
		private List<Person> population = new List<Person>();
		private List<Person> graveyard = new List<Person>();
		private uint personUniqueId = 0;
		private ResourcePack resources = new ResourcePack();

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }
		public List<Person> Population { get { return population; } }
		public List<Person> Graveyard { get { return graveyard; } }
		public List<Resource> Resources { get { return resources.Resources; } }

		public Village(Player player, string name)
		{
			OwnerPlayer = player;
			Name = name;
			resources.Add(StartingFood);
			population.Clear();

			AddPerson(Person.GenerateStartingChief(personUniqueId++));
			for (int i = 0; i < StartingPopulation; ++i)
			{
				Person person = Person.GenerateStartingPerson(personUniqueId++);
				AddPerson(person);
			}
			for (int i = 0; i < StartingChildren; ++i)
			{
				Person person = Person.GenerateStartingChild(personUniqueId++);
				AddPerson(person);
			}
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
