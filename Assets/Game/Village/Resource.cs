namespace Sovereign
{
	public abstract class Resource
	{
		private int count;

		public abstract string Name { get; }
		public int Count { get { return count; } }
		
		public Resource(int x)
		{
			count = x;
		}

		public Resource Add(int x)
		{
			count += x;
			return this;
		}

		public Resource Add(Resource r)
		{
			return Add(r.count);
		}

		public bool CanRemove(int x)
		{
			return x <= count;
		}

		public bool CanRemove(Resource r)
		{
			return CanRemove(r.count);
		}

		public void Remove(int x)
		{
			if (CanRemove(x))
			{
				count -= x;
			}
		}

		public void Remove(Resource r)
		{
			Remove(r.count);
		}
	}
}
