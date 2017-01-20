using System.Collections.Generic;

namespace Sovereign
{
	public class VillageManager : ICommandHandler
	{
		public bool CanHandleCommand(Player player, string command, List<string> args)
		{
			return true;
		}

		public void HandleCommand(Player player, string command, List<string> args)
		{
			throw new System.NotImplementedException();
		}
	}
}

