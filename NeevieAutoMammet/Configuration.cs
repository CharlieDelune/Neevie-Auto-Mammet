using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Plugin;
using NeevieAutoMammet.Constants;
using NeevieAutoMammet.Constants.Time;

namespace NeevieAutoMammet;

[Serializable]
public class Configuration : IPluginConfiguration
{
	private DalamudPluginInterface _pluginInterface;
	public bool AddSoundEffect = true;
	public ChatType DefaultChatType = ChatType.Emote;
	public string MammetName = "The mammet";
	public Sound SoundEffect = Sound.Sound16;
	public Numbers StaticHour = Numbers.THREE;
	public Numbers StaticMinute = Numbers.ZERO;
	public TimesOfDay StaticTimeOfDay = TimesOfDay.PM;
	public bool UseStaticTime = true;
	public List<string> Additions { get; set; } = new();
	public int Version { get; set; } = 0;

	public void Initialize(DalamudPluginInterface pluginInterface)
	{
		_pluginInterface = pluginInterface;
	}

	public Configuration MakeCopy()
	{
		return new Configuration
		{
			Additions = Additions,
			AddSoundEffect = AddSoundEffect,
			DefaultChatType = DefaultChatType,
			MammetName = MammetName,
			SoundEffect = SoundEffect,
			StaticHour = StaticHour,
			StaticMinute = StaticMinute,
			StaticTimeOfDay = StaticTimeOfDay,
			UseStaticTime = UseStaticTime
		};
	}

	public void Save()
	{
		_pluginInterface!.SavePluginConfig(this);
	}

	public void SaveCopy(Configuration copy)
	{
		Additions = copy.Additions;
		AddSoundEffect = copy.AddSoundEffect;
		DefaultChatType = copy.DefaultChatType;
		MammetName = copy.MammetName;
		SoundEffect = copy.SoundEffect;
		StaticHour = copy.StaticHour;
		StaticMinute = copy.StaticMinute;
		StaticTimeOfDay = copy.StaticTimeOfDay;
		UseStaticTime = copy.UseStaticTime;
	}
}