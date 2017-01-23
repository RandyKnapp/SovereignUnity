using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	public sealed class FamilyManager
	{
		private readonly Dictionary<uint, Family> families = new Dictionary<uint, Family>();

		public List<Family> Families { get { return families.Values.ToList(); } }

		public void AddFamily(Family family)
		{
			if (!families.ContainsKey(family.Uid))
			{
				families.Add(family.Uid, family);
			}
		}

		public Family GetFamilyForPerson(Person person)
		{
			List<Family> foundFamilies = families.Values.Where(f => f.HasMember(person)).ToList();
			return (foundFamilies != null && foundFamilies.Count > 0) ? foundFamilies[0] : null;
		}
	}
}
