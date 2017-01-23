using System.Collections.Generic;

namespace Sovereign
{
	public partial class GameObject
	{
		private static readonly Dictionary<uint, GameObject> allObjects = new Dictionary<uint, GameObject>();

		private static void AddGameObject(GameObject gameObject)
		{
			if (!allObjects.ContainsKey(gameObject.Uid))
			{
				allObjects.Add(gameObject.Uid, gameObject);
			}
		}

		public static GameObject GetGameObject(uint uid)
		{
			GameObject result;
			allObjects.TryGetValue(uid, out result);
			return result;
		}
	}
}
