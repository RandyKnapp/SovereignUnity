using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	public class Village : IProducer, IGameFlowHandler
	{
		private const int StartingPopulation = 10;
		private const int StartingFood = 15;

		private string name;
		private Player player;
		private List<Person> population = new List<Person>();
		private List<Person> graveyard = new List<Person>();
		private uint personUniqueId = 0;
		private int food;

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }
		public List<Person> Population { get { return population; } }
		public List<Person> Graveyard { get { return graveyard; } }
		public int Food { get { return food; } }

		public Village(Player player)
		{
			OwnerPlayer = player;
			Name = name;
			food = StartingFood;
			population.Clear();

			AddPerson(Person.GenerateStartingChief(personUniqueId++));
			for (int i = 0; i < StartingPopulation - 1; ++i)
			{
				Person person = Person.GenerateStartingPerson(personUniqueId++);
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

		public void Produce()
		{

		}

		public void NewGame() { }
		public void EndGame() { }

		public void BeginTurn(int turnIndex)
		{
		}

		public void EndTurn(int turnIndex)
		{
			FeedPeople();
			BuryDead();
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
			if (food > 0)
			{
				food--;
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
