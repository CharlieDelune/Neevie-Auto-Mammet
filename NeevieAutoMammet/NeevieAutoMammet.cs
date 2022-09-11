using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using NeevieAutoMammet.Attributes;
using NeevieAutoMammet.Constants;
using NeevieAutoMammet.Windows;

namespace NeevieAutoMammet;

public sealed class NeevieAutoMammet : IDalamudPlugin
{
	private readonly PluginCommandManager<NeevieAutoMammet> _pluginCommandManager;
	private readonly WindowManager _windowManager;
	public WindowSystem WindowSystem = new(Global.PLUGIN_NAME);

	public NeevieAutoMammet(
		[RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
		[RequiredVersion("1.0")] CommandManager commands)
	{
		PluginInterface = pluginInterface;

		Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
		Configuration.Initialize(PluginInterface);

		// you might normally want to embed resources and load them from the manifest stream
		// var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
		// var goatImage = PluginInterface.UiBuilder.LoadImage(imagePath);

		_pluginCommandManager = new PluginCommandManager<NeevieAutoMammet>(this, commands);

		_windowManager = new WindowManager(this, pluginInterface, Configuration);
	}

	private Configuration Configuration { get; }
	private DalamudPluginInterface PluginInterface { get; }
	public string Name => Global.PLUGIN_NAME;

	public void Dispose()
	{
		_pluginCommandManager.Dispose();
		_windowManager.Dispose();
	}

	[Command(Commands.BASE_COMMAND)]
	// [Aliases("/betterplaytime")]
	[HelpMessage("Opens Auto-Mammet Emote Generator\nOptional Arguments:\nconfig - Opens Config Window")]
	public void PluginCommand(string command, string args)
	{
		switch (args)
		{
			case Commands.CONFIG_ARG:
				_windowManager.DrawConfigWindow();
				break;
			default:
				_windowManager.DrawMainWindow();
				break;
		}
	}
}