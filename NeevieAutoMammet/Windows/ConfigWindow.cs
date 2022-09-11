using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NeevieAutoMammet.Constants;
using NeevieAutoMammet.Constants.Time;
using NeevieAutoMammet.Functions;

namespace NeevieAutoMammet.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private Configuration localConfig;
    private readonly WindowManager windowManager;
    private readonly PlaySound PlaySound;
    
    private readonly Sound[] _validSounds = 
        ((Sound[])Enum.GetValues(typeof(Sound))).Where(s => s != Sound.None && s != Sound.Unknown).ToArray();
    private readonly Numbers[] _validHours = 
        ((Numbers[])Enum.GetValues(typeof(Numbers))).Where(n => (int)n > 0 && (int)n < 13).ToArray();
    private readonly Numbers[] _validMinutes = 
        ((Numbers[])Enum.GetValues(typeof(Numbers))).ToArray();
    private readonly TimesOfDay[] _validTimesOfDay = 
        ((TimesOfDay[])Enum.GetValues(typeof(TimesOfDay))).ToArray();

    private float _lastWidth = 0;
    private bool _ignoreTextEdit = false;

    public ConfigWindow(Configuration configuration, WindowManager windowManager) : base(
        Constants.Windows.CONFIG, ImGuiWindowFlags.NoCollapse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 400),
            MaximumSize = new Vector2(1600, 900)
        };

        this.configuration = configuration;
        this.windowManager = windowManager;
        PlaySound = new PlaySound(new SigScanner());
        localConfig = configuration.MakeCopy();
    }

    public void Dispose()
    {
        localConfig = configuration;
    }

    public override void Draw()
    {
        DrawInput();
        DrawTime();
        DrawAudio();
        DrawAdditions();
        DrawFooter();
    }

    private void DrawInput()
    {
        ImGui.Text("Mammet Name:");
        ImGui.InputText("##NeevieMammetNameInput", ref localConfig.MammetName, 200);
    }

    private unsafe void DrawAdditions()
    {
        ImGui.Text("Additions");
        ImGui.BeginChild("##NeevieChildAdditions", 
                         new Vector2(-1, (this.Size?.Y ?? 25) - GetFooterHeight() - 20));
        for (int i = 0; i < localConfig.Additions.Count; i++)
        {
            var input = localConfig.Additions[i];
            ImGui.InputTextMultiline($"##additions{i}", ref input, 2000, 
                                     new Vector2(ImGui.GetWindowContentRegionMax().X - 50, 50), 
                                     ImGuiInputTextFlags.CallbackEdit |
                                     ImGuiInputTextFlags.NoHorizontalScroll, 
                                     tcbd => OnTextEdit(tcbd, i));
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconString()}##{i}"))
            {
                localConfig.Additions.RemoveAt(i);
            }
            ImGui.PopFont();
        }
        ImGui.EndChild();
        
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}"))
        {
            localConfig.Additions.Add("");
        }
        ImGui.PopFont();
        ImGui.Separator();
    }

    private unsafe int OnTextEdit(ImGuiInputTextCallbackData* data, int index)
    {
        try
        {
            UTF8Encoding utf8 = new();
            var s = utf8.GetString(data->Buf, data->BufTextLen);

            // For some reason, ImGui's InputText never verifies that BufTextLen never goes negative
            // which can lead to some serious problems and crashes with trying to get the string.
            // Here we do the check ourself with the turnery operator. If it does happen to be
            // a negative number, return a blank string so the rest of the code can continue as normal
            // at which point the buffer will be cleared and BufTextLen will be set to 0, preventing any
            // memory damage or crashes.
            string txt = data->BufTextLen >= 0 ? utf8.GetString(data->Buf, data->BufTextLen) : "";
            localConfig.Additions[index] = txt;

            int pos = data->CursorPos;
            
            // Wrap the string if there is enough there.
            /*if ( txt.Length > 0 )
                txt = WrapString( txt, ref pos );*/

            // Convert the string back to bytes.
            byte[] bytes = utf8.GetBytes(txt);

            // Replace with new values.
            for (int i = 0; i < bytes.Length; ++i)
                data->Buf[i] = bytes[i];

            // Terminate the string.
            data->Buf[bytes.Length] = 0;

            // Assign the new buffer text length. This is the
            // number of bytes that make up the text, not the number
            // of characters in the text.
            data->BufTextLen = bytes.Length;

            // Reassing the cursor position to adjust for the change in text lengths.
            data->CursorPos = pos;

            // Flag the buffer as dirty so ImGui will rebuild the buffer
            // and redraw the text in the InputText.
            data->BufDirty = 1;
        }
        catch (Exception e)
        {
            windowManager.DrawErrorWindow(e.Message);
        }

        return 0;
    }
    
    /// <summary>
    /// Takes a string and wraps it based on the current width of the window.
    /// </summary>
    /// <param name="text">The string to be wrapped.</param>
    /// <returns></returns>
    protected string WrapString(string text, ref int cursorPos)
    {
        try
        {
            // If the string is empty then just return it.
            if ( text.Length == 0 )
                return text;

            // Trim any return carriages off the end. This can happen if the user
            // backspaces a new line character off of the end.
            text = text.TrimEnd( '\r' );

            // Replace all wrap markers with spaces and adjust cursor offset. Do this before
            // all non-spaced wrap markers because the Spaced marker contains the nonspaced marker
            while ( text.Contains( Words.SPACED_WRAP_MARKER + '\n' ) )
            {
                int idx = text.IndexOf(Words.SPACED_WRAP_MARKER + '\n');
                text = text[0..idx] + " " + text[(idx + (Words.SPACED_WRAP_MARKER + '\n').Length)..^0];

                // We adjust the cursor position by one less than the wrap marker
                // length to account for the space that replaces it.
                if ( cursorPos > idx )
                    cursorPos -= Words.SPACED_WRAP_MARKER.Length;

            }

            while ( text.Contains( Words.NOSPACE_WRAP_MARKER + '\n' ) )
            {
                int idx = text.IndexOf(Words.NOSPACE_WRAP_MARKER + '\n');
                text = text[0..idx] + text[(idx + (Words.NOSPACE_WRAP_MARKER + '\n').Length)..^0];

                if ( cursorPos > idx )
                    cursorPos -= (Words.NOSPACE_WRAP_MARKER + '\n').Length;
            }
            
            text = text.FixSpacing( ref cursorPos );

            // Get the maximum allowed character width.
            float width = this._lastWidth - (85 * ImGuiHelpers.GlobalScale);

            // Iterate through each character.
            int lastSpace = 0;
            int offset = 0;
            for ( int i = 1; i < text.Length; ++i )
            {
                // If the current character is a space, mark it as a wrap point.
                if ( text[i] == ' ' )
                    lastSpace = i;

                // If the size of the text is wider than the available size
                float txtWidth = ImGui.CalcTextSize(text[offset..i ]).X;
                if ( txtWidth + 10 * ImGuiHelpers.GlobalScale > width )
                {
                    // Replace the last previous space with a new line
                    StringBuilder sb = new(text);

                    if ( lastSpace > offset )
                    {
                        sb.Remove( lastSpace, 1 );
                        sb.Insert( lastSpace, Words.SPACED_WRAP_MARKER + '\n' );
                        offset = lastSpace + Words.SPACED_WRAP_MARKER.Length;
                        i += Words.SPACED_WRAP_MARKER.Length;

                        // Adjust cursor position for the marker but not
                        // the new line as the new line is replacing the space.
                        if ( lastSpace < cursorPos )
                            cursorPos += Words.SPACED_WRAP_MARKER.Length;
                    }
                    else
                    {
                        sb.Insert( i, Words.NOSPACE_WRAP_MARKER + '\n' );
                        offset = i + Words.NOSPACE_WRAP_MARKER.Length;
                        i += Words.NOSPACE_WRAP_MARKER.Length;

                        // Adjust cursor position for the marker and the
                        // new line since both are inserted.
                        if ( cursorPos > i - Words.NOSPACE_WRAP_MARKER.Length )
                            cursorPos += Words.NOSPACE_WRAP_MARKER.Length + 1;
                    }
                    text = sb.ToString();
                }
            }
            return text;
        }
        catch ( Exception e )
        {
            windowManager.DrawErrorWindow(e.Message);
        }
        return "";
    }

    private void SaveConfig(Configuration configCopy)
    {
        configuration.SaveCopy(configCopy);
        configuration.Save();
    }
    
    private void DrawFooter()
    {
        ImGui.Spacing();
        if (ImGui.BeginTable($"NeevieConfigFooterButtonTable", 3))
        {
            // Setup the three columns for the buttons. I use a table here for easy space sharing.
            // The table will handle all sizing and positioning of the buttons automatically with no
            // extra input from me.
            ImGui.TableSetupColumn($"FooterCopyColumn", ImGuiTableColumnFlags.WidthStretch, 1);
            ImGui.TableSetupColumn($"FooterClearButtonColumn", ImGuiTableColumnFlags.WidthStretch, 1);
            ImGui.TableSetupColumn($"FooterSpellCheckButtonColumn", ImGuiTableColumnFlags.WidthStretch, 1);
            ImGui.TableNextColumn();
            if (ImGui.Button("Close"))
            {
                localConfig = configuration.MakeCopy();
                IsOpen = false;
            }
            ImGui.TableNextColumn();
            if (ImGui.Button("Save"))
            {
                SaveConfig(localConfig);
            }
            ImGui.TableNextColumn();
            if (ImGui.Button("Save and Close"))
            {
                SaveConfig(localConfig);
                IsOpen = false;
            }
            ImGui.EndTable();
        }
    }
    
    private void DrawAudio()
    {
        ImGui.Text("Add Sound Effect to Emote:");
        ImGui.Checkbox($"##NeeviePlaySoundCheckbox", ref localConfig.AddSoundEffect);

        if (!localConfig.AddSoundEffect)
            return;

        ImGui.SameLine();

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Play.ToIconChar()}##NeevieSoundTestButton",
                         new Vector2(ImGui.GetItemRectSize().Y)))
        {
            StartSound();
        }
        ImGui.PopFont();
        ImGui.SameLine();
        DrawGameSound();
    }

    
    private void DrawGameSound()
    {
        ImGui.SetNextItemWidth(150);
        if (ImGui.BeginCombo($"##NeevieAlertSound", localConfig.SoundEffect.ToName()))
        {
            for (var i = 0; i < _validSounds.Length; i++)
            {
                var se = _validSounds[i];
                if (!ImGui.Selectable($"{se.ToName()}##AlertSoundsCombo{i}"))
                    continue;

                localConfig.SoundEffect = se;
                PlaySound.Play(se);
            }
            ImGui.EndCombo();
        }
    }
    
    private void DrawTime()
    {
        ImGui.Text("Use Static Time:");
        ImGui.Checkbox($"##NeevieUseStaticTimeCheckbox", ref localConfig.UseStaticTime);

        if (!localConfig.UseStaticTime)
            return;
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        if (ImGui.BeginCombo($"##NeevieHourCombo", ((int)localConfig.StaticHour).ToString()))
        {
            for (var i = 0; i < _validHours.Length; i++)
            {
                var num = _validHours[i];
                if (!ImGui.Selectable($"{(int)num}##NeevieHourCombo{i}"))
                    continue;

                localConfig.StaticHour = num;
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        if (ImGui.BeginCombo($"##NeevieMinuteCombo", ((int)localConfig.StaticMinute).ToString("0#")))
        {
            for (var i = 0; i < _validMinutes.Length; i++)
            {
                var num = _validMinutes[i];
                if (!ImGui.Selectable($"{((int)num).ToString("0#")}##NeevieMinuteCombo{i}"))
                    continue;

                localConfig.StaticMinute = num;
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        if (ImGui.BeginCombo($"##NeevieTimeOfDayCombo", localConfig.StaticTimeOfDay.ToString()))
        {
            for (var i = 0; i < _validTimesOfDay.Length; i++)
            {
                var tod = _validTimesOfDay[i];
                if (!ImGui.Selectable($"{tod.ToString()}##NeevieTimeOfDayCombo{i}"))
                    continue;

                localConfig.StaticTimeOfDay = tod;
            }
            ImGui.EndCombo();
        }
    }
    
    public bool StartSound()
    {
        if (!localConfig.AddSoundEffect)
            return false;

        PlaySound.Play(localConfig.SoundEffect);
        return true;
    }

    private float GetFooterHeight()
    {
        float result = 70;
        return result * ImGuiHelpers.GlobalScale;
    }
}
