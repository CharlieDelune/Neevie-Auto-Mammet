using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using ImGuiNET;
using NeevieAutoMammet.Constants;
using NeevieAutoMammet.Constants.Time;

namespace NeevieAutoMammet.Windows;

public class MainWindow : Window, IDisposable
{
	private readonly Configuration _configuration;

	private readonly ChatType[] _validChatTypes =
		((ChatType[])Enum.GetValues(typeof(ChatType))).ToArray();
	private readonly WindowManager _windowManager;
	private ChatType _chatType = ChatType.Emote;
	private List<TextChunk> _chunks = new();
	private readonly bool _crossWorld = false;
	private readonly int _linkshell = 0;
	private int _nextChunk;

	private string _previousBuiltString;
	private string _telltarget = "";

	public MainWindow(Configuration configuration, WindowManager windowManager) : base(
		Constants.Windows.MAIN, ImGuiWindowFlags.MenuBar)
	{
		SizeConstraints = new WindowSizeConstraints
		{
			MinimumSize = new Vector2(375, 330),
			MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
		};

		_configuration = configuration;
		_windowManager = windowManager;
		_previousBuiltString = string.Empty;
	}

	public void Dispose()
	{
	}

	public override void Draw()
	{
		DrawMenu();
		DrawChatTypeChooser();
		DrawChunkDisplay();
	}

	public void GenerateEmote()
	{
		string addition = $"{Global.SPEAK_ON_ERROR}";
		if (_configuration.Additions.Any())
		{
			int rand = new Random().Next(0, _configuration.Additions.Count);
			addition = $"{Global.SPEAK_ON_SUCCESS} \"{_configuration.Additions[rand]}\"";
		}

		string soundEffect = _configuration.AddSoundEffect ? _configuration.SoundEffect.ToChatSound() : "";
		_previousBuiltString =
			$"{Global.EMOTE_CHIMES} {_configuration.MammetName} {Global.EMOTE_ANNOUNCE} {GenerateTime()}{Global.EMOTE_COMPLIMENT} {addition} {soundEffect}";
		GenerateChunks();
	}

	public float GetFooterHeight()
	{
		float result = 70;
		return result * ImGuiHelpers.GlobalScale;
	}

	public void ResetChatType()
	{
		_chatType = _configuration.DefaultChatType;
		_telltarget = string.Empty;
	}

	public override void Update()
	{
		GenerateChunks();
	}

	private void DrawChatTypeChooser()
	{
		ImGui.Text("Channel:");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(150);
		if (ImGui.BeginCombo("##NeevieChatType", _chatType.ToString()))
		{
			for (int i = 0; i < _validChatTypes.Length; i++)
			{
				ChatType ct = _validChatTypes[i];
				if (!ImGui.Selectable($"{ct.ToString()}##NeevieChatTypeOption{i}"))
				{
					continue;
				}

				_chatType = ct;
			}

			ImGui.EndCombo();
		}

		if (_chatType != ChatType.Tell)
		{
			return;
		}

		ImGui.SameLine();
		ImGui.InputTextWithHint("##NeevieTellTarget", "User Name@World", ref _telltarget, 128);
	}

	private void DrawChunkDisplay()
	{
		ImGui.Text("Your generated emote:");
		// Draw the chunk display
		if (ImGui.BeginChild("##NeevieChunkChild", new Vector2(-1, (Size?.Y ?? 25) - GetFooterHeight())))
		{
			for (int i = 0; i < _chunks.Count; ++i)
			{
				if (i > 0)
				{
					ImGui.Spacing();
				}

				ImGui.Separator();

				ImGui.SetNextItemWidth(-1);
				ImGui.TextWrapped(_chunks[i].CompleteText);
			}

			ImGui.EndChild();
		}

		ImGui.Separator();
		ImGui.Spacing();
		if (ImGui.Button(
			    $"Copy{(_chunks.Count > 1 ? $" ({_nextChunk + 1}/{_chunks.Count})" : "")}##NeevieCopyButton"))
		{
			CopyToClipboard();
		}

		ImGui.SameLine();
		if (ImGui.Button("Generate Again!##NeevieGenerateButton"))
		{
			GenerateEmote();
		}

		ImGui.SameLine();
		if (ImGui.Button("Close##NeevieCloseButton"))
		{
			IsOpen = false;
		}
	}

	private void DrawMenu()
	{
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.MenuItem("Settings##NeevieSettingsMenu"))
			{
				_windowManager.DrawConfigWindow();
			}

			ImGui.EndMenuBar();
		}
	}

	private void GenerateChunks()
	{
		_chunks = ChatHelper.FFXIVify(ChatHelper.GetFullChatHeader(_chatType, _telltarget, _crossWorld, _linkshell),
			_previousBuiltString);
	}

	private string GenerateTime()
	{
		Numbers hour = _configuration.StaticHour;
		Numbers minute = _configuration.StaticMinute;
		TimesOfDay timeOfDay = _configuration.StaticTimeOfDay;
		if (!_configuration.UseStaticTime)
		{
			long serverLong = Framework.GetServerTime();
			DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(serverLong).LocalDateTime;
			hour = (Numbers)dateTime.Hour;
			if (hour == Numbers.ZERO)
			{
				hour = Numbers.TWELVE;
			}

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

	protected void CopyToClipboard()
	{
		try
		{
			// If there are no chunks to copy exit the function.
			if (_chunks.Count == 0)
			{
				return;
			}

			// Copy the next chunk over.
			ImGui.SetClipboardText(_chunks[_nextChunk++].CompleteText.Trim());

			// If we're not at the last chunk, return.
			if (_nextChunk < _chunks.Count)
			{
				return;
			}

			// After this point, we assume we've copied the last chunk.
			_nextChunk = 0;
		}
		catch (Exception e)
		{
			_windowManager.DrawErrorWindow(e.Message);
		}
	}
}