using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using GameTestClass;
using PlayMode = GameTestClass.PlayMode;

[CreateAssetMenu(fileName = "GameSettingsSimpleSO", menuName = "SO/GameSettingsSimpleSO")]
public class GameSettingsSimpleSO : ScriptableObject
{
    [Header("Game Options")]
    [SerializeField] private LevelSO _levelPlayer;
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

    #region MethodsForTestingOnly
    public override string ToString() => JsonUtility.ToJson(this);

    public (string val1, string val2) GetValues()
    {
        return ($"_levelPlayer=[{((_levelPlayer==null)?null:_levelPlayer.name)}]\n_usedPlayMode=[{_usedPlayMode}]\n_notCopyToGlobal=[{_notCopyToGlobal}]\n_globalDefaultTopListr=[{_globalDefaultTopList}]\n",
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



