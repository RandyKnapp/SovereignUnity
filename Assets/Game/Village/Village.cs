using System.Collections.Generic;

namespace Sovereign
{
	public class Village : IProducer
	{
		private string name;
		private Player player;
		private List<Person> population = new List<Person>();

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }
		public List<Person> Population { get { return population; } }

		private const int StartingPolulation = 10;

		public Village(Player player, string name)
		{
			OwnerPlayer = player;
			Name = name;
			population.Clear();

			for (int i = 0; i < StartingPolulation; ++i)
			{
				Person person = Person.GenerateStartingPerson();
				population.Add(person);
			}
		}

		public void Produce()
		{

		}
	}
}
