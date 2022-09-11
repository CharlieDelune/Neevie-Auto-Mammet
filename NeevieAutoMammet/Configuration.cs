using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using NeevieAutoMammet.Constants.Time;

namespace NeevieAutoMammet
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public string MammetName = "The mammet";
        public bool UseStaticTime = true;
        public Numbers StaticHour = Numbers.THREE;
        public Numbers StaticMinute = Numbers.ZERO;
        public TimesOfDay StaticTimeOfDay = TimesOfDay.PM;
        public List<string> Additions { get; set; } = new();
        public bool AddSoundEffect = true;
        public Sound SoundEffect = Sound.Sound16;
        
        private DalamudPluginInterface _pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
        }

        public void Save()
        {
            _pluginInterface!.SavePluginConfig(this);
        }

        public Configuration MakeCopy()
        {
            return new Configuration()
            {
                Additions = Additions,
                AddSoundEffect = AddSoundEffect,
                MammetName = MammetName,
                SoundEffect = SoundEffect,
                StaticHour = StaticHour,
                StaticMinute = StaticMinute,
                StaticTimeOfDay = StaticTimeOfDay,
                UseStaticTime = UseStaticTime
            };
        }

        public void SaveCopy(Configuration copy)
        {
            Additions = copy.Additions;
            AddSoundEffect = copy.AddSoundEffect;
            MammetName = copy.MammetName;
            SoundEffect = copy.SoundEffect;
            StaticHour = copy.StaticHour;
            StaticMinute = copy.StaticMinute;
            StaticTimeOfDay = copy.StaticTimeOfDay;
            UseStaticTime = copy.UseStaticTime;
        }
    }
}
