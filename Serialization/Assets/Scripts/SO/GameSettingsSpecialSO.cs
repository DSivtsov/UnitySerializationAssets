// <copyright file="GameSettingsSpecialSO.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
//
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using GameTestClass;
using PlayMode = GameTestClass.PlayMode;

[CreateAssetMenu(fileName = "GameSettingsSpecialSO", menuName = "SO/GameSettingsSpecialSO")]
public class GameSettingsSpecialSO : ScriptableObject, ISerializationResoverRefUnityObject
{
    [Header("Game Options")]
    [SerializeField, NonOdinSerialized] private LevelSO _levelPlayer;
    //for case without the NonOdinSerialized attribute
    //[SerializeField] private LevelSO _levelPlayer;
    [SerializeField] private PlayMode _usedPlayMode;
    [SerializeField] private bool _notCopyToGlobal;
    [SerializeField] private bool _globalDefaultTopList;
    [Header("Audio Options")]
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _effectVolume;
    [SerializeField] private SequenceType _musicSequenceType;

    #region PropertiesForExternalAccessOnly
    public PlayMode UsedPlayMode => _usedPlayMode;
    public bool NotCopyToGlobal => _notCopyToGlobal;
    public bool GlobalDefaultTopList => _globalDefaultTopList;
    public LevelSO LevelPlayer => _levelPlayer;
    #endregion

    #region SpecialFieldsAndPublicMethodsToSupportSerializationScriptableObject
    [SerializeField,HideInInspector] private string _levelSOName;

    /// <summary>
    /// Prepare GameSettingsSpecialSO UnityObject for serialization before serialization
    /// </summary>
    public void BeforeSerialize()
    {
        //for memmber LevelSO
        _levelSOName = GetStringReferenceLevelSO();
    }

    /// <summary>
    /// Restore GameSettingsSpecialSO UnityObject from deserialization after deserialization
    /// </summary>
    public void AfterDeserialize()
    {
        //for memmber LevelSO
        _levelPlayer = ResolveStringReferenceLevelSO();
    }
    #endregion

    #region DataAndPrivateMethodsToSupportSerializationClassMember_LevelSO
    private Dictionary<string, LevelSO> dictLevelSO;

    private void OnEnable()
    {
        if (dictLevelSO == null)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                FillDictionaryForResolverRefLevelSO();
            }
#else
            FillDictionaryForResolverRefLevelSO();
#endif
        }
    }

    private void FillDictionaryForResolverRefLevelSO()
    {
        dictLevelSO = Resources.LoadAll<LevelSO>("").ToDictionary((level) => level.name, (level) => level);
#if UNITY_EDITOR
        Debug.Log($"OnEnable() : LevelSO dict filled dict.Count={dictLevelSO.Count}"); 
#endif
    }

    private string GetStringReferenceLevelSO() => _levelPlayer.name;

    private LevelSO ResolveStringReferenceLevelSO()
    {
        if (dictLevelSO != null)
        {
            if (_levelSOName != null)
            {
                if (dictLevelSO.TryGetValue(_levelSOName, out LevelSO levelSO))
                {
                    return levelSO;
                }
                else
                    Debug.LogError($"RestoreAfterLoad() : Can find the LevelSO with {_levelSOName} name");
            }
        }
        else
            Debug.Log($"RestoreAfterLoad() : dict==null [{dictLevelSO == null}]");
        return null;
    }
    #endregion

    #region OnlyForTesting
    public override string ToString() => JsonUtility.ToJson(this);
    //public void ChangeValues() => _levelPlayer = null;

    public (string val1, string val2) GetValues()
    {
        return ($"_levelPlayer=[{((_levelPlayer == null) ? null : _levelPlayer.name)}]\n_usedPlayMode=[{_usedPlayMode}]\n_notCopyToGlobal=[{_notCopyToGlobal}]\n_globalDefaultTopListr=[{_globalDefaultTopList}]\n",
            $"_masterVolume=[{_masterVolume}]\n_musicVolume=[{_musicVolume}]\n_effectVolume=[{_effectVolume}]\n_musicSequenceType=[{_musicSequenceType}]\n");
    }

    public void ChangeValues()
    {
        _levelPlayer = null;
        _usedPlayMode = (PlayMode.Offline == _usedPlayMode) ? PlayMode.Online : PlayMode.Offline;
        _notCopyToGlobal = !_notCopyToGlobal;
        _globalDefaultTopList = !_globalDefaultTopList;
        _masterVolume = UnityEngine.Random.Range(-20.0f, 40.0f);
        _musicVolume = UnityEngine.Random.Range(-20.0f, 40.0f);
        _effectVolume = UnityEngine.Random.Range(-20.0f, 40.0f);
        _musicSequenceType = (SequenceType)(((int)_musicSequenceType + 1) % 3);
    }
    #endregion
}
