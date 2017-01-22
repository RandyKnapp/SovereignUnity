using System.Linq;
using System.Collections.Generic;

namespace Sovereign
{
	public sealed class ResourcePack
	{
		private readonly Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

		public List<Resource> Resources { get { return resources.Values.ToList(); } }

		public ResourcePack(params Resource[] args)
		{
			foreach (Resource resource in args)
			{
				Add(resource);
			}
		}

		public void Add(Resource resource)
		{
			Resource currentResource = GetResource(resource);
			if (currentResource == null)
			{
				resources.Add(resource.Name, resource);
			}
			else
			{
				currentResource.Add(resource);
			}
		}

		public void Add(ResourcePack resourcePack)
		{
			foreach (Resource resource in resourcePack.Resources)
			{
				Add(resource);
			}
		}

		public bool CanRemove(Resource resource)
		{
			Resource currentResource = GetResource(resource);
			if (currentResource == null)
			{
				return false;
			}
			else
			{
				return currentResource.CanRemove(resource);
			}
		}

		public bool CanRemove(ResourcePack resourcePack)
		{
			foreach (Resource resource in resourcePack.Resources)
			{
				if (!CanRemove(resource))
				{
					return false;
				}
			}

			return true;
		}

		public void Remove(Resource resource)
		{
			if (CanRemove(resource))
			{
				Resource currentResource = GetResource(resource);
				currentResource.Remove(resource);
			}
		}

		public void Remove(ResourcePack resourcePack)
		{
			foreach (Resource resource in resourcePack.Resources)
			{
				Remove(resource);
			}
		}

		public Resource GetResource(Resource resource)
		{
			return GetResource(resource.Name);
		}

		public Resource GetResource(string resourceName)
		{
			Resource resource;
			resources.TryGetValue(resourceName, out resource);
			return resource;
		}

		public int GetResourceCount(string resourceName)
		{
			Resource r = GetResource(resourceName);
			return r != null ? r.Count : 0;
		}
	}
}
