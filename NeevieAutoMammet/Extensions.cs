using System;
using System.Collections.Generic;

namespace NeevieAutoMammet;

internal static class Extensions
{
    /// <summary>
    /// Unwraps the text string using the spaced and no space markers.
    /// </summary>
    /// <param name="s">The <see cref="string"/> to be unwrapped.</param>
    /// <returns><see cref="string"/> with all wrap markers replaced.</returns>
    internal static string Unwrap( this string s ) => s.Trim().Replace( Constants.Words.SPACED_WRAP_MARKER + "\n", " " ).Replace( Constants.Words.NOSPACE_WRAP_MARKER + "\n", "" );

    /// <summary>
    /// Takes a string and attempts to collect all of the words inside of it.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns><see cref="Word"/> array containing all words in the string.</returns>
    internal static Word[] Words( this string s )
    {
        s = s.Unwrap();
        if ( s.Length == 0 )
            return Array.Empty<Word>();

        List<Word> words = new();

        int start = 0;
        int len = 1;
        while ( start + len <= s.Length )
        {
            // Scoot the starting point until we've skipped all spaces, return carriage, and newline characters.
            while ( start < s.Length && " \r\n".Contains( s[start] ) )
                ++start;

            // If the start has gone all the way to tend, leave the loop.
            if ( start == s.Length )
                break;

            if ( start + len == s.Length || " \r\n".Contains( s[start + len] ) )
            {
                int wordoffset = 0;
                int wordlenoffset = 0;
                // If the word starting index is a punctuation character then we scoot the word offset forward up to the entire
                // length of the current string.
                while ( start + wordoffset < s.Length && Constants.Words.PUNCTUATION_CLEANING_LIST.Contains( s[start + wordoffset] ) && wordoffset <= len )
                    ++wordoffset;

                // Default to false hyphen termination.
                bool hyphen = false;
                // If the word ends with a punctuation character then we scoot the word offset left up to the point that
                // the offset puts us at -1 word length. This will happen when the word has no letters.
                while ( start + len - wordlenoffset - 1 > -1 && Constants.Words.PUNCTUATION_CLEANING_LIST.Contains( s[start + len - wordlenoffset - 1] ) && wordoffset <= len )
                {
                    // If the character is a hyphen, flag it as true.
                    if ( s[start + len - wordlenoffset - 1] == '-' )
                        hyphen = true;

                    // If the character is not a hyphen, flat it as false.
                    else
                        hyphen = false;

                    ++wordlenoffset;
                }

                // Add the start offset to the len offset to account for the lost length at the start.
                wordlenoffset += wordoffset;

                // When we create the word we add the offset to ensure that we
                // adjust the position as needed.
                Word w = new()
                {
                    StartIndex = start,
                    EndIndex = start + len,
                    WordIndex = start+wordoffset,
                    WordLength = len-wordlenoffset,
                    HyphenTerminated = hyphen
                };

                words.Add( w );
                start += len;
                len = 1;

                continue;
            }
            ++len;
        }
        return words.ToArray();
    }
    
    /// <summary>
    /// Removes all double spaces from a string.
    /// </summary>
    /// <param name="s">The string to remove double spaces from.</param>
    /// <returns><see cref="string"/> with double-spacing fixed.</returns>
    internal static string FixSpacing( this string s )
    {
        // Start by initially running the replace command.
        do
        {
            // Replace double spaces.
            s = s.Replace( "  ", " " );

            // Loop because 3 spaces together will only get knocked down
            // to 2 spaces and it won't check again so we need to. With
            // each pass, any area with more than one space will become
            // less spaced until only one remains.
        } while ( s.Contains( "  " ) );

        // Return the correctly spaced string.
        return s;
    }
    
    /// <summary>
    /// Removes all double spaces from a string.
    /// </summary>
    /// <param name="s">The string to remove double spaces from.</param>
    /// <param name="cursorPos">A reference to a text cursor to be manipulated.</param>
    /// <returns><see cref="string"/> with double-spacing fixed.</returns>
    internal static string FixSpacing( this string s, ref int cursorPos )
    {
        int idx;
        do
        {
            // Get the position of the first double space.
            idx = s.IndexOf( "  " );
            if ( idx == cursorPos - 1 )
            {
                idx = s[cursorPos..^0].IndexOf( "  " );
                if ( idx > -1 )
                    idx += cursorPos;
            }

            // If the index is greater than -1;
            if ( idx > -1 )
            {
                // If the index is 0 just remove the space from the front of the line.
                if ( idx == 0 )
                    s = s[1..^0];

                // Remove the space from inside the string.
                else
                    s = s[0..idx] + s[(idx + 1)..^0];

                // If the removed space is at a lower index than the cursor
                // move the cursor back a space to account for the position change.
                if ( idx <= cursorPos )
                    cursorPos -= 1;
            }
        } while ( idx > -1 );

        return s;
    }
}
