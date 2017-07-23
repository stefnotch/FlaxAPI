////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxEditor.Utilities
{
    /// <summary>
    /// Helper class to filter items based on a input filter query.
    /// </summary>
    public static class QueryFilterHelper
    {
        /// <summary>
        /// Matches the specified text with the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="text">The text.</param>
        /// <returns>True if text has one or more matches, otherwise false.</returns>
        public static bool Match(string filter, string text)
        {
            Range[] matches;
            return Match(filter, text, out matches);
        }
        
        /// <summary>
        /// Matches the specified text with the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="text">The text.</param>
        /// <param name="matches">The found matches.</param>
        /// <returns>True if text has one or more matches, otherwise false.</returns>
        public static bool Match(string filter, string text, out Range[] matches)
        {
            // Empty inputs
            matches = null;
            if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(text))
                return false;

            // Full match
            if (string.Equals(filter, text, StringComparison.CurrentCultureIgnoreCase))
            {
                matches = new[] { new Range(0, filter.Length) };
                return true;
            }

            const int MinLength = 2;
            List<Range> ranges = null;

            // Find matching sequences
            // We do simple iteration over the characters
            int textLength = text.Length;
            int filterLength = filter.Length;
            for (int textPos = 0; textPos < textLength; textPos++)
            {
                int matchStartPos = -1;
                int endPos = Mathf.Min(textLength, textPos + filterLength);
                int filterPos = 0;

                for (int i = textPos; i < endPos; i++, filterPos++)
                {
                    var filterChar = char.ToLower(filter[filterPos]);
                    var textChar = char.ToLower(text[i]);
                    
                    if (filterChar == textChar)
                    {
                        // Check if start the matching sequence
                        if (matchStartPos == -1)
                        {
                            if (ranges == null)
                                ranges = new List<Range>();
                            matchStartPos = textPos;
                        }
                    }
                    else
                    {
                        // Check if stop matching sequence
                        if (matchStartPos != -1)
                        {
                            var length = textPos - matchStartPos;
                            if (length >= MinLength)
                                ranges.Add(new Range(matchStartPos, length));
                            textPos = matchStartPos + length;
                            matchStartPos = -1;
                        }
                        break;
                    }
                }

                // Check sequence on the end
                if (matchStartPos != -1 && filterPos == filterLength)
                {
                    var length = endPos - matchStartPos;
                    if (length >= MinLength)
                        ranges.Add(new Range(matchStartPos, length));
                    textPos = matchStartPos + length;
                }
            }

            // Check if has any range
            if (ranges != null && ranges.Count > 0)
            {
                matches = ranges.ToArray();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Describies subrange of the text.
        /// </summary>
        public struct Range
        {
            /// <summary>
            /// The start index of the range.
            /// </summary>
            public readonly int StartIndex;

            /// <summary>
            /// The length.
            /// </summary>
            public readonly int Length;

            /// <summary>
            /// The end index of the range.
            /// </summary>
            public int EndIndex => StartIndex + Length;

            /// <summary>
            /// Initializes a new instance of the <see cref="Range"/> struct.
            /// </summary>
            /// <param name="start">The start.</param>
            /// <param name="length">The length.</param>
            public Range(int start, int length)
            {
                StartIndex = start;
                Length = length;
            }
            
            /// <summary>
            /// Tests for equality between two objects.
            /// </summary>
            /// <param name="left">The first value to compare.</param>
            /// <param name="right">The second value to compare.</param>
            /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
            public static bool operator ==(Range left, Range right)
            {
                return left.Equals(right);
            }
            
            /// <summary>
            /// Tests for equality between two objects.
            /// </summary>
            /// <param name="left">The first value to compare.</param>
            /// <param name="right">The second value to compare.</param>
            /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
            public static bool operator !=(Range left, Range right)
            {
                return !left.Equals(right);
            }

            /// <inheritdoc />
            public bool Equals(Range other)
            {
                return StartIndex == other.StartIndex && Length == other.Length;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Range && Equals((Range)obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return (StartIndex * 397) ^ Length;
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"StartIndex: {StartIndex}, Length: {Length}";
            }
        }
    }
}
