using System.Collections.Generic;

namespace Sovereign
{
	public interface ICommandHandler
	{
		bool CanHandleCommand(Player player, string command, List<string> args);
		void HandleCommand(Player player, string command, List<string> args);
	}
}