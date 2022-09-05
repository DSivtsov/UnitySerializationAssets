// <copyright file="UnitySerializationUtilityMod.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
//
//  NOTICE: THIS FILE HAS BEEN MODIFIED BY Denis Sivtsov
//  UNDER COMPLIANCE WITH THE APACHE 2.0 LICENCE
//  FROM THE ORIGINAL WORK OF THE COMPANY "Sirenix IVS".
// </copyright>
//  THE FOLLOWING IS THE COPYRIGHT OF THE ORIGINAL DOCUMENT:
//-----------------------------------------------------------------------
// <copyright file="UnitySerializationUtility.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
// The file was modified to add possibility to serialize/deserialize UnityObject
// with using the reference resolvers which support IExternalGuidReferenceResolver
// or IExternalStringReferenceResolver interface
//-----------------------------------------------------------------------
namespace OdinSerializer
{
    using System;
    using System.IO;
    using Utilities;
    using UnityEngine;

    public static class UnitySerializationUtilityMod
    {
        public static void SerializeUnityObject(UnityEngine.Object unityObject, ref byte[] bytes, DataFormat format, bool serializeUnityFields = false, SerializationContext context = null)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }
            if (format == DataFormat.Nodes)
            {
                Debug.LogError("The serialization data format '" + format.ToString() + "' is not supported by this method. You must create your own writer.");
                return;
            }
            using (var stream = Cache<CachedMemoryStream>.Claim())
            {
                if (context != null)
                {
                    using (var writerCache = GetCachedUnityWriter(format, stream.Value.MemoryStream, context))
                    {
                        UnitySerializationUtility.SerializeUnityObject(unityObject, writerCache.Value as IDataWriter, serializeUnityFields);
                    }
                }
                else
                {
                    Debug.LogError("This serialization  method without set ReferenceResolver not supported");
                    return;
                }
                bytes = stream.Value.MemoryStream.ToArray();
            }
        }

        public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref byte[] bytes, DataFormat format, DeserializationContext context = null)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            if (format == DataFormat.Nodes)
            {
                try
                {
                    Debug.LogError("The serialization data format '" + format.ToString() + "' is not supported by this method. You must create your own reader.");
                }
                catch { }
                return;
            }
            using (var stream = Cache<CachedMemoryStream>.Claim())
            {
                stream.Value.MemoryStream.Write(bytes, 0, bytes.Length);
                stream.Value.MemoryStream.Position = 0;
                if (context != null)
                {
                    using (var readerCache = GetCachedUnityReader(format, stream.Value.MemoryStream, context))
                    {
                        UnitySerializationUtility.DeserializeUnityObject(unityObject, readerCache.Value as IDataReader);
                    }
                }
                else
                {
                    Debug.LogError("This serialization  method without set ReferenceResolver not supported");
                    return;
                }
            }
        }

        private static ICache GetCachedUnityWriter(DataFormat format, Stream stream, SerializationContext context)
        {
            ICache cache;
            switch (format)
            {
                case DataFormat.Binary:
                    {
                        var c = Cache<BinaryDataWriter>.Claim();
                        c.Value.Stream = stream;
                        cache = c;
                    }
                    break;
                case DataFormat.JSON:
                    {
                        var c = Cache<JsonDataWriter>.Claim();
                        c.Value.Stream = stream;
                        cache = c;
                    }
                    break;
                case DataFormat.Nodes:
                    throw new InvalidOperationException("Don't do this for nodes!");
                default:
                    throw new NotImplementedException(format.ToString());
            }
            (cache.Value as IDataWriter).Context = context;
            return cache;
        }

        private static ICache GetCachedUnityReader(DataFormat format, Stream stream, DeserializationContext context)
        {
            ICache cache;

            switch (format)
            {
                case DataFormat.Binary:
                    {
                        var c = Cache<BinaryDataReader>.Claim();
                        c.Value.Stream = stream;
                        cache = c;
                    }
                    break;
                case DataFormat.JSON:
                    {
                        var c = Cache<JsonDataReader>.Claim();
                        c.Value.Stream = stream;
                        cache = c;
                    }
                    break;
                case DataFormat.Nodes:
                    throw new InvalidOperationException("Don't do this for nodes!");
                default:
                    throw new NotImplementedException(format.ToString());
            }
            (cache.Value as IDataReader).Context = context;
            return cache;
        }
    }
}
