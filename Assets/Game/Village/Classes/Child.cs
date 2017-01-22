namespace Sovereign
{
	public class Child : PersonClass
	{
		public override string Name { get { return "Child"; } }
		
		public override ResourcePack Produce()
		{
			return new ResourcePack();
		}
	}
}
