namespace Sovereign
{
	public class Slave : PersonClass
	{
		public override string Name { get { return "Slave"; } }
		
		public override ResourcePack Produce()
		{
			return new ResourcePack();
		}
	}
}
