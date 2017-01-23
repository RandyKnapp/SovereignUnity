using System;

namespace Sovereign
{
	public class Person : GameObject, IProducer
	{
		private static readonly string[] ChiefTitles = {
			"Chief", "Jarl", "Arl", "Earl"
		};
		private static readonly string[] MaleNames = {
			"Ake", "Arvid", "Asger", "Asmund", "Audun",
			"Balder", "Bard", "Birger", "Bjarke", "Bjarte", "Bjorn", "Brandt", "Brynjar",
			"Calder", "Canute", "Carr", "Colborn", "Colden", "Cyler", "Dagfinn", "Destin",
			"Eerikki", "Egil", "Einar", "Eindride", "Eirik", "Elof", "Endre", "Erland",
			"Felman", "Fiske", "Folke", "Frey", "Frijof", "Frode", "Gandalf", "Geir", "Gosta", 
			"Gudbrand", "Gunnar", "Hagen", "Hakon", "Haldor", "Halvar", "Halvdan", "Havardr", 
			"Hjalmar", "Ingvar", "Inghard", "Ivar",
			"Jari", "Jerrik", "Kare", "Kelby", "Kensley", "Kirk", "Kjell", "Knud", "Kustaa",
			"Lamont", "Latham", "Leif", "Loki", "Manning", "Odin", "Olaf", "Olin", "Osmund",
			"Rolph", "Rangvald", "Ragnar", "Raul", "Sigurd", "Soini", "Sten", "Stian", "Stigr", "Sveinn",
			"Tait", "Tarben", "Thor", "Thorvaldr", "Torbjorn", "Torvald", "Troels", "Trygve", "Tyr",
			"Uffe", "Ulf", "Unn", "Vali", "Vern", "Vidar"
		};
		private static readonly string[] FemaleNames = {
			"Alfhild", "Alvilda", "Ase", "Aslaug", "Asta", "Astrid",
			"Bergljot", "Bodil", "Borghild", "Brenna",
			"Dagmar", "Dagny", "Eerika", "Eira", "Embla", "Eydis", "Freya",
			"Gerd", "Gul", "Gunbog", "Gunhild", "Gunvor", "Gyda",
			"Hege", "Helga", "Hertha", "Inger", "Ingrid", "Inkeri", "Iona", 
			"Jorunn", "Kari", "Lathertha", "Magnhild", "Nanna", "Oili", "Olga", "Oydis",
			"Ragna", "Ragnhild", "Runa",
			"Sassa", "Sigfrid", "Signy", "Sigrun", "Siri", "Siv", "Solveig", "Sylvi",
			"Thora", "Thyra", "Tone", "Tordis", "Torhild", "Tove", "Turid", "Tyra", "Ylva"
		};
		private const int StarvationThreshold = 3;
		private const int ComingOfAgeThreshold = 13;
		private static readonly Random rand = new Random();

		private int starvingCounter;

		public Village Village { get; private set; }
		public Family Family { get; set; }
		public bool IsChief { get; set; }
		public string Title { get; set; }
		public int Age { get; set; }
		public bool IsChild { get { return Age < ComingOfAgeThreshold; } }
		public bool IsSlave { get { return Class is Slave; } }
		public string Name { get; set; }
		public string Sex { get; set; }
		public PersonClass Class { get; set; }
		public bool Starving { get { return starvingCounter > 0; } }
		public bool Dead { get; private set; }

		public event Action<Person> OnDeath = delegate {};

		public Person()
		{
			Family = new Family(this);
		}

		public static Person GenerateStartingChief(Village village)
		{
			Person person = GenerateStartingPerson(village);
			person.Sex = rand.Next(0, 2) == 0 ? "Male" : "Female";
			person.Age = rand.Next(20, 40);
			person.Class = new Chief();
			person.IsChief = true;
			person.Title = ChiefTitles[rand.Next(0, ChiefTitles.Length)];

			return person;
		}

		public static Person GenerateStartingChild(Village village)
		{
			Person person = GenerateStartingPerson(village);
			person.Age = rand.Next(0, ComingOfAgeThreshold);
			person.Class = new Child();
			return person;
		}

		public static Person GenerateStartingPerson(Village village)
		{
			Person person = new Person();
			person.Village = village;
			person.Age = rand.Next(14, 31);
			person.Sex = rand.Next(0, 2) == 0 ? "Male" : "Female";
			person.Name = GetRandomName(person.Sex);
			person.Class = new Farmer();

			return person;
		}

		private static string GetRandomName(string sex)
		{
			return sex == "Male" ? MaleNames[rand.Next(MaleNames.Length)] : FemaleNames[rand.Next(FemaleNames.Length)];
		}

		public void Die()
		{
			Dead = true;
			OnDeath(this);
		}

		public ResourcePack Produce()
		{
			return Class.Produce();
		}

		public Food GetFoodRequired()
		{
			return new Food(1);
		}

		public void GoHungry()
		{
			starvingCounter++;
			if (starvingCounter >= StarvationThreshold)
			{
				Die();
			}
		}
	}
}
