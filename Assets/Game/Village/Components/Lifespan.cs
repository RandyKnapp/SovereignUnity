using System;

namespace Sovereign
{
	public class Lifespan : PersonComponent, ILifeCycleHandler
	{
		private static readonly Random rand = new Random();

		public const int ComingOfAgeThreshold = 13;
		private const int AverageLifespan = 50;
		private const int LifespanRange = 10;

		private bool dead;
		private Season birthSeason;
		private int lifeCounter;
		private int deathCounter;

		public bool IsDead { get { return dead; } }
		public Season BirthSeason { get { return birthSeason; } }
		public int Age { get { return lifeCounter / 2; } set { lifeCounter = value * 2; } }
		public bool IsChild { get { return Age < ComingOfAgeThreshold; } }

		public event Action<Person> OnDeath = delegate {};
		public event Action<Person> OnComingOfAge = delegate {};

		public Lifespan(Person person) : base(person)
		{
		}

		public void SetStartingAge(int age)
		{
			lifeCounter = age * 2;
			birthSeason = rand.Next(0, 2) == 0 ? Season.Summer : Season.Winter;
			lifeCounter += birthSeason == Season.Winter ? 1 : 0;
		}

		public void SetDeathAge(int age)
		{

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
			bool wasAlive = !IsDead;
			lifeCounter++;
			if (wasChild && !IsChild)
			{
				OnComingOfAge(person);
			}

			if (wasAlive && lifeCounter > deathCounter)
			{
				Die();
				if (Age < 2)
				{
					Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " has died in infancy!");
				}
				else if (Age < ComingOfAgeThreshold)
				{
					Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " has died from sickness as a child!");
				}
				else
				{
					Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " has died of old age!");
				}
			}

			if (IsDead)
			{
				return;
			}
		}

		public void Die()
		{
			dead = true;
			OnDeath(person);
		}
	}
}