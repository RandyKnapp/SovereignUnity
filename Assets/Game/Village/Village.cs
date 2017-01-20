namespace Sovereign
{
	public class Village
	{
		private string name;
		private Player player;

		public string Name { get { return name; } private set { name = value; } }
		public Player OwnerPlayer { get { return player; } private set { player = value; } }

		public Village(Player player, string name)
		{
			OwnerPlayer = player;
			Name = name;
		}
	}
}