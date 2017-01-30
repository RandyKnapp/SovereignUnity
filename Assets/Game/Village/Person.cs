using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Assertions.Must;

namespace Sovereign
{
	public enum Sex
	{
		Female,
		Male
	}
	
	public class Person : GameObject, IProducer, ILifeCycleHandler
	{
		private const int StarvationThreshold = 3;
		private static readonly Random rand = new Random();

		private readonly List<PersonComponent> components = new List<PersonComponent>();
		private readonly Family family;
		private readonly Lifespan lifespan;
		private readonly Reproduction reproduction;
		private readonly Childbearing childbearing;

		private int starvingCounter;
		private PersonClass personClass;

		public Village Village { get; private set; }
		public Family Family { get { return family; } }
		public Lifespan Lifespan { get { return lifespan; } }
		public int Age { get { return lifespan.Age; } }
		public Sex Sex { get { return reproduction.Sex; } }
		public bool IsSlave { get { return family.HasOwner(); } }
		public bool IsChild { get { return lifespan.IsChild; } }
		public bool IsDead { get { return lifespan.IsDead; } }
		public bool IsPregnant { get { return childbearing.IsPregnant; } }

		public bool IsChief { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string DisplayName { get { return (Title != null ? Title + " " : "") + Name; } }
		public PersonClass Class { get { return personClass; } set { personClass = value; } }
		public bool Starving { get { return starvingCounter > 0; } }

		public event Action<Person, Person> OnMarried = delegate {};
		public event Action<Person> OnDeath = delegate {};
		public event Action<Person> OnComingOfAge = delegate {};
		public event Action<Person, Person> OnHaveBaby = delegate {};

		public Person()
		{
			family = AddComponent(new Family(this));
			lifespan = AddComponent(new Lifespan(this));
			reproduction = AddComponent(new Reproduction(this));
			childbearing = AddComponent(new Childbearing(this, reproduction));

			family.OnMarried += OnMarriedHandler;
			lifespan.OnDeath += OnDeathHandler;
			lifespan.OnComingOfAge += OnComingOfAgeHandler;
			childbearing.OnHaveBaby += OnHaveBabyHandler;
		}

		private T AddComponent<T>(T comp) where T : PersonComponent
		{
			components.Add(comp);
			return comp;
		}

		private void OnMarriedHandler(Person person, Person spouse)
		{
			OnMarried(person, spouse);
		}

		private void OnDeathHandler(Person person)
		{
			OnDeath(person);
		}

		private void OnComingOfAgeHandler(Person person)
		{
			personClass = new Farmer();
			OnComingOfAge(person);
		}

		private void OnHaveBabyHandler(Person parent, Person baby)
		{
			baby.Village = parent.Village;
			OnHaveBaby(parent, baby);
		}

		public static Person GenerateStartingChief(Village village)
		{
			Person person = GenerateStartingPerson(village, 20, 40);
			person.Class = new Chief();
			person.IsChief = true;
			person.Title = Names.GetRandom(Names.ChiefTitles);

			return person;
		}

		public static Person GenerateStartingChild(Village village)
		{
			Person person = GenerateStartingPerson(village, 0, Lifespan.ComingOfAgeThreshold);
			person.Class = new Child();
			return person;
		}

		public static Person GenerateStartingPerson(Village village, Sex startingSex)
		{
			Person person = GenerateStartingPerson(village);
			person.reproduction.Sex = startingSex;
			return person;
		}

		public static Person GenerateStartingPerson(Village village, int ageMin = 14, int ageMax = 31)
		{
			Person person = new Person();
			person.BeBorn();
			person.lifespan.SetStartingAge(rand.Next(ageMin, ageMax));
			person.Village = village;
			person.Class = new Farmer();

			return person;
		}

		public void BeBorn()
		{
			Name = Names.GetRandomPersonName(Sex);
			personClass = new Child();
			foreach (ILifeCycleHandler handler in components.OfType<ILifeCycleHandler>())
			{
				handler.BeBorn();
			}
		}

		public void AgeOneSeason()
		{
			foreach (ILifeCycleHandler handler in components.OfType<ILifeCycleHandler>())
			{
				handler.AgeOneSeason();
			}
		}

		public void Die()
		{
			foreach (ILifeCycleHandler handler in components.OfType<ILifeCycleHandler>())
			{
				handler.Die();
			}
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

		public void BecomeFreePerson()
		{
			if (IsSlave)
			{
				Class = new Farmer();
				Family.FreeSlave(Family.Owner, this);
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

		public string GetDebugString()
		{
			string output = "[" + Uid + "] " + DisplayName + " - " + Sex + ", " + Age + ", " + Class.Name;

			List<string> attributes = new List<string>();
			if (IsChief)
			{
				attributes.Add("Chief");
			}
			if (family.HasSpouse())
			{
				attributes.Add("Married:" + family.Spouse.DisplayName);
			}
			if (IsPregnant)
			{
				attributes.Add("Pregnant");
			}
			if (Starving)
			{
				attributes.Add("Starving");
			}
			if (IsDead)
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
