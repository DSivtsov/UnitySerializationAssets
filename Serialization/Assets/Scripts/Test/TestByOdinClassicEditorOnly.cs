// <copyright file="TestSaveLoad.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
#define EditorOnlyStoreOdinClassicRefResolvers
using System.Collections.Generic;
using UnityEngine;

public class TestByOdinClassicEditorOnly : MonoBehaviour
{
    private const string FileExtension = ".txt";
    private const string FilePathBasePartStrDRef = "OdinStrRef";
    private const string FilePathBasePartGuiDRef = "OdinGuiDRef";
    private const string FilePathGuiDRef = FilePathBasePartGuiDRef + FileExtension;
    private const string FilePathMainFile = "TestOdinClassic.txt";

    private string _currentValues1, _currentValues2;
    private Vector2 scrollViewVector = Vector2.zero;
    private string objValues;

    [SerializeField] private GameSettings _gameSettings;

    [SerializeField] private List<Object> unityReferences = new List<Object>();

	private void OnGUI()
	{
        if (GUI.Button(new Rect(0, 70, 150, 30), "SaveFirstStep"))
        {
            Debug.Log("ShowUnityObject & SaveTwoSteps");
            ShowValues();
            OdinCallsExample.SaveTwoSteps(_gameSettings, FilePathMainFile, ref unityReferences);
        }

        if (GUI.Button(new Rect(250, 70, 150, 30), "ShowUnityObject"))
        {
            Debug.Log("ShowUnityObject");
            ShowValues();
        }

        if (GUI.Button(new Rect(500, 70, 150, 30), "LoadTwoSteps"))
        {
            Debug.Log("LoadTwoSteps & ShowUnityObject");
            _gameSettings = OdinCallsExample.LoadTwoSteps<GameSettings>(FilePathMainFile, unityReferences);
            ShowValues();
        }

        if (GUI.Button(new Rect(250, 100, 150, 30), "RandomizeValues"))
        {
            Debug.Log("ChangeValues & ShowUnityObject");
            _gameSettings.ChangeValues();
            ShowValues();
        }

        // Begin the ScrollView
        scrollViewVector = GUI.BeginScrollView(new Rect(150, 220, 350, 200), scrollViewVector, new Rect(0, 0, 500, 300));

        // Put something inside the ScrollView
        GUI.TextArea(new Rect(0, 0, 500, 300), objValues);

        // End the ScrollView
        GUI.EndScrollView();

#if UNITY_EDITOR
        if (GUI.Button(new Rect(0, 100, 150, 30), "SaveGuiDRef"))
        {
            Debug.Log("SaveGuiDRef");
            OdinCallsExample.SaveGuiDRef(FilePathGuiDRef, unityReferences);
        }

        if (GUI.Button(new Rect(500, 100, 150, 30), "LoadGuiDRef"))
        {
            Debug.Log("LoadGuiDRef");
            OdinCallsExample.LoadGuiDRef(FilePathGuiDRef, unityReferences);
        }
#endif
    }

    private void ShowValues()
    {
        (_currentValues1, _currentValues2) = _gameSettings.GetValues();
        objValues = _currentValues1 + _currentValues2;
        Debug.Log($"ShowValues() : ComplexityGame={_gameSettings.ComplexityGame}");
        Debug.Log($"ShowValues() : {_gameSettings}");
    }

    private void Awake()
    {
        ShowValues();
    }
}
