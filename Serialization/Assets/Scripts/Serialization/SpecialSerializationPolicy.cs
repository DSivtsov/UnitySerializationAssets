// <copyright file="specialUnitySerializationPolicy.cs">
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
//----------------------------------------------------------------------------------------------------
// The file was modified to add possibility to use NonOdinSerializedAttribute attribute in Odin policy
// added ISerializationPolicy specialUnityPolicy
//----------------------------------------------------------------------------------------------------
using UnityEngine;
using OdinSerializer;
using OdinSerializer.Utilities;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
/// <summary>
/// specialUnitySerializationPolicy support the NonOdinSerializedAttribute attribute in Odin policy
/// </summary>
public static class SpecialSerializationPolicy
{
    private static readonly object LOCK = new object();

    private static volatile ISerializationPolicy specialUnityPolicy;

    /// <summary>
    /// Modification: Use the NonOdinSerializedAttribute to excluded members from serialization
    /// <para /> Standard behavior of ISerializationPolicy Unity:
    /// Public fields, as well as fields or auto-properties marked with <see cref="SerializeField"/> or <see cref="OdinSerializeAttribute"/> and not marked with <see cref="NonSerializedAttribute"/>, are serialized.
    /// <para />
    /// There are two exceptions:
    /// <para/>1) All fields in tuples, as well as in private nested types marked as compiler generated (e.g. lambda capture classes) are also serialized.
    /// <para/>2) Virtual auto-properties are never serialized. Note that properties specified by an implemented interface are automatically marked virtual by the compiler.
    /// </summary>
    public static ISerializationPolicy SpecialUnity
    {
        get
        {
            if (specialUnityPolicy == null)
            {
                lock (LOCK)
                {
                    if (specialUnityPolicy == null)
                    {
                        // In Unity 2017.1's .NET 4.6 profile, Tuples implement System.ITuple. In Unity 2017.2 and up, tuples implement System.ITupleInternal instead for some reason.
                        Type tupleInterface = typeof(string).Assembly.GetType("System.ITuple") ?? typeof(string).Assembly.GetType("System.ITupleInternal");

                        specialUnityPolicy = new CustomSerializationPolicy("OdinSerializerPolicies.Unity", true, (member) =>
                        {
                            /*
                             * Use the NonOdinSerializedAttribute
                             */
                            if (member.IsDefined<NonOdinSerializedAttribute>(true))
                            {
#if UNITY_EDITOR
                                Debug.Log($"ISerializationPolicy : member={member.Name} will exclude from serialization"); 
#endif
                                return false;
                            }

                            // As of Odin 3.0, we now allow non-auto properties and virtual properties.
                            // However, properties still need a getter and a setter.
                            if (member is PropertyInfo)
                            {
                                var propInfo = member as PropertyInfo;
                                if (propInfo.GetGetMethod(true) == null || propInfo.GetSetMethod(true) == null) return false;
                            }

                            // If OdinSerializeAttribute is defined, NonSerializedAttribute is ignored.
                            // This enables users to ignore Unity's infinite serialization depth warnings.
                            if (member.IsDefined<NonSerializedAttribute>(true) && !member.IsDefined<OdinSerializeAttribute>())
                            {
                                return false;
                            }

                            if (member is FieldInfo && ((member as FieldInfo).IsPublic || (member.DeclaringType.IsNestedPrivate && member.DeclaringType.IsDefined<CompilerGeneratedAttribute>()) || (tupleInterface != null && tupleInterface.IsAssignableFrom(member.DeclaringType))))
                            {
                                return true;
                            }

                            return member.IsDefined<SerializeField>(false) || member.IsDefined<OdinSerializeAttribute>(false) || (UnitySerializationUtility.SerializeReferenceAttributeType != null && member.IsDefined(UnitySerializationUtility.SerializeReferenceAttributeType, false));
                        });
                    }
                }
            }

            return specialUnityPolicy;
        }
    }
}