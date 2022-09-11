using System;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace NeevieAutoMammet.Windows;

public class WindowManager : IDisposable
{
    private readonly MainWindow mainWindow;
    private readonly ConfigWindow configWindow;
    private readonly ErrorWindow errorWindow;
    private readonly WindowSystem windowSystem = new(Constants.Global.PLUGIN_NAME);
    private readonly DalamudPluginInterface pluginInterface;
    private readonly Configuration configuration;
    
    public WindowManager(NeevieAutoMammet neevieAutoMammet, DalamudPluginInterface pluginInterface, Configuration configuration)
    {
        this.pluginInterface = pluginInterface;
        this.configuration = configuration;

        mainWindow = new MainWindow(this.configuration, this);
        configWindow = new ConfigWindow(this.configuration, this);
        errorWindow = new ErrorWindow();
        
        windowSystem.AddWindow(mainWindow);
        windowSystem.AddWindow(configWindow);
        windowSystem.AddWindow(errorWindow);
        
        this.pluginInterface.UiBuilder.Draw += DrawUI;
    }

    public void DrawErrorWindow(string error)
    {
        errorWindow.SetErrorMessage(error);
        errorWindow.IsOpen = true;
    }
    
    public void DrawConfigWindow()
    {
        configWindow.IsOpen = true;
    }

    public void DrawMainWindow()
    {
        mainWindow.GenerateEmote();
        mainWindow.IsOpen = true;
    }

    public void Dispose()
    {
        windowSystem.RemoveAllWindows();
    }
    
    private void DrawUI()
    {
        windowSystem.Draw();
    }
}
