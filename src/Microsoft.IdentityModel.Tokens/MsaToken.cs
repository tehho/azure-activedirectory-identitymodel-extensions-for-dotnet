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

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// A <see cref="SecurityToken"/> designed for representing a MSA Token. 
    /// </summary>
    public class MsaToken : SecurityToken
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MsaToken(string token)
        {
            EncodedToken = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <inheritdoc/>
        public override string Id => string.Empty;

        /// <inheritdoc/>
        public override string Issuer => string.Empty;

        /// <inheritdoc/>
        public override SecurityKey SecurityKey => null;

        /// <inheritdoc/>
        public override SecurityKey SigningKey { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        /// <inheritdoc/>
        public override DateTime ValidFrom => throw new NotSupportedException();

        /// <inheritdoc/>
        public override DateTime ValidTo => throw new NotSupportedException();
    }
}
