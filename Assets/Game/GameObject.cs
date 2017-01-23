namespace Sovereign
{
	public partial class GameObject
	{
		private uint uid;

		public uint Uid { get { return uid;} }

		public GameObject()
		{
			uid = UidManager.GetUid();
			AddGameObject(this);
		}
	}
}
