using Dalamud.Game;

namespace NeevieAutoMammet.Functions;

public delegate ulong PlaySoundDelegate(Sound id, ulong unk1, ulong unk2);

public sealed class PlaySound : FunctionBase<PlaySoundDelegate>
{
	public PlaySound(SigScanner sigScanner)
		: base(sigScanner, "E8 ?? ?? ?? ?? 4D 39 BE")
	{
	}

	public void Play(Sound id)
	{
		Invoke(id, 0ul, 0ul);
	}
}