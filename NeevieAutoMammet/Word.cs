namespace NeevieAutoMammet;

internal sealed class Word
{
    /// <summary>
    ///     The last index of the text. This can be offset from
    ///     StartIndex+WordLength if there is punctuation in the
    ///     text.
    /// </summary>
    internal int EndIndex = -1;

    /// <summary>
    ///     A value indicating whether or not the word is hyphen-terminated.
    /// </summary>
    internal bool HyphenTerminated = false;
    /// <summary>
    ///     The start of the entire text segment.
    /// </summary>
    internal int StartIndex = -1;

    /// <summary>
    ///     The index where the word starts. This can be different
    ///     from StartIndex when the text starts with punctuation.
    /// </summary>
    internal int WordIndex = -1;

    /// <summary>
    ///     The length of the word excluding any punctuation marks.
    /// </summary>
    internal int WordLength = -1;

	internal int WordEndIndex => WordIndex + WordLength;

	public string GetString(string s)
	{
		return GetString(s, 0);
	}

	public string GetString(string s, int offset)
	{
		return StartIndex + offset >= 0 && StartIndex < EndIndex && EndIndex + offset <= s.Unwrap().Length
			? s.Unwrap()[(StartIndex + offset)..(EndIndex + offset)]
			: "";
	}

	public string GetWordString(string s)
	{
		return GetWordString(s, 0);
	}

	public string GetWordString(string s, int offset)
	{
		return WordIndex + offset >= 0 && WordLength > 0 && WordIndex + WordLength + offset <= s.Unwrap().Length
			? s.Unwrap()[(WordIndex + offset)..(WordIndex + WordLength + offset)]
			: "";
	}

	public void Offset(int value)
	{
		StartIndex += value;
		WordIndex += value;
		EndIndex += value;
	}
}