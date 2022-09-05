// <copyright file="NonOdinSerializedAttribute.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
using System;

/// <summary>
/// Indicates that an instance field or auto-property should be Not serialized by Odin vs current rules of used policy
/// </summary>
/// <seealso cref="System.Attribute" />
[JetBrains.Annotations.MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class NonOdinSerializedAttribute : Attribute
{
}