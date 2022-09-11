using System;
using System.IO;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Data;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using NeevieAutoMammet.Attributes;
using NeevieAutoMammet.Windows;

namespace NeevieAutoMammet
{
    public sealed class NeevieAutoMammet : IDalamudPlugin
    {
        public string Name => Constants.Global.PLUGIN_NAME;
        public WindowSystem WindowSystem = new(Constants.Global.PLUGIN_NAME);
        
        private Configuration Configuration { get; set; }
        private DalamudPluginInterface PluginInterface { get; init; }
        private readonly PluginCommandManager<NeevieAutoMammet> pluginCommandManager;
        private readonly WindowManager windowManager;

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

            pluginCommandManager = new PluginCommandManager<NeevieAutoMammet>(this, commands);

            windowManager = new WindowManager(this, pluginInterface, Configuration);
        }
        
        [Command(Constants.Commands.BASE_COMMAND)]
        // [Aliases("/betterplaytime")]
        [HelpMessage("Opens Auto-Mammet Emote Generator\nOptional Arguments:\nconfig - Opens Config Window")]
        public void PluginCommand(string command, string args)
        {
            switch (args)
            {
                case Constants.Commands.CONFIG_ARG:
                    windowManager.DrawConfigWindow();
                    break;
                default:
                    windowManager.DrawMainWindow();
                    break;
            }
        }

        public void Dispose()
        {
            pluginCommandManager.Dispose();
            windowManager.Dispose();
        }
    }
}
