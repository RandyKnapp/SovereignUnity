using System;

namespace Sovereign
{
	public class Childbearing : PersonComponent, ILifeCycleHandler
	{
		private static readonly Random rand = new Random();

		private const int PregnancyDuration = 2;
		private const float BasePregnancyChance = 0.25f;
		private const float LiveBirthChance = 0.6f;
		private const float SurviveChildbirthChance = 0.9f;
		private const float SurviveInfancyChance = 0.5f;
		private const float SurviveChildhoodChance = 0.5f;

		private readonly Reproduction reproduction;
		private int pregnancyCounter;

		public bool IsPregnant { get { return pregnancyCounter > 0; } }

		public event Action<Person, Person> OnHaveBaby = delegate {};

		public Childbearing(Person person, Reproduction reproduction) : base(person)
		{
			this.reproduction = reproduction;
		}

		public void BeBorn()
		{
		}

		public void AgeOneSeason()
		{
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
		}

		public void CheckForPregnancy()
		{
			if (person.Sex == Sex.Male || IsPregnant)
			{
				return;
			}

			Person mate = person.Family.GetPotentialMate();
			if (mate == null)
			{
				return;
			}

			float seasonalModifier = GameManager.Instance.Timeline.Season == Season.Winter ? 1.0f : 0.5f;
			float marriageModifier = person.Family.HasSpouse() ? 1.0f : 0.2f;
			float fertility = reproduction.GetFertility();
			float familySizeChance = MathUtil.Lerp(1.0f, 0.01f, person.Family.NumberOfChildren() / 5.0f);
			const float basePregnancyChance = BasePregnancyChance;
			float pregnancyChance = basePregnancyChance * fertility * seasonalModifier * marriageModifier * familySizeChance;

			bool pregnant = rand.NextDouble() < pregnancyChance;
			if (pregnant)
			{
				if (mate != person.Family.Spouse)
				{
					person.Family.Marry(mate);
				}

				pregnancyCounter = PregnancyDuration;
				Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " is pregnant!");
			}
		}

		public void HaveBaby()
		{
			pregnancyCounter = 0;

			Person baby = new Person();
			baby.BeBorn();
			Family.AddChildToFamily(person, baby);
			OnHaveBaby(person, baby);

			CheckForBirthComplications(baby);
			if (!baby.IsDead)
			{
				Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " gave birth to a baby! (" + baby.DisplayName + ", " + baby.Sex + ")");
			}
		}

		private void CheckForBirthComplications(Person baby)
		{
			if (rand.NextDouble() > SurviveChildbirthChance)
			{
				person.Die();
				Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " died in childbirth.");
			}

			if (rand.NextDouble() > LiveBirthChance)
			{
				baby.Die();
				Messenger.PostMessageToPlayer(person.Village.OwnerPlayer, person.DisplayName + " gave birth, but the baby did not survive.");
			}
			else if (rand.NextDouble() > SurviveInfancyChance)
			{
				baby.Lifespan.SetDeathAge(1);
			}
			else if (rand.NextDouble() > SurviveChildhoodChance)
			{
				baby.Lifespan.SetDeathAge(Lifespan.ComingOfAgeThreshold - rand.Next(1, 7));
			}
		}

	}
}