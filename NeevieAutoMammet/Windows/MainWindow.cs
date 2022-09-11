using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using Microsoft.VisualBasic;
using NeevieAutoMammet.Constants;
using NeevieAutoMammet.Constants.Time;

namespace NeevieAutoMammet.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private readonly WindowManager windowManager;
    private List<TextChunk> _chunks = new();
    
    internal ChatType _chatType = ChatType.Emote;
    protected string _telltarget = "";
    protected int _linkshell = 0;
    protected bool _crossWorld = false;
    
    
    protected int _nextChunk = 0;

    public MainWindow(Configuration configuration, WindowManager windowManager) : base(
        Constants.Windows.MAIN, ImGuiWindowFlags.MenuBar)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.configuration = configuration;
        this.windowManager = windowManager;
    }

    public void Dispose()
    {
    }

    public void GenerateEmote()
    {
        string addition = $"{Global.SPEAK_ON_ERROR}";
        if (configuration.Additions.Any())
        {
            int rand = new Random().Next(0, configuration.Additions.Count);
            addition = $"{Global.SPEAK_ON_SUCCESS} \"{configuration.Additions[rand]}\"";
        }
        string soundEffect = configuration.AddSoundEffect ? configuration.SoundEffect.ToChatSound() : "";
        string builtString = $"{Global.EMOTE_CHIMES} {configuration.MammetName} {Global.EMOTE_ANNOUNCE} {GenerateTime()}{Global.EMOTE_COMPLIMENT} {addition} {soundEffect}";
        this._chunks = ChatHelper.FFXIVify( GetFullChatHeader(), builtString);
    }

    private string GenerateTime()
    {
        Numbers hour = configuration.StaticHour;
        Numbers minute = configuration.StaticMinute;
        TimesOfDay timeOfDay = configuration.StaticTimeOfDay;
        if (!configuration.UseStaticTime)
        {
            var serverLong = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.GetServerTime();
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(serverLong).LocalDateTime;
            hour = (Numbers)dateTime.Hour;
            if (hour == Numbers.ZERO) hour = Numbers.TWELVE;
            minute = (Numbers)dateTime.Minute;
            timeOfDay = Enum.Parse<TimesOfDay>(dateTime.ToString("tt"));
        }

        string hourString = hour.ToString();
        string minuteString = minute.ToString().Replace('_', '-');
        string ohString = (int)minute < 10 ? "oh-" : "";
        minuteString = (int)minute == 0 ? string.Empty : $"{ohString}{minuteString} ";
        string todString = timeOfDay == TimesOfDay.AM ? "morning" : (int)hour < 7 ? "afternoon" : "evening";
        string compiledString = $"{hourString} {minuteString}in the {todString}";
        return new string(compiledString.Select((c, i) => i % 2 == 1 ? char.ToUpper(c) : char.ToLower(c)).ToArray());
    }

    public override void Draw()
    {
        DrawMenu();
        ImGui.Text("Your generated emote:");
        DrawChunkDisplay();
    }
    
    private void DrawChunkDisplay()
    {
        // Draw the chunk display
        if (ImGui.BeginChild($"Neevie##ScratchPadChildFrame", new(-1, (this.Size?.Y ?? 25) - GetFooterHeight())))
        {
            for (int i = 0; i < this._chunks.Count; ++i)
            {
                //// If not the first chunk, add a spacing.
                if ( i > 0 )
                    ImGui.Spacing();

                // Put a separator at the top of the chunk.
                ImGui.Separator();

                // Set width and display the chunk.
                ImGui.SetNextItemWidth( -1 );
                ImGui.TextWrapped( this._chunks[i].CompleteText );
            }
                
            /*if ( this._textchanged )
            {
                ImGui.SetScrollHereY();
                this._textchanged = false;
            }*/
            ImGui.EndChild();
        }
        ImGui.Separator();
        ImGui.Spacing();
        if (ImGui.Button(
                $"Copy{(this._chunks.Count > 1 ? $" ({this._nextChunk + 1}/{this._chunks.Count})" : "")}##NeevieCopy"))
        {
            DoCopyToClipboard();
        }
        ImGui.SameLine();
        if (ImGui.Button("Generate Again!##NeevieGenerate"))
        {
            GenerateEmote();
        }

    }

    public float GetFooterHeight()
    {
        float result = 70;
        return result * ImGuiHelpers.GlobalScale;
    }
    
    private void DrawMenu()
    {
        if (ImGui.BeginMenuBar()) {
            if (ImGui.MenuItem($"Settings##NeevieAutoMammetSettingsMenu"))
            {
                windowManager.DrawConfigWindow();
            }
            ImGui.EndMenuBar();
        }
    }
    
    /// <summary>
    /// Gets the slash command (if one exists) and the tell target if one is needed.
    /// </summary>
    internal string GetFullChatHeader() => GetFullChatHeader(this._chatType, this._telltarget, this._crossWorld, this._linkshell);
    internal string GetFullChatHeader(ChatType c, string t, bool cw, int l )
    {
        if ( c == ChatType.None )
            return c.GetShortHeader();

        // Get the slash command.
        string result = c.GetShortHeader();

        // If /tell get the target or placeholder.
        if ( c == ChatType.Tell )
            result += $" {t} ";

        // Grab the linkshell command.
        if ( c == ChatType.Linkshell )
            result = $"/{(cw ? "cw" : "")}linkshell{l + 1}";

        return result;
    }
    
    /// <summary>
    /// Gets the next chunk of text and copies it to the player's clipboard.
    /// </summary>
    protected void DoCopyToClipboard()
    {
        try
        {
            // If there are no chunks to copy exit the function.
            if ( this._chunks.Count == 0 )
                return;

            // Copy the next chunk over.
            ImGui.SetClipboardText( this._chunks[this._nextChunk++].CompleteText.Trim() );

            // If we're not at the last chunk, return.
            if ( this._nextChunk < this._chunks.Count )
                return;

            // After this point, we assume we've copied the last chunk.
            this._nextChunk = 0;
        }
        catch ( Exception e )
        {
            windowManager.DrawErrorWindow(e.Message);
        }
    }
}
