//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.TestUtils;
using Microsoft.IdentityModel.Tokens;
using Xunit;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Microsoft.IdentityModel.JsonWebTokens.Tests
{
    public class StringSegmentTests
    {
        [Theory, MemberData(nameof(SegmentSplitTheoryData))]
        public void SplitTests(StringSegmentTheoryData theoryData)
        {
            var ss = new StringSegment(theoryData.Data);
            IList<StringSegment> segments = null;
            if (theoryData.Delimiter != default)
                segments = ss.Split(theoryData.Delimiter, theoryData.SplitOption, theoryData.TrimSpaces);

            Assert.True(segments.Count == theoryData.ExpectedSubstrings.Count());
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                if (theoryData.TrimChar != default)
                    segment.Trim(theoryData.TrimChar);

                if (theoryData.TrimSpaces)
                    segment.Trim();

                segments[i] = segment;

                Assert.Equal(segments[i].ToString(), theoryData.ExpectedSubstrings[i]);
            }

            Assert.True(segments.Count == theoryData.ExpectedSubstrings.Count());
        }

        public static TheoryData<StringSegmentTheoryData> SegmentSplitTheoryData()
        {
            var theoryData = new TheoryData<StringSegmentTheoryData>();

            theoryData.Add(new StringSegmentTheoryData("TokenMultipleConsecutiveEmptiesRemoveEmties")
            {
                Data = ", , ,   header, ,   payload ,",
                Delimiter = ',',
                TrimSpaces = true,
                ExpectedSubstrings = new string[] { "header", "payload" },
                SplitOption = StringSplitOptions.RemoveEmptyEntries
            });
            theoryData.Add(new StringSegmentTheoryData("Query")
            {
                Data = "?a=Ann&b=Bar&c=Can",
                Delimiter = '&',
                TrimChar = '?',
                ExpectedSubstrings = new string[] { "a=Ann", "b=Bar", "c=Can" },
            });
            theoryData.Add(new StringSegmentTheoryData("TokenNoWhitespace")
            {
                Data = "header,payload,signature",
                Delimiter = ',',
                ExpectedSubstrings = new string[] { "header", "payload", "signature" },
            });
            theoryData.Add(new StringSegmentTheoryData("TokenWithWhitespace")
            {
                Data = " header  . payload . signature      ",
                Delimiter = '.',
                TrimSpaces = true,
                ExpectedSubstrings = new string[] { "header", "payload", "signature" },
            });
            theoryData.Add(new StringSegmentTheoryData("TokenNoSignature")
            {
                Data = "header.payload.",
                Delimiter = '.',
                ExpectedSubstrings = new string[] { "header", "payload", "" },
            });
            theoryData.Add(new StringSegmentTheoryData("TokenNoSignatureRemoveEmpties")
            {
                Data = "header.payload.",
                Delimiter = '.',
                ExpectedSubstrings = new string[] { "header", "payload" },
                SplitOption = StringSplitOptions.RemoveEmptyEntries
            });
            theoryData.Add(new StringSegmentTheoryData("TokenMultipleEmpties")
            {
                Data = ",header,payload,",
                Delimiter = ',',
                ExpectedSubstrings = new string[] { "", "header", "payload", "" },
            });
            theoryData.Add(new StringSegmentTheoryData("TokenMultipleEmptyEntriesRemoveEmpties")
            {
                Data = ",header,payload,",
                Delimiter = ',',
                ExpectedSubstrings = new string[] { "header", "payload" },
                SplitOption = StringSplitOptions.RemoveEmptyEntries
            });
            theoryData.Add(new StringSegmentTheoryData("TokenMultipleConsecutiveEmpties")
            {
                Data = ",,,  header ,  , payload ,",
                Delimiter = ',',
                TrimSpaces = true,
                ExpectedSubstrings = new string[] { "", "", "", "header", "", "payload", "" },
            });

            return theoryData;
        }

        [Theory, MemberData(nameof(SegmentTrimTheoryData))]
        public void TrimTests(StringSegmentTheoryData theoryData)
        {
            var ss = new StringSegment(theoryData.Data);
            IList<StringSegment> segments = null;
            if (theoryData.Delimiter != default)
                segments = ss.Split(theoryData.Delimiter, theoryData.SplitOption);

            Assert.True(segments.Count == theoryData.ExpectedSubstrings.Count());
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                if (theoryData.TrimChar != default)
                    segment.Trim(theoryData.TrimChar);

                if (theoryData.TrimSpaces)
                    segment.Trim();

                segments[i] = segment;

                Assert.Equal(segments[i].ToString(), theoryData.ExpectedSubstrings[i]);
            }

            Assert.True(segments.Count == theoryData.ExpectedSubstrings.Count());
        }
        public static TheoryData<StringSegmentTheoryData> SegmentTrimTheoryData()
        {
            var theoryData = new TheoryData<StringSegmentTheoryData>();

            theoryData.Add(new StringSegmentTheoryData("TokenWhitespaces")
            {
                Data = "  header  ,  payload , signature",
                Delimiter = ',',
                TrimChar = ' ',
                ExpectedSubstrings = new string[] { "header", "payload", "signature" },
            });

            theoryData.Add(new StringSegmentTheoryData("TokenWhitespacesRemoveEmptyEntries")
            {
                Data = "  header  ,  payload , signature",
                Delimiter = ',',
                TrimChar = ' ',
                ExpectedSubstrings = new string[] { "header", "payload", "signature" },
                SplitOption = StringSplitOptions.RemoveEmptyEntries
            });

            theoryData.Add(new StringSegmentTheoryData("TokenWhitespacesLastEmptyEntry")
            {
                Data = "  header  ,  payload ,",
                Delimiter = ',',
                TrimSpaces = true,
                ExpectedSubstrings = new string[] { "header", "payload", ""},
                SplitOption = StringSplitOptions.None
            });

            theoryData.Add(new StringSegmentTheoryData("TokenWhitespacesRemoveLastEmptyEntry")
            {
                Data = "  header  ,  payload ,",
                Delimiter = ',',
                TrimSpaces = true,
                ExpectedSubstrings = new string[] { "header", "payload" },
                SplitOption = StringSplitOptions.RemoveEmptyEntries
            });

            return theoryData;
        }

        public class StringSegmentTheoryData : TheoryDataBase
        {
            public StringSegmentTheoryData(string testId) : base(testId) { }

            public string Data { get; set; }

            public bool TrimSpaces { get; set; }

            public char TrimChar { get; set; } = default;

            public char Delimiter { get; set;}

            public string[] ExpectedSubstrings { get; set; }

            public StringSplitOptions SplitOption { get; set; } = StringSplitOptions.None;
        }
    }
}

#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
