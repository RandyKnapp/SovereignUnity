using System;
using System.Collections.Generic;

namespace Sovereign
{
	public class Commands
	{
		private class CommandData
		{
			public string command;
			public string helpText;
			public int requiredArgs;
			public List<Action<Player, string, List<string>>> handlers = new List<Action<Player, string, List<string>>>();
		};

		private static readonly Dictionary<string, CommandData> handlers = new Dictionary<string, CommandData>();
		private static IPlayerMessenger messenger;

		public static void Initialize(IPlayerMessenger externalMessenger)
		{
			messenger = externalMessenger;
		}

		public static void Add(string command, Action<Player, string, List<string>> handler, int requiredArgs, string helpText, string[] aliases = null)
		{
			CommandData data;
			if (!HasCommand(command))
			{
				data = new CommandData();
				data.command = command;
				data.helpText = helpText;
				data.requiredArgs = requiredArgs;
				handlers.Add(command, data);
			}
			else
			{
				data = handlers[command];
			}
			
			data.handlers.Add(handler);
			
			if (aliases != null)
			{
				foreach (string alias in aliases)
				{
					Add(alias, handler, requiredArgs, helpText);
				}
			}
		}

		public static bool HasCommand(string command)
		{
			return handlers.ContainsKey(command);
		}

		public static void Call(Player player, string command, List<string> args)
		{
			command = PreprocessCommand(command, args);

			if (!ValidateCommand(player, command, args))
			{
				return;
			}

			CommandData data;
			handlers.TryGetValue(command, out data);
			if (data != null)
			{
				foreach (var handler in data.handlers)
				{
					handler(player, command, args);
				}
			}
		}

		private static string PreprocessCommand(string command, List<string> args)
		{
			return command.ToLowerInvariant();
		}

		private static bool ValidateCommand(Player player, string command, List<string> args)
		{
			CommandData data;
			handlers.TryGetValue(command, out data);
			if (data != null)
			{
				if (args.Count == data.requiredArgs)
				{
					return true;
				}
				else
				{
					messenger.PostMessageToPlayer(player, "Command '" + command + "' has an incorrect number of arguments");
					PrintHelp(player, command);
					return false;
				}
			}

			messenger.PostMessageToPlayer(player, "Command '" + command + "' not found!");
			return false;
		}

		private static void PrintHelp(Player player, string command)
		{
			CommandData data;
			handlers.TryGetValue(command, out data);
			if (data != null)
			{
				string message = data.helpText;
				messenger.PostMessageToPlayer(player, message);
			}
		}
	}
}
