// <copyright file="OdinCallsExample.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>

using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using System.IO;
using System.Text;
using OdinSerializer.Utilities;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;
public static class OdinCallsExample
{
    private static readonly DataFormat[] arr = new DataFormat[] { DataFormat.JSON, DataFormat.Binary };
    private static DataFormat FormatData = arr[0];
    public static void SetFormat(int toolbarInt) => FormatData = arr[toolbarInt];

    #region StoreByOdinModUnityObject
    public static void SaveUnityObject(Object data, string filePath)
    {
        SerializationContext serContext = new SerializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = default;
        //UnitySerializationUtility.SerializeUnityObject(data, ref bytes, ref unityReferences, FormatData, serializeUnityFields: true, context: serContext);
        UnitySerializationUtilityMod.SerializeUnityObject(data, ref bytes, FormatData, serializeUnityFields: true, context: serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadUnityObject(Object data, string filePath)
    {
        DeserializationContext desContext = new DeserializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = File.ReadAllBytes(filePath);
        //UnitySerializationUtility.DeserializeUnityObject(data, ref bytes, ref unityReferences, FormatData, desContext);
        UnitySerializationUtilityMod.DeserializeUnityObject(data, ref bytes, FormatData, desContext);
    }
    #endregion

    #region StoreByOdinPlainClass
    /* with UnityObject SerializationUtility.SerializeValue works ok, but DeserializeValue create the new object in memmory link to UnityObject will be broken (+ warning messages)
     * with SystemObject (contains the ref to UnityObject) SerializationUtility.SerializeValue & DeserializeValue works ok
     * on both cases use the UniversalScriptableObjectStringReferenceResolve<T>
     */
    public static void Save<T>(T data, string filePath)
    {
        SerializationContext serContext = new SerializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
            //StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = SerializationUtility.SerializeValue(data, FormatData, serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static T Load<T>(string filePath)
    {
        DeserializationContext desContext = new DeserializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>(),
            //StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>(),
        };
        byte[] bytes = File.ReadAllBytes(filePath);
        return SerializationUtility.DeserializeValue<T>(bytes, FormatData, desContext);
    }
    #endregion

    #region StoreBySpecialSOAndJSONOdin
    public static void SaveSpecialSOByOdin<T>(T data, string filePath, ref List<Object> unityReferences) where T : UnityEngine.Object
    {
        byte[] bytes = default;
        SerializationContext myContext = new SerializationContext()
        {
            Config = new SerializationConfig() { SerializationPolicy = SpecialSerializationPolicy.SpecialUnity }
        };
        if (data is ISerializationResoverRefUnityObject resoverRefUnityObject)
        {
            resoverRefUnityObject.BeforeSerialize();
        }
        UnitySerializationUtility.SerializeUnityObject(data, ref bytes, ref unityReferences, FormatData, serializeUnityFields: true, context: myContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadSpecialSOByOdin<T>(T data, string filePath, ref List<Object> unityReferences) where T : UnityEngine.Object
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        DeserializationContext myContext = new DeserializationContext()
        {
            Config = new SerializationConfig() { SerializationPolicy = SpecialSerializationPolicy.SpecialUnity }
        };
        UnitySerializationUtility.DeserializeUnityObject(data, ref bytes, ref unityReferences, FormatData, context: myContext);
        if (data is ISerializationResoverRefUnityObject resoverRefUnityObject)
        {
            resoverRefUnityObject.AfterDeserialize();
        }
    }

    #endregion

    #region StoreBySpeciaSOAndJSONUnity
    public static void SaveSpecialSOByUnity<T>(T data, string filePath) where T : UnityEngine.Object
    {
        using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8, 1024))
        {
            if (data is ISerializationResoverRefUnityObject resoverRefUnityObject)
            {
                resoverRefUnityObject.BeforeSerialize();
            }
            string strJSON = JsonUtility.ToJson(data);
            sw.WriteLine(strJSON);
        }
    }

    public static void LoadSpecialSOByUnity<T>(T data, string filePath) where T : UnityEngine.Object
    {
        using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, false, 1024))
        {
            string str = sr.ReadLine();
            JsonUtility.FromJsonOverwrite(str, data);
            if (data is ISerializationResoverRefUnityObject resoverRefUnityObject)
            {
                resoverRefUnityObject.AfterDeserialize();
            } 
        }
    }
    #endregion

    #region StoreByOdinClassicInTwoStepsSerializations
    public static void SaveTwoSteps<T>(T data, string filePath, ref List<Object> unityReferences)
    {
        byte[] bytes = SerializationUtility.SerializeValue(data, FormatData, out unityReferences);
        File.WriteAllBytes(filePath, bytes);
    }

    public static T LoadTwoSteps<T>(string filePath, List<Object> unityReferences)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        return SerializationUtility.DeserializeValue<T>(bytes, FormatData, unityReferences);
    }
    #endregion

    #region StoreUnityRefByUniversalRefResolvers
    /*
     * They Store the Unity Refs which they get from List<Object> unityReferences 
     */
    private static string FileName((string fileBase, string fileExt) fileName, int num) => fileName.fileBase + num + fileName.fileExt;

    public static void SaveStrRef((string fileBase, string fileExt) fileName, List<Object> unityReferences)
    {
        SerializationContext serContext = new SerializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>()
            {
                NextResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>()
            },
        };
        for (int i = 0; i < unityReferences.Count; i++)
        {
            byte[] bytes = Serialize(unityReferences[i], serContext);
            File.WriteAllBytes(FileName(fileName, i), bytes);
        }
    }

    public static void LoadStrRef((string fileBase, string fileExt) fileName, List<Object> unityReferences, bool clearUnityReferences = true)
    {
        DeserializationContext desContext = new DeserializationContext()
        {
            StringReferenceResolver = new UniversalScriptableObjectStringReferenceResolver<ComplexitySO>()
            {
                NextResolver = new UniversalScriptableObjectStringReferenceResolver<LevelSO>()
            },
        };
        if (clearUnityReferences)
        {
            unityReferences.Clear();
        }
        int i = 0;
        string currentFileName = FileName(fileName, i);
        while (File.Exists(currentFileName))
        {
            byte[] bytes = File.ReadAllBytes(currentFileName);
            unityReferences.Add((Object)Deserialize(bytes, desContext));
            currentFileName = FileName(fileName, ++i);
        }
    }
    #endregion

    #region StoreByOdinClassicRefResolversEditorOnly
#if UNITY_EDITOR
    public static void SaveGuiDRef(string filePath, List<Object> unityReferences)
    {
        var serContext = new SerializationContext()
        {
            GuidReferenceResolver = new ScriptableObjectGuidReferenceResolver(),
        };
        byte[] bytes = Serialize(unityReferences[0], serContext);
        File.WriteAllBytes(filePath, bytes);
    }

    public static void LoadGuiDRef(string filePath, List<Object> unityReferences, bool clearUnityReferences = true)
    {
        if (clearUnityReferences)
        {
            unityReferences.Clear();
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        var desContext = new DeserializationContext()
        {
            GuidReferenceResolver = new ScriptableObjectGuidReferenceResolver(),
        };
        unityReferences.Add((Object)Deserialize(bytes, desContext));
    }
#endif
    #endregion
       

    #region CommonForOdinRefResolvers
    static byte[] Serialize(object obj, SerializationContext context)
    {
        return SerializationUtility.SerializeValue(obj, FormatData, context);
    }

    static object Deserialize(byte[] bytes, DeserializationContext context)
    {
        return SerializationUtility.DeserializeValue<object>(bytes, FormatData, context);
    } 
    #endregion
}