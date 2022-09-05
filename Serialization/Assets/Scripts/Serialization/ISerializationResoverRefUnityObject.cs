// <copyright file="ISerializationResoverRefUnityObject.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
//
public interface ISerializationResoverRefUnityObject
{
    /// <summary>
    /// Prepare UnityObject for serialization before serialization
    /// </summary>
    void BeforeSerialize();
    /// <summary>
    /// Restore UnityObject from deserialization after deserialization
    /// </summary>
    void AfterDeserialize();
}
