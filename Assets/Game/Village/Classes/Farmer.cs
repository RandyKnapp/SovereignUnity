namespace Sovereign
{
	public class Farmer : PersonClass
	{
		public override string Name { get { return "Farmer"; } }
		
		public override ResourcePack Produce()
		{
			return new ResourcePack(new Food(2));
		}
	}
}
