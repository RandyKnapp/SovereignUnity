using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	public enum FamilyConnectionType
	{
		Spouse,
		Parent,
		Child,
		Sibling,
		Slave,
		Owner
	}

	class FamilyConnection
	{
		public Person from;
		public Person to;
		public FamilyConnectionType type;
	}

	// TODO: Allow polygamy?????
	public sealed class Family : GameObject
	{
		private readonly List<Person> members = new List<Person>();
		private readonly List<FamilyConnection> connections = new List<FamilyConnection>();

		public string Name { get; private set; }
		public Person HeadOfFamily { get; private set; }

		private void AddPersonAsMember(Person person)
		{
			if (!members.Contains(person))
			{
				members.Add(person);
				person.Family = this;
			}
		}

		private void AddConnection(Person from, Person to, FamilyConnectionType type)
		{
			FamilyConnection connection = new FamilyConnection();
			connection.from = from;
			connection.to = to;
			connection.type = type;
			connections.Add(connection);
		}

		private void AddTwoWayConnection(Person a, Person b, FamilyConnectionType type)
		{
			AddConnection(a, b, type);
			AddConnection(b, a, type);
		}

		public void AddHeadOfFamily(Person person)
		{
			HeadOfFamily = person;
			AddPersonAsMember(person);
			Name = person.Name;
		}

		public void AddSpouse(Person member, Person spouse)
		{
			AddPersonAsMember(spouse);
			AddTwoWayConnection(member, spouse, FamilyConnectionType.Spouse);
		}

		public void AddChild(Person parentA, Person parentB, Person child)
		{
			List<Person> otherChildren = GetChildren(parentA);

			AddPersonAsMember(child);
			AddConnection(parentA, child, FamilyConnectionType.Child);
			AddConnection(parentB, child, FamilyConnectionType.Child);
			AddConnection(child, parentA, FamilyConnectionType.Parent);
			AddConnection(child, parentB, FamilyConnectionType.Parent);

			foreach (Person sibling in otherChildren)
			{
				AddTwoWayConnection(child, sibling, FamilyConnectionType.Sibling);
			}
		}

		public void AddSibling(Person person, Person sibling)
		{
			AddPersonAsMember(sibling);
			AddTwoWayConnection(person, sibling, FamilyConnectionType.Sibling);
		}

		public void AddSlave(Person owner, Person slave)
		{
			// TODO: Remove slave from previous owner
			AddPersonAsMember(slave);
			AddConnection(owner, slave, FamilyConnectionType.Owner);
			AddConnection(slave, owner, FamilyConnectionType.Slave);
		}

		private List<FamilyConnection> GetAllRelationships(Person person)
		{
			return connections.Where(c => c.from == person).ToList();
		}

		private List<Person> GetAllRelationshipsOfType(Person p, FamilyConnectionType type)
		{
			List<FamilyConnection> allRelationships = GetAllRelationships(p);
			return allRelationships.Where(r => r.type == type && !r.to.Dead).Select(r => r.to).ToList();
		}

		public List<Person> GetChildren(Person parent)
		{
			return GetAllRelationshipsOfType(parent, FamilyConnectionType.Child);
		}

		public List<Person> GetParents(Person child)
		{
			return GetAllRelationshipsOfType(child, FamilyConnectionType.Parent);
		}

		public List<Person> GetSiblings(Person person)
		{
			return GetAllRelationshipsOfType(person, FamilyConnectionType.Sibling);
		}

		public Person GetSpouse(Person person)
		{
			var spouses = GetAllRelationshipsOfType(person, FamilyConnectionType.Spouse);
			return (spouses != null && spouses.Count == 1) ? spouses[0] : null;
		}

		public Person GetOwner(Person slave)
		{
			var owners = GetAllRelationshipsOfType(slave, FamilyConnectionType.Owner);
			return (owners != null && owners.Count == 1) ? owners[0] : null;
		}

		public List<Person> GetSlaves(Person owner)
		{
			return GetAllRelationshipsOfType(owner, FamilyConnectionType.Owner);
		}

		public bool IsParentOf(Person parent, Person child)
		{
			return GetChildren(parent).Contains(child);
		}

		public bool IsChildOf(Person child, Person parent)
		{
			return GetParents(child).Contains(parent);
		}

		public bool AreSiblings(Person personA, Person personB)
		{
			return GetSiblings(personA).Contains(personB);
		}

		public bool AreMarried(Person personA, Person personB)
		{
			return GetSpouse(personA) == personB;
		}

		public bool HasSpouse(Person person)
		{
			return GetSpouse(person) != null;
		}

		public bool IsOwnedBy(Person slave, Person owner)
		{
			return GetOwner(slave) == owner;
		}

		public bool IsSlaveOf(Person owner, Person slave)
		{
			return GetSlaves(owner).Contains(slave);
		}

		public bool HasMember(Person person)
		{
			return members.Contains(person);
		}

		public bool HasChildren(Person parent)
		{
			return NumberOfChildren(parent) > 0;
		}

		public int NumberOfChildren(Person parent)
		{
			return GetChildren(parent).Count;
		}

		public bool OwnsSlaves(Person owner)
		{
			return NumberOfSlaves(owner) > 0;
		}

		public int NumberOfSlaves(Person owner)
		{
			return GetSlaves(owner).Count;
		}
	}
}
