// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Json;

namespace Microsoft.IdentityModel.JsonWebTokens
{
    public partial class JsonWebToken
    {
        internal static JsonClaimSet CreatePayloadClaimSet(byte[] bytes, int length)
        {
            Utf8JsonReader reader = new(bytes.AsSpan().Slice(0, length));
            if (!JsonSerializerPrimitives.IsReaderAtTokenType(ref reader, JsonTokenType.StartObject, false))
                throw LogHelper.LogExceptionMessage(
                    new JsonException(
                        LogHelper.FormatInvariant(
                        Tokens.LogMessages.IDX11023,
                        LogHelper.MarkAsNonPII("JsonTokenType.StartObject"),
                        LogHelper.MarkAsNonPII(reader.TokenType),
                        LogHelper.MarkAsNonPII(ClassName),
                        LogHelper.MarkAsNonPII(reader.TokenStartIndex),
                        LogHelper.MarkAsNonPII(reader.CurrentDepth),
                        LogHelper.MarkAsNonPII(reader.BytesConsumed))));

            Dictionary<string, object> claims = new();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Aud))
                    {
                        List<string> audiences = new List<string>();
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.StartArray)
                        {
                            JsonSerializerPrimitives.ReadStringsSkipNulls(ref reader, audiences, JwtRegisteredClaimNames.Aud, ClassName);
                            claims[JwtRegisteredClaimNames.Aud] = audiences;
                        }
                        else
                        {
                            if (reader.TokenType != JsonTokenType.Null)
                            {
                                audiences.Add(JsonSerializerPrimitives.ReadString(ref reader, JwtRegisteredClaimNames.Aud, ClassName));
                                claims[JwtRegisteredClaimNames.Aud] = audiences[0];
                            }
                            else
                            {
                                claims[JwtRegisteredClaimNames.Aud] = audiences;
                            }
                        }
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Azp))
                    {
                        claims[JwtRegisteredClaimNames.Azp] = JsonSerializerPrimitives.ReadString(ref reader, JwtRegisteredClaimNames.Azp, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Exp))
                    {
                        claims[JwtRegisteredClaimNames.Exp] = JsonSerializerPrimitives.ReadLong(ref reader, JwtRegisteredClaimNames.Exp, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Iat))
                    {
                        claims[JwtRegisteredClaimNames.Iat] = JsonSerializerPrimitives.ReadLong(ref reader, JwtRegisteredClaimNames.Iat, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Iss))
                    {
                        claims[JwtRegisteredClaimNames.Iss] = JsonSerializerPrimitives.ReadString(ref reader, JwtRegisteredClaimNames.Iss, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Jti))
                    {
                        claims[JwtRegisteredClaimNames.Jti] = JsonSerializerPrimitives.ReadString(ref reader, JwtRegisteredClaimNames.Jti, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Nbf))
                    {
                        claims[JwtRegisteredClaimNames.Nbf] = JsonSerializerPrimitives.ReadLong(ref reader, JwtRegisteredClaimNames.Nbf, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtPayloadUtf8Bytes.Sub))
                    {
                        claims[JwtRegisteredClaimNames.Sub] = JsonSerializerPrimitives.ReadString(ref reader, JwtRegisteredClaimNames.Sub, ClassName, true);
                    }
                    else
                    {
                        string propertyName = reader.GetString();
                        claims[propertyName] = JsonSerializerPrimitives.ReadPropertyValueAsObject(ref reader, propertyName, JsonClaimSet.ClassName, true);
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            };

            return new JsonClaimSet(claims);
        }
    }
}
