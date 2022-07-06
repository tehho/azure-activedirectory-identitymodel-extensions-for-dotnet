using System;
using System.Collections.Generic;

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// StringSegment represents a sub-region of a string. Instead of creating an instance of a sub-string , it simply uses _start and _end to track the region.
    /// This speeds up string operations without calling the Split() or SubString(), and reduces objects creations.
    /// </summary>
    internal struct StringSegment : IEquatable<StringSegment>
    {
        private string _source;
        private int _start;
        private int _end;

        public StringSegment(string source, bool trim = false) : this(source, 0, source.Length, trim) {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">The target string.</param>
        /// <param name="start">The start of the string segment.</param>
        /// <param name="end">The end (inclusive) of the string segment.</param>
        /// <param name="trim">Whether to trim all leading and trailing white spaces.</param>
        public StringSegment(string source, int start, int end, bool trim = false)
        {
            //if (start < 0 || start > source.Length) throw new ArgumentOutOfRangeException(nameof(start));
            // if (end < 0 || end > source.Length) throw new ArgumentOutOfRangeException(nameof(end));
            //// if (start > end) throw new ArgumentOutOfRangeException(nameof(end), "start cannot be greater than end + 1");

            _source = source ?? throw new ArgumentNullException(nameof(source));
            _start = start;
            _end = end;

            if (trim)
                Trim();
        }

        public char this[int index] => _source[_start + index];

        public string Source => _source;

        public int StartIndex => _start;

        public int EndIndex => _end;

        /// <summary>
        /// Trim all leading and trailing white spaces.
        /// </summary>
        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        /// <summary>
        /// Trim all leading spaces by moving the _start index to the next non-whitespace character.
        /// </summary>
        public void TrimStart()
        {
            while (_start < _end && char.IsWhiteSpace(_source[_start]))
                _start++;
        }

        /// <summary>
        /// Trim (Skip) all trailing spaces by moving the _end index to the previous non-whitespace character.
        /// </summary>
        public void TrimEnd()
        {
            while (_start < _end && char.IsWhiteSpace(_source[_end - 1]))
                _end--;
        }

        /// <summary>
        /// Trim (skip) all the specified characters from the head and tail.
        /// A usage is to trim the " char at the start and end of a string.
        /// </summary>
        /// <param name="c">The character to be trimmed</param>
        /// <returns>This instance with all specified char trimmed.</returns>
        public void Trim(char c)
        {
            TrimStart(c);
            TrimEnd(c);
        }

        /// <summary>
        /// Trim (skip) all the specified characters from the head .
        /// For example, trim the " char at the start of a string.
        /// </summary>
        /// <param name="c">The character to be trimmed</param>
        /// <returns>This instance with all specified char trimmed.</returns>
        public void TrimStart(char c)
        {
            while (_start < _end && _source[_start] == c)
                _start++;
        }

        /// <summary>
        /// Trim (skip) all the specified characters from the head .
        /// For example, trim the " char at the start of a string.
        /// </summary>
        /// <param name="c">The character to be trimmed</param>
        /// <returns>This instance with all specified char trimmed.</returns>
        public void TrimEnd(char c)
        {
            while (_start < _end && _source[_end - 1] == c)
                _end--;
        }

        /// <summary>
        /// Check if the beginning of this instance matches the specified string.
        /// </summary>
        /// <param name="s">The string to compare.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
        /// <returns>true if value matches the beginning of this instance; otherwise, false.</returns>
        public bool StartsWith(string s, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return s != null && Length >= s.Length && string.Compare(_source, _start, s, 0, s.Length, comparisonType) == 0;
        }

        /// <summary>
        /// Move the starting point (_start) forward by the specified number of chars.
        /// Note: _start can be _end + 1, which indicates empty string.
        /// </summary>
        /// <param name="offset">The number of characters to move forward by.</param>
        /// <returns>The actual number of moves made.</returns>
        public int Advance(int offset) => _start = Math.Min(_start + offset, _end);

        /// <summary>
        /// Creates a string.
        /// </summary>
        /// <returns>The string starting at a _start and continues to the _end.</returns>
        public override string ToString() => _source.Substring(_start, Length);

        /// <summary>
        /// The number of characters in this instance.
        /// </summary>
        public int Length => _end - _start;

        /// <summary>
        ///  Indicates whether this instance is an empty string.
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Splits this instance into a number of instances based on the specified character.
        /// </summary>
        /// <param name="c">The delimiter character.</param>
        /// <param name="option">None to include empty array elements, RemoveEmptyEntries to omit empty array elements from the array returned.</param>
        /// <param name="trim">Whether to trim leading and trailing spaces.</param>
        /// <returns></returns>
        public IList<StringSegment> Split(char c, StringSplitOptions option = StringSplitOptions.None, bool trim = false)
        {
            var result = new List<StringSegment>();
            int start = _start;
            StringSegment newSegment;
            while (start <= _end)
            {
                int index = _source.IndexOf(c, start);
                if (index == -1 || index >= _end)
                {
                    newSegment = new StringSegment(_source, start, _end, trim);
                    if (option == StringSplitOptions.None || !newSegment.IsEmpty)
                    {
                        result.Add(newSegment);
                    }
                    break;
                }

                newSegment = new StringSegment(_source, start, index, trim);
                if (option == StringSplitOptions.None || !newSegment.IsEmpty)
                {
                    result.Add(newSegment); // index - 1 to exclude the _source[index]
                }

                start = index + 1;
            }

            return result;
        }

        #region not used but best practices

        /// <summary>
        ///  Determines whether two instances have the same value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(StringSegment other) => this == other;

        /// <summary>
        ///  Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The other instance to check.</param>
        /// <returns>true if both are the same instance, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is StringSegment s)
                return this == s;

            return false;
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => _source.GetHashCode() ^ _start.GetHashCode() ^ _end.GetHashCode();

        /// <summary>
        ///  Determines two instances have the same value.
        /// </summary>
        /// <param name="first">The first string to compare.</param>
        /// <param name="second">The second string to compare</param>
        /// <returns></returns>
        public static bool operator ==(StringSegment first, StringSegment second) => first._start == second._start && first._end == second._end && first._source == second._source;

        /// <summary>
        ///  Determines two instances have the different values.
        /// </summary>
        /// <param name="first">The first string to compare.</param>
        /// <param name="second">The second string to compare</param>
        /// <returns></returns>
        public static bool operator !=(StringSegment first, StringSegment second) => !(first == second);

        #endregion
    }
}
