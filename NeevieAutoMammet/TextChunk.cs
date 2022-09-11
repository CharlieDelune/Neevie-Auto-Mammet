using System.Linq;

namespace NeevieAutoMammet;

internal class TextChunk
{
    /// <summary>
    ///     The continuation marker to append to the end of the Complete Text value.
    /// </summary>
    internal string ContinuationMarker = "";
    /// <summary>
    ///     Chat header to put at the beginning of each chunk when copied.
    /// </summary>
    internal string Header = "";

    /// <summary>
    ///     The index where this chunk starts within the original text.
    /// </summary>
    internal int StartIndex = -1;

    /// <summary>
    ///     Original text.
    /// </summary>
    internal string Text = "";

    /// <summary>
    ///     Default constructor.
    /// </summary>
    /// <param name="text">The text that forms the chunk.</param>
    internal TextChunk(string text)
	{
		Text = text;
	}

    /// <summary>
    ///     Text split into words.
    /// </summary>
    internal Word[] Words => Text.Words();

    /// <summary>
    ///     The number of words in the text chunk.
    /// </summary>
    internal int WordCount => Words.Count();

    /// <summary>
    ///     Assembles the complete chunk with header, continuation markers, and user-defined text.
    /// </summary>
    internal string CompleteText =>
		$"{(Header.Length > 0 ? $"{Header} " : "")}{Text.Trim()}{(ContinuationMarker.Length > 0 ? $" {ContinuationMarker}" : "")}";

	public override string ToString()
	{
		return
			$"{{ StartIndex: {StartIndex}, Header: \"{Header}\", Text: \"{Text}\", Words: {Words}, Marker: {ContinuationMarker}}}";
	}
}