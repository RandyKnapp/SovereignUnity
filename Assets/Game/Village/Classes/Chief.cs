namespace Sovereign
{
	public class Chief : PersonClass
	{
		public override string Name { get { return "Chief"; } }
		
		public override ResourcePack Produce()
		{
			return new ResourcePack();
		}
	}
}
