using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace NeevieAutoMammet.Windows;

public class ErrorWindow : Window, IDisposable
{
    private string errorString = "";

    public ErrorWindow() : base(
        Constants.Windows.ERROR, ImGuiWindowFlags.NoCollapse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void SetErrorMessage(string error)
    {
        errorString = error;
    }

    public void Dispose()
    {
        errorString = "";
    }

    public override void Draw()
    {
        ImGui.Text("Sorry, Galvjn blew it and something went wrong:");
        ImGui.Spacing();
        ImGui.Text(errorString);
    }
}
