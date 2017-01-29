using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions.Must;

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
		private static readonly string[] SpouseNames = { "wife", "husband" };
		private const int StarvationThreshold = 3;
		private const int ComingOfAgeThreshold = 13;
		private const int AverageLifespan = 50;
		private const int LifespanRange = 10;
		private const int PregnancyDuration = 2;
		private const float BasePregnancyChance = 0.25f;
		private const float BaseMarriageChance = 0.25f;
		private const float LiveBirthChance = 0.6f;
		private const float SurviveChildbirthChance = 0.9f;
		private const float SurviveInfancyChance = 0.5f;
		private const float SurviveChildhoodChance = 0.5f;
		private static readonly Random rand = new Random();

		private int starvingCounter;
		private int lifeCounter;
		private Season birthSeason;
		private int deathCounter;
		private int pregnancyCounter;

		public Village Village { get; private set; }
		public Family Family { get; set; }
		public bool IsChief { get; set; }
		public string Title { get; set; }
		public int Age { get { return lifeCounter / 2; } set { lifeCounter = value * 2; } }
		public Season BirthSeason { get { return birthSeason; } }
		public bool IsChild { get { return Age < ComingOfAgeThreshold; } }
		public bool IsSlave { get { return Family.HasOwner(); } }
		public string Name { get; set; }
		public string DisplayName { get { return (Title != null ? Title + " " : "") + Name; } }
		public Sex Sex { get; set; }
		public PersonClass Class { get; set; }
		public bool Starving { get { return starvingCounter > 0; } }
		public bool Dead { get; private set; }
		public bool IsPregnant { get { return pregnancyCounter > 0; } }

		public event Action<Person> OnDeath = delegate {};
		public event Action<Person> OnComingOfAge = delegate {};
		public event Action<Person, Person> OnHaveBaby = delegate {};

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
			person.Class = new Farmer();

			return person;
		}

		public static string GetSpouseTypeName(Person person)
		{
			return SpouseNames[(int)person.Sex];
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
			Sex = rand.Next(0, 2) == 0 ? Sex.Male : Sex.Female;
			Name = GetRandomName(Sex);
			Class = new Child();
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
				if (Age < 2)
				{
					Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " has died in infancy!");
				}
				else if (Age < ComingOfAgeThreshold)
				{
					Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " has died from sickness as a child!");
				}
				else
				{
					Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " has died of old age!");
				}
			}

			if (Dead)
			{
				return;
			}

			CheckForMarriage();
			CheckForPregnancy();

			if (IsPregnant)
			{
				pregnancyCounter--;
				if (pregnancyCounter == 0)
				{
					HaveBaby();
				}
			}
		}

		public void Die()
		{
			Dead = true;
			OnDeath(this);
			Family.HandleDeath(this);
		}

		public ResourcePack Produce()
		{
			return Class.Produce();
		}

		public Food GetFoodRequired()
		{
			return new Food(0);
		}

		public void GoHungry()
		{
			starvingCounter++;
			if (starvingCounter >= StarvationThreshold)
			{
				Die();
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " has died of starvation!");
			}
		}

		public bool CanMarry(Person newSpouse)
		{
			return !Family.HasSpouse() && !newSpouse.Family.HasSpouse();
		}

		public void Marry(Person newSpouse)
		{
			if (CanMarry(newSpouse))
			{
				BecomeFreePerson();
				newSpouse.BecomeFreePerson();
				Family.AddSpouse(newSpouse);
				newSpouse.Family.AddSpouse(this);

				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " and " + newSpouse.DisplayName + " were married!");
			}
		}

		public void HaveBaby()
		{
			pregnancyCounter = 0;

			Person baby = new Person();
			baby.BeBorn();
			baby.Village = Village;
			Family.AddChildToFamily(this, baby);
			OnHaveBaby(this, baby);

			CheckForBirthComplications(baby);
			if (!baby.Dead)
			{
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " gave birth to a baby! (" + baby.DisplayName + ", " + baby.Sex + ")");
			}
		}

		public void BecomeFreePerson()
		{
			if (IsSlave)
			{
				Class = new Farmer();
				Family.FreeSlave(Family.Owner, this);
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " became a free person!");
			}
		}

		public void Enslave(Person owner)
		{
			if (!IsSlave)
			{
				Class = new Slave();
				Family.AddSlaveToOwner(owner, this);
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " was enslaved (Owner: " + owner.DisplayName + ")!");
			}
		}

		public void CheckForMarriage()
		{
			if (!Family.HasSpouse())
			{
				Person potentialSpouse = GetPotentialMate();
				if (potentialSpouse != null)
				{
					const float marriageChance = BaseMarriageChance;
					bool getMarried = rand.NextDouble() < marriageChance;
					if (getMarried)
					{
						Marry(potentialSpouse);
					}
				}
			}
		}

		public void CheckForPregnancy()
		{
			if (Sex == Sex.Male || IsPregnant)
			{
				return;
			}

			Person mate = GetPotentialMate();
			if (mate == null)
			{
				return;
			}

			float seasonalModifier = GameManager.Instance.Timeline.Season == Season.Winter ? 1.0f : 0.5f;
			float marriageModifier = Family.HasSpouse() ? 1.0f : 0.2f;
			float fertility = GetBaseFertility();
			float familySizeChance = MathUtil.Lerp(1.0f, 0.01f, Family.NumberOfChildren() / 5.0f);
			const float basePregnancyChance = BasePregnancyChance;
			float pregnancyChance = basePregnancyChance * fertility * seasonalModifier * marriageModifier * familySizeChance;

			bool pregnant = rand.NextDouble() < pregnancyChance;
			if (pregnant)
			{
				if (mate != Family.Spouse)
				{
					Marry(mate);
				}

				pregnancyCounter = PregnancyDuration;
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " is pregnant!");
			}
		}

		private Person GetPotentialMate()
		{
			if (Family.HasSpouse())
			{
				return Family.Spouse;
			}

			List<Person> potentialMates = Village.Population.Where(p => !p.Dead && !p.IsChild && !p.IsSlave && Math.Abs(p.Age - Age) < 5 && p.Sex != Sex && !p.Family.IsRelated(this) && !p.Family.HasSpouse()).ToList();
			if (potentialMates.Count == 0)
			{
				return null;
			}
			return potentialMates[rand.Next(0, potentialMates.Count)];
		}

		private float GetBaseFertility()
		{
			if (Age < ComingOfAgeThreshold)
			{
				return 0;
			}

			return Sex == Sex.Male ? GetMaleFertility() : GetFemaleFertility();
		}

		private float GetMaleFertility()
		{
			const int MalePeakFertilityBegin = 20;
			const int MalePeakFertilityEnd = 40;
			const int ChildbearingThreshold = 80;

			if (Age > ChildbearingThreshold)
			{
				return 0;
			}
			else if (Age > MalePeakFertilityEnd)
			{
				return MathUtil.Lerp(1.0f, 0.0f, MathUtil.Map(MalePeakFertilityEnd, ChildbearingThreshold, Age));
			}
			else if (Age < MalePeakFertilityBegin)
			{
				return MathUtil.Lerp(0.5f, 1.0f, MathUtil.Map(ComingOfAgeThreshold, MalePeakFertilityBegin, Age));
			}
			else
			{
				return 1.0f;
			}
		}

		private float GetFemaleFertility()
		{
			const int FemalePeakFertilityBegin = 25;
			const int FemalePeakFertilityEnd = 35;
			const int ChildbearingThreshold = 50;

			if (Age > ChildbearingThreshold)
			{
				return 0;
			}
			else if (Age > FemalePeakFertilityEnd)
			{
				return MathUtil.Lerp(1.0f, 0.0f, MathUtil.Map(FemalePeakFertilityEnd, ChildbearingThreshold, Age));
			}
			else if (Age < FemalePeakFertilityBegin)
			{
				return MathUtil.Lerp(0.5f, 1.0f, MathUtil.Map(ComingOfAgeThreshold, FemalePeakFertilityBegin, Age));
			}
			else
			{
				return 1.0f;
			}
		}

		private void CheckForBirthComplications(Person baby)
		{
			if (rand.NextDouble() > LiveBirthChance)
			{
				baby.Die();
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " gave birth, but the baby did not survive.");
			}

			if (rand.NextDouble() > SurviveChildbirthChance)
			{
				Die();
				Messenger.PostMessageToPlayer(Village.OwnerPlayer, DisplayName + " died in childbirth.");
			}

			if (rand.NextDouble() > SurviveInfancyChance)
			{
				baby.deathCounter = 2;
			}

			if (rand.NextDouble() > SurviveChildhoodChance)
			{
				baby.deathCounter = (ComingOfAgeThreshold * 2) - rand.Next(1, 14);
			}
		}

		public string GetDebugString()
		{
			string output = "[" + Uid + "] " + DisplayName + " - " + Sex + ", " + Age + ", " + Class.Name;

			List<string> attributes = new List<string>();
			if (IsChief)
			{
				attributes.Add("Chief");
			}
			if (IsPregnant)
			{
				attributes.Add("Pregnant");
			}
			if (Starving)
			{
				attributes.Add("Starving");
			}
			if (Dead)
			{
				attributes.Add("Dead");
			}

			if (attributes.Count > 0)
			{
				output += " (" + string.Join(", ", attributes.ToArray()) + ")";
			}

			return output;
		}
	}
}
