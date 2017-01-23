namespace Sovereign
{
	public sealed class UidManager
	{
		private static uint uid = 100;

		public static uint GetUid()
		{
			return uid++;
		}
	}
}
