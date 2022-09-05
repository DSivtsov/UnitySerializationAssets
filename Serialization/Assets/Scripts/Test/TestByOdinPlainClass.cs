// <copyright file="TestByOdinPlainClass.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>

using UnityEngine;

public class TestByOdinPlainClass : MonoBehaviour
{
    private const string FilePathGameSettingsPlainClass = "ByOdinPlainClass.txt";

    [SerializeField] private GameSettings _gameSettingPlainClass;

    private string _currentValues1, _currentValues2;
    private int toolbarPrev = 0, toolbarCur=0;
    private string[] toolbarStrings = { "JSON", "Binary"};
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 70, 150, 30), "SaveUnityObject"))
        {
            Debug.Log("ShowUnityObject & SaveUnityObject");
            ShowValues();
            OdinCallsExample.Save(_gameSettingPlainClass, FilePathGameSettingsPlainClass);
        }

        if (GUI.Button(new Rect(500, 70, 150, 30), "LoadUnityObject"))
        {
            Debug.Log("LoadUnityObject & ShowUnityObject");
            _gameSettingPlainClass = OdinCallsExample.Load<GameSettings>(FilePathGameSettingsPlainClass);
            ShowValues();
        }

        if (GUI.Button(new Rect(250, 70, 150, 30), "ShowUnityObject"))
        {
            Debug.Log("ShowUnityObject");
            ShowValues();
        }

        if (GUI.Button(new Rect(250, 320, 150, 30), "RandomizeValues"))
        {
            Debug.Log("ChaneValues & ShowUnityObject");
            _gameSettingPlainClass.ChangeValues();
            ShowValues();
        }

        GUI.Label(new Rect(250, 120, 250, 80), _currentValues1);
        GUI.Label(new Rect(250, 220, 250, 80), _currentValues2);

        toolbarCur = GUI.Toolbar(new Rect(200, 25, 250, 30), toolbarPrev, toolbarStrings);
        if (toolbarCur != toolbarPrev)
        {
            OdinCallsExample.SetFormat(toolbarCur);
            toolbarPrev = toolbarCur;
        }
    }

    private void ShowValues()
    {
        (_currentValues1, _currentValues2) = _gameSettingPlainClass.GetValues();
        Debug.Log($"LevelPlayer={_gameSettingPlainClass.ComplexityGame}");
        Debug.Log(_gameSettingPlainClass);
    }

    private void Awake()
	{
        (_currentValues1, _currentValues2) = _gameSettingPlainClass.GetValues();
    }
}
