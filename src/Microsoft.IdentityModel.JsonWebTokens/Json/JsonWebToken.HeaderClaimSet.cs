// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens.Json;

namespace Microsoft.IdentityModel.JsonWebTokens
{
    public partial class JsonWebToken
    {
        internal static JsonClaimSet CreateHeaderClaimSet(byte[] bytes)
        {
            return CreateHeaderClaimSet(bytes, bytes.Length);
        }

        internal static JsonClaimSet CreateHeaderClaimSet(byte[] bytes, int length)
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
                    if (reader.ValueTextEquals(JwtHeaderUtf8Bytes.Alg))
                    {
                        claims[JwtHeaderParameterNames.Alg] = JsonSerializerPrimitives.ReadString(ref reader, JwtHeaderParameterNames.Alg, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtHeaderUtf8Bytes.Cty))
                    {
                        claims[JwtHeaderParameterNames.Cty] = JsonSerializerPrimitives.ReadString(ref reader, JwtHeaderParameterNames.Cty, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtHeaderUtf8Bytes.Kid))
                    {
                        claims[JwtHeaderParameterNames.Kid] = JsonSerializerPrimitives.ReadString(ref reader, JwtHeaderParameterNames.Kid, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtHeaderUtf8Bytes.Typ))
                    {
                        claims[JwtHeaderParameterNames.Typ] = JsonSerializerPrimitives.ReadString(ref reader, JwtHeaderParameterNames.Typ, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtHeaderUtf8Bytes.X5t))
                    {
                        claims[JwtHeaderParameterNames.X5t] = JsonSerializerPrimitives.ReadString(ref reader, JwtHeaderParameterNames.X5t, ClassName, true);
                    }
                    else if (reader.ValueTextEquals(JwtHeaderUtf8Bytes.Zip))
                    {
                        claims[JwtHeaderParameterNames.Zip] = JsonSerializerPrimitives.ReadString(ref reader, JwtHeaderParameterNames.Zip, ClassName, true);
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
