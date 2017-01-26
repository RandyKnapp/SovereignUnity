using System;
using UnityEditor.iOS.Xcode;

namespace Sovereign
{
	public enum Sex
	{
		Female,
		Male
	}
	
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
		private const int ChildbearingThreshold = 50;
		private const int AverageLifespan = 70;
		private const int LifespanRange = 10;
		private static readonly Random rand = new Random();

		private int starvingCounter;
		private int lifeCounter;
		private Season birthSeason;
		private int deathCounter;

		public Village Village { get; private set; }
		public Family Family { get; set; }
		public bool IsChief { get; set; }
		public string Title { get; set; }
		public int Age { get { return lifeCounter / 2; } set { lifeCounter = value * 2; } }
		public Season BirthSeason { get { return birthSeason; } }
		public bool IsChild { get { return Age < ComingOfAgeThreshold; } }
		public bool IsSlave { get { return Class is Slave; } }
		public string Name { get; set; }
		public Sex Sex { get; set; }
		public PersonClass Class { get; set; }
		public bool Starving { get { return starvingCounter > 0; } }
		public bool Dead { get; private set; }

		public event Action<Person> OnDeath = delegate {};
		public event Action<Person> OnComingOfAge = delegate {};

		public Person()
		{
			Family = new Family(this);
		}

		public static Person GenerateStartingChief(Village village)
		{
			Person person = GenerateStartingPerson(village, 20, 40);
			person.Class = new Chief();
			person.IsChief = true;
			person.Title = ChiefTitles[rand.Next(0, ChiefTitles.Length)];

			return person;
		}

		public static Person GenerateStartingChild(Village village)
		{
			Person person = GenerateStartingPerson(village, 0, ComingOfAgeThreshold);
			person.Class = new Child();
			return person;
		}

		public static Person GenerateStartingPerson(Village village, int ageMin = 14, int ageMax = 31)
		{
			Person person = new Person();
			person.BeBorn();
			person.Village = village;
			person.SetStartingAge(rand.Next(ageMin, ageMax));
			person.Sex = rand.Next(0, 2) == 0 ? Sex.Male : Sex.Female;
			person.Name = GetRandomName(person.Sex);
			person.Class = new Farmer();

			return person;
		}

		private void SetStartingAge(int age)
		{
			lifeCounter = age * 2;
			birthSeason = rand.Next(0, 2) == 0 ? Season.Summer : Season.Winter;
			lifeCounter += birthSeason == Season.Winter ? 1 : 0;
		}

		private static string GetRandomName(Sex sex)
		{
			return sex == Sex.Male ? MaleNames[rand.Next(MaleNames.Length)] : FemaleNames[rand.Next(FemaleNames.Length)];
		}

		public void BeBorn()
		{
			lifeCounter = 0;
			birthSeason = GameManager.Instance.Timeline.Season;
			deathCounter = (AverageLifespan * 2) + rand.Next(-LifespanRange * 2, LifespanRange * 2);
		}

		public void AgeOneSeason()
		{
			bool wasChild = IsChild;
			bool wasAlive = !Dead;
			lifeCounter++;
			if (wasChild && !IsChild)
			{
				Class = new Farmer();
				OnComingOfAge(this);
			}

			if (wasAlive && lifeCounter > deathCounter)
			{
				Die();
			}
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

		public string GetDebugString()
		{
			string output = "[" + Uid + "] " + (Title != null ? Title + " " : "") + Name + " - " + Sex + ", " + Age + ", " + Class.Name;
			if (Starving)
			{
				output += " (Starving)";
			}
			else if (Dead)
			{
				output += " (Dead)";
			}

			return output;
		}

		private float GetBaseFertility()
		{
			// Men, girls before puberty and women after menopause do not have babies (in this game)
			if (Sex == Sex.Male || Age < ComingOfAgeThreshold || Age >= ChildbearingThreshold)
			{
				return 0;
			}

			// Only married women have babies (in this game)
			if (!Family.HasSpouse())
			{
				return 0;
			}

			// Fertility starts at 13 and peaks between 25 and 35 then tapers off until menopause
			if (Age < 25)
			{
				return MathUtil.Lerp(0.5f, 1.0f, MathUtil.Map(ComingOfAgeThreshold, 25, Age));
			}
			else if (Age > 35)
			{
				return MathUtil.Lerp(1.0f, 0.0f, MathUtil.Map(25, 35, Age));
			}
			else
			{
				return 1.0f;
			}
		}
	}
}
