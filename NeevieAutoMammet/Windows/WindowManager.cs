using System;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using NeevieAutoMammet.Constants;

namespace NeevieAutoMammet.Windows;

public class WindowManager : IDisposable
{
	private readonly Configuration _configuration;
	private readonly ConfigWindow _configWindow;
	private readonly ErrorWindow _errorWindow;
	private readonly MainWindow _mainWindow;
	private readonly DalamudPluginInterface _pluginInterface;
	private readonly WindowSystem _windowSystem = new(Global.PLUGIN_NAME);

	public WindowManager(NeevieAutoMammet neevieAutoMammet, DalamudPluginInterface pluginInterface,
		Configuration configuration)
	{
		_pluginInterface = pluginInterface;
		_configuration = configuration;

		_mainWindow = new MainWindow(_configuration, this);
		_configWindow = new ConfigWindow(_configuration, this);
		_errorWindow = new ErrorWindow();

		_windowSystem.AddWindow(_mainWindow);
		_windowSystem.AddWindow(_configWindow);
		_windowSystem.AddWindow(_errorWindow);

		_pluginInterface.UiBuilder.Draw += DrawUI;
	}

	public void Dispose()
	{
		_windowSystem.RemoveAllWindows();
	}

	public void DrawConfigWindow()
	{
		_configWindow.IsOpen = true;
	}

	public void DrawErrorWindow(string error)
	{
		_errorWindow.SetErrorMessage(error);
		_errorWindow.IsOpen = true;
	}

	public void DrawMainWindow()
	{
		_mainWindow.ResetChatType();
		_mainWindow.GenerateEmote();
		_mainWindow.IsOpen = true;
	}

	private void DrawUI()
	{
		_windowSystem.Draw();
	}
}