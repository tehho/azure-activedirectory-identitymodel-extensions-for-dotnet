// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using BenchmarkDotNet.Attributes;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.TestUtils;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.IdentityModel.Benchmarks
{
    [MemoryDiagnoser]
    public class CreateTokenTests
    {
        private JsonWebTokenHandler _jsonWebTokenHandler;
        private SecurityTokenDescriptor _tokenDescriptor;

        [GlobalSetup]
        public void Setup()
        {
            DateTime now = DateTime.UtcNow;
            _jsonWebTokenHandler = new JsonWebTokenHandler();
            _tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = Default.Audience,
                Expires = now + TimeSpan.FromDays(1),
                Claims = new Dictionary<string, object>
                {
                    { JwtRegisteredClaimNames.Azp, Default.Azp },
                    { JwtRegisteredClaimNames.Email, "Bob@contoso.com" },
                    { JwtRegisteredClaimNames.Jti, Default.Jti },
                },
                Issuer = Default.Issuer,
                IssuedAt = now,
                NotBefore = now,
                SigningCredentials = KeyingMaterial.JsonWebKeyRsa256SigningCredentials,
            };
        }

        [Benchmark]
        public string JsonWebTokenHandler_CreateToken()
        {
            for (int i = 0; i< 1000; i++)
                _jsonWebTokenHandler.CreateToken(_tokenDescriptor);

            return "";
        }
    }
}
