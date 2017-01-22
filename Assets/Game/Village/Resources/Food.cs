namespace Sovereign
{
	public sealed class Food : Resource
	{
		public override string Name { get { return "Food"; } }

		public Food(int x) : base(x)
		{
		}
	}
}
