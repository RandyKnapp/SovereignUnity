namespace Sovereign
{
	public abstract class PersonClass : IProducer
	{
		public abstract string Name { get; }
		public abstract void Produce();
	}
}
