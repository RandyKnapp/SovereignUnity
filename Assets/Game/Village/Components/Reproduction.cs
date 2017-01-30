using System;

namespace Sovereign
{
	public class Reproduction : PersonComponent, ILifeCycleHandler
	{
		private static readonly Random rand = new Random();

		private Sex sex = Sex.Male;

		public Sex Sex { get { return sex; } set { sex = value; } }

		public Reproduction(Person person) : base(person)
		{
		}

		public void BeBorn()
		{
			sex = rand.Next(0, 2) == 0 ? Sex.Male : Sex.Female;
		}

		public void AgeOneSeason()
		{
		}

		public void Die()
		{
		}

		public float GetFertility()
		{
			if (person.IsChild)
			{
				return 0;
			}

			return sex == Sex.Male ? GetMaleFertility() : GetFemaleFertility();
		}

		private float GetMaleFertility()
		{
			const int MalePeakFertilityBegin = 20;
			const int MalePeakFertilityEnd = 40;
			const int ChildbearingThreshold = 80;

			if (person.Age > ChildbearingThreshold)
			{
				return 0;
			}
			else if (person.Age > MalePeakFertilityEnd)
			{
				return MathUtil.Lerp(1.0f, 0.0f, MathUtil.Map(MalePeakFertilityEnd, ChildbearingThreshold, person.Age));
			}
			else if (person.Age < MalePeakFertilityBegin)
			{
				return MathUtil.Lerp(0.5f, 1.0f, MathUtil.Map(Lifespan.ComingOfAgeThreshold, MalePeakFertilityBegin, person.Age));
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

			if (person.Age > ChildbearingThreshold)
			{
				return 0;
			}
			else if (person.Age > FemalePeakFertilityEnd)
			{
				return MathUtil.Lerp(1.0f, 0.0f, MathUtil.Map(FemalePeakFertilityEnd, ChildbearingThreshold, person.Age));
			}
			else if (person.Age < FemalePeakFertilityBegin)
			{
				return MathUtil.Lerp(0.5f, 1.0f, MathUtil.Map(Lifespan.ComingOfAgeThreshold, FemalePeakFertilityBegin, person.Age));
			}
			else
			{
				return 1.0f;
			}
		}
	}
}