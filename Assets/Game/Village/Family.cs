using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	// TODO: Allow polygamy?????
	public sealed class Family
	{
		private readonly uint rootPerson;
		private uint spouse;
		private uint owner;
		private readonly List<uint> parents = new List<uint>();
		private readonly List<uint> siblings = new List<uint>();
		private readonly List<uint> children = new List<uint>();
		private readonly List<uint> slaves = new List<uint>();

		public Person RootPerson { get { return GameObject.GetGameObject<Person>(rootPerson); } }
		public Person Spouse { get { return GameObject.GetGameObject<Person>(spouse); } }
		public Person Owner { get { return GameObject.GetGameObject<Person>(owner); } }
		public List<Person> Parents { get { return ListAsListOfPeople(parents); } }
		public List<Person> Siblings { get { return ListAsListOfPeople(parents); } }
		public List<Person> Children { get { return ListAsListOfPeople(parents); } }
		public List<Person> Slaves { get { return ListAsListOfPeople(parents); } }

		public Family(Person root)
		{
			rootPerson = root.Uid;
		}

		private void AddPersonToList(List<uint> list, Person person)
		{
			if (!list.Contains(person.Uid))
			{
				list.Add(person.Uid);
			}
		}

		public void AddSpouse(Person spouse)
		{
			if (!HasSpouse())
			{
				this.spouse = spouse.Uid;
			}
		}

		public void AddParent(Person parent)
		{
			AddPersonToList(parents, parent);
		}

		public void AddChild(Person child)
		{
			AddPersonToList(children, child);
		}

		public void AddSibling(Person sibling)
		{
			AddPersonToList(siblings, sibling);
		}

		public void AddSlave(Person slave)
		{
			AddPersonToList(slaves, slave);
		}

		public void RemoveSlave(Person slave)
		{
			slaves.Remove(slave.Uid);
		}

		public void AddOwner(Person owner)
		{
			this.owner = owner.Uid;
		}

		public void RemoveOwner()
		{
			owner = 0;
		}

		private List<Person> ListAsListOfPeople(List<uint> list)
		{
			return list.Select(uid => GameObject.GetGameObject<Person>(uid)).ToList();
		}
		
		public bool IsParentOf(Person child)
		{
			return children.Contains(child.Uid);
		}

		public bool IsChildOf(Person parent)
		{
			return parents.Contains(parent.Uid);
		}

		public bool IsSibling(Person sibling)
		{
			return siblings.Contains(sibling.Uid);
		}

		public bool IsSpouse(Person spouse)
		{
			return this.spouse == spouse.Uid;
		}

		public bool HasSpouse()
		{
			return spouse != 0;
		}

		public bool IsOwnerOf(Person slave)
		{
			return slaves.Contains(slave.Uid);
		}

		public bool IsSlaveOf(Person owner)
		{
			return this.owner == owner.Uid;
		}

		public bool IsRelated(Person person)
		{
			return IsSpouse(person) || IsChildOf(person) || IsParentOf(person) || IsSibling(person) || IsOwnerOf(person) || IsSlaveOf(person);
		}

		public bool HasChildren()
		{
			return children.Count > 0;
		}

		public int NumberOfChildren()
		{
			return children.Count;
		}

		public bool OwnsSlaves()
		{
			return slaves.Count > 0;
		}

		public int NumberOfSlaves()
		{
			return slaves.Count;
		}

		public bool HasOwner()
		{
			return owner != 0;
		}

		public Person GetHeir()
		{
			Person heir = null;
			if (HasChildren())
			{
				List<Person> maleChildren = Children.Where(p => !p.IsChild && p.Sex == Sex.Male).ToList();
				if (maleChildren.Count > 0)
				{
					maleChildren.Sort((a, b) => b.Age.CompareTo(a.Age));
					heir = maleChildren[0];
				}
			}

			// Spouse
			if (heir == null)
			{
				if (HasSpouse())
				{
					heir = Spouse;
				}
			}

			// Oldest adult female child
			if (heir == null)
			{
				List<Person> femaleChildren = Children.Where(p => !p.IsChild && p.Sex == Sex.Female).ToList();
				if (femaleChildren.Count > 0)
				{
					femaleChildren.Sort((a, b) => b.Age.CompareTo(a.Age));
					heir = femaleChildren[0];
				}
			}

			// Oldest adult brother
			if (heir == null)
			{
				List<Person> maleSiblings = Siblings.Where(p => !p.IsChild && p.Sex == Sex.Male).ToList();
				if (maleSiblings.Count > 0)
				{
					maleSiblings.Sort((a, b) => b.Age.CompareTo(a.Age));
					heir = maleSiblings[0];
				}
			}

			return heir;
		}

		public static void AddChildToFamily(Person parent, Person child)
		{
			Person parentSpouse = parent.Family.Spouse;
			List<Person> newSiblings = parent.Family.Children;

			parent.Family.AddChild(child);
			parentSpouse.Family.AddChild(child);
			child.Family.AddParent(parent);
			child.Family.AddParent(parentSpouse);
			foreach (Person sibling in newSiblings)
			{
				sibling.Family.AddSibling(child);
				child.Family.AddSibling(sibling);
			}
		}

		public static void AddSlaveToOwner(Person owner, Person slave)
		{
			if (slave.Family.HasOwner())
			{
				Person formerOwner = slave.Family.Owner;
				formerOwner.Family.RemoveSlave(slave);
				slave.Family.RemoveOwner();
			}

			owner.Family.AddSlave(slave);
			slave.Family.AddOwner(owner);
		}

		public static void FreeSlave(Person owner, Person slave)
		{
			if (owner.Family.IsOwnerOf(slave))
			{
				owner.Family.RemoveSlave(slave);
				slave.Family.RemoveOwner();
			}
		}

		public static void HandleDeath(Person person)
		{
			Family f = person.Family;
			if (f.HasSpouse())
			{
				f.Spouse.Family.spouse = 0;
			}
			if (f.HasChildren())
			{
				f.Children.ForEach(child => child.Family.parents.Remove(person.Uid));
			}
			if (f.Siblings.Count > 0)
			{
				f.Siblings.ForEach(sib => sib.Family.siblings.Remove(person.Uid));
			}
			if (f.Parents.Count > 0)
			{
				f.Parents.ForEach(parent => parent.Family.children.Remove(person.Uid));
			}
			if (f.OwnsSlaves())
			{
				Person heir = f.GetHeir();
				if (heir != null)
				{
					f.Slaves.ForEach(slave => Family.AddSlaveToOwner(heir, slave));
				}
				else
				{
					f.Slaves.ForEach(slave => Family.FreeSlave(person, slave));
				}
			}
			if (f.HasOwner())
			{
				f.Owner.Family.slaves.Remove(person.Uid);
			}
		}
	}
}
