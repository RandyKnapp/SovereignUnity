using System.Collections.Generic;

namespace Sovereign
{
	public class Village : IProducer
	{
		private const int StartingPolulation = 10;

		private string name;
		private Player player;
		private List<Person> population = new List<Person>();
		private uint personUniqueId = 0;

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }
		public List<Person> Population { get { return population; } }

		public Village(Player player)
		{
			OwnerPlayer = player;
			Name = name;
			population.Clear();

			population.Add(Person.GenerateStartingChief(personUniqueId++));
			for (int i = 0; i < StartingPolulation - 1; ++i)
			{
				Person person = Person.GenerateStartingPerson(personUniqueId++);
				population.Add(person);
			}
		}

		public void Produce()
		{

		}
	}
}
