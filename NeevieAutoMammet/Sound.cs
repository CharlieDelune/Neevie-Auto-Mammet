namespace NeevieAutoMammet;

public enum Sound : byte
{
	None = 0x00,
	Unknown = 0x01,
	Sound01 = 0x25,
	Sound02 = 0x26,
	Sound03 = 0x27,
	Sound04 = 0x28,
	Sound05 = 0x29,
	Sound06 = 0x2A,
	Sound07 = 0x2B,
	Sound08 = 0x2C,
	Sound09 = 0x2D,
	Sound10 = 0x2E,
	Sound11 = 0x2F,
	Sound12 = 0x30,
	Sound13 = 0x31,
	Sound14 = 0x32,
	Sound15 = 0x33,
	Sound16 = 0x34
}

public static class SoundsExtensions
{
	public static Sound FromIdx(int idx)
	{
		return idx switch
		{
			0 => Sound.None,
			1 => Sound.Sound01,
			2 => Sound.Sound02,
			3 => Sound.Sound03,
			4 => Sound.Sound04,
			5 => Sound.Sound05,
			6 => Sound.Sound06,
			7 => Sound.Sound07,
			8 => Sound.Sound08,
			9 => Sound.Sound09,
			10 => Sound.Sound10,
			11 => Sound.Sound11,
			12 => Sound.Sound12,
			13 => Sound.Sound13,
			14 => Sound.Sound14,
			15 => Sound.Sound15,
			16 => Sound.Sound16,
			_ => Sound.Unknown
		};
	}

	public static string ToChatSound(this Sound value)
	{
		return value switch
		{
			Sound.None => "",
			Sound.Sound01 => "<se.1>",
			Sound.Sound02 => "<se.2>",
			Sound.Sound03 => "<se.3>",
			Sound.Sound04 => "<se.4>",
			Sound.Sound05 => "<se.5>",
			Sound.Sound06 => "<se.6>",
			Sound.Sound07 => "<se.7>",
			Sound.Sound08 => "<se.8>",
			Sound.Sound09 => "<se.9>",
			Sound.Sound10 => "<se.10>",
			Sound.Sound11 => "<se.11>",
			Sound.Sound12 => "<se.12>",
			Sound.Sound13 => "<se.13>",
			Sound.Sound14 => "<se.14>",
			Sound.Sound15 => "<se.15>",
			Sound.Sound16 => "<se.16>",
			_ => ""
		};
	}

	public static int ToIdx(this Sound value)
	{
		return value switch
		{
			Sound.None => 0,
			Sound.Sound01 => 1,
			Sound.Sound02 => 2,
			Sound.Sound03 => 3,
			Sound.Sound04 => 4,
			Sound.Sound05 => 5,
			Sound.Sound06 => 6,
			Sound.Sound07 => 7,
			Sound.Sound08 => 8,
			Sound.Sound09 => 9,
			Sound.Sound10 => 10,
			Sound.Sound11 => 11,
			Sound.Sound12 => 12,
			Sound.Sound13 => 13,
			Sound.Sound14 => 14,
			Sound.Sound15 => 15,
			Sound.Sound16 => 16,
			_ => -1
		};
	}

	public static string ToName(this Sound value)
	{
		return value switch
		{
			Sound.None => "None",
			Sound.Sound01 => "Sound Effect 1",
			Sound.Sound02 => "Sound Effect 2",
			Sound.Sound03 => "Sound Effect 3",
			Sound.Sound04 => "Sound Effect 4",
			Sound.Sound05 => "Sound Effect 5",
			Sound.Sound06 => "Sound Effect 6",
			Sound.Sound07 => "Sound Effect 7",
			Sound.Sound08 => "Sound Effect 8",
			Sound.Sound09 => "Sound Effect 9",
			Sound.Sound10 => "Sound Effect 10",
			Sound.Sound11 => "Sound Effect 11",
			Sound.Sound12 => "Sound Effect 12",
			Sound.Sound13 => "Sound Effect 13",
			Sound.Sound14 => "Sound Effect 14",
			Sound.Sound15 => "Sound Effect 15",
			Sound.Sound16 => "Sound Effect 16",
			_ => "Unknown"
		};
	}
}