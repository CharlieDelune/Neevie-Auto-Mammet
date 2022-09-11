using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using NeevieAutoMammet.Attributes;
using static Dalamud.Game.Command.CommandInfo;

namespace NeevieAutoMammet;

public class PluginCommandManager<THost> : IDisposable
{
	private readonly CommandManager _commandManager;
	private readonly THost _host;
	private readonly (string, CommandInfo)[] _pluginCommands;

	public PluginCommandManager(THost host, CommandManager commandManager)
	{
		_commandManager = commandManager;
		_host = host;

		_pluginCommands = _host.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
		                                             BindingFlags.Static | BindingFlags.Instance)
			.Where(method => method.GetCustomAttribute<CommandAttribute>() != null)
			.SelectMany(GetCommandInfoTuple)
			.ToArray();

		AddCommandHandlers();
	}

	public void Dispose()
	{
		RemoveCommandHandlers();
		GC.SuppressFinalize(this);
	}

	private void AddCommandHandlers()
	{
		foreach ((string command, CommandInfo commandInfo) in _pluginCommands)
		{
			_commandManager.AddHandler(command, commandInfo);
		}
	}

	private IEnumerable<(string, CommandInfo)> GetCommandInfoTuple(MethodInfo method)
	{
		HandlerDelegate handlerDelegate =
			(HandlerDelegate)Delegate.CreateDelegate(typeof(HandlerDelegate), _host, method);

		CommandAttribute? command = handlerDelegate.Method.GetCustomAttribute<CommandAttribute>();
		AliasesAttribute? aliases = handlerDelegate.Method.GetCustomAttribute<AliasesAttribute>();
		HelpMessageAttribute? helpMessage = handlerDelegate.Method.GetCustomAttribute<HelpMessageAttribute>();

		CommandInfo commandInfo = new CommandInfo(handlerDelegate)
		{
			HelpMessage = helpMessage?.HelpMessage ?? string.Empty
		};

		// Create list of tuples that will be filled with one tuple per alias, in addition to the base command tuple.
		List<(string, CommandInfo)> commandInfoTuples = new List<(string, CommandInfo)>
			{ (command!.Command, commandInfo) };
		if (aliases != null)
		{
			foreach (string alias in aliases.Aliases)
			{
				commandInfoTuples.Add((alias, commandInfo));
			}
		}

		return commandInfoTuples;
	}

	private void RemoveCommandHandlers()
	{
		foreach ((string command, var _) in _pluginCommands)
		{
			_commandManager.RemoveHandler(command);
		}
	}
}