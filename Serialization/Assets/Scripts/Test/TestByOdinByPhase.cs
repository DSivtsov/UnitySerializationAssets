// <copyright file="TestSaveLoad.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
#define TWOSTEP_SER
using System.Collections.Generic;
using UnityEngine;

public class TestByOdinByPhase : MonoBehaviour
{
    private const string FileExtension = ".txt";
    private const string FilePathBasePartStrDRef = "OdinStrRef";
    private const string FilePathMainFile = "TestOdinByPhase.txt";

    private string _currentValues1, _currentValues2;
    //private int toolbarPrev = 0, toolbarCur = 0;
    //private string[] toolbarStrings = { "JSON", "Binary" };
    private Vector2 scrollViewVector = Vector2.zero;

    private string objValues;

    [SerializeField] private GameSettingsComplex _gameSettingsComplex;

    [SerializeField] private List<Object> unityReferences = new List<Object>();

	private void OnGUI()
	{
        if (GUI.Button(new Rect(0, 70, 150, 30), "SaveFirstStep"))
        {
            Debug.Log("ShowUnityObject & SaveTwoSteps");
            ShowValues();
            OdinCallsExample.SaveTwoSteps(_gameSettingsComplex, FilePathMainFile, ref unityReferences);
        }

        if (GUI.Button(new Rect(0, 100, 150, 30), "SaveStrRef"))
        {
            Debug.Log("SaveStrRef");
            OdinCallsExample.SaveStrRef((FilePathBasePartStrDRef, FileExtension), unityReferences);
        }

        if (GUI.Button(new Rect(250, 70, 150, 30), "ShowUnityObject"))
        {
            Debug.Log("ShowUnityObject");
            ShowValues();
        }

        if (GUI.Button(new Rect(500, 70, 150, 30), "LoadTwoSteps"))
        {
            Debug.Log("LoadTwoSteps & ShowUnityObject");
            _gameSettingsComplex = OdinCallsExample.LoadTwoSteps<GameSettingsComplex>(FilePathMainFile, unityReferences);
            ShowValues();
        }

        if (GUI.Button(new Rect(500, 100, 150, 30), "LoadStrRef"))
        {
            Debug.Log("LoadStrRef");
            OdinCallsExample.LoadStrRef((FilePathBasePartStrDRef, FileExtension), unityReferences);
        }

        if (GUI.Button(new Rect(250, 100, 150, 30), "RandomizeValues"))
        {
            Debug.Log("ChangeValues & ShowUnityObject");
            _gameSettingsComplex.ChangeValues();
            ShowValues();
        }

        // Begin the ScrollView
        scrollViewVector = GUI.BeginScrollView(new Rect(150, 220, 350, 200), scrollViewVector, new Rect(0, 0, 500, 300));

        // Put something inside the ScrollView
        GUI.TextArea(new Rect(0, 0, 500, 300), objValues);

        // End the ScrollView
        GUI.EndScrollView();

        //toolbarCur = GUI.Toolbar(new Rect(200, 25, 250, 30), toolbarPrev, toolbarStrings);
        //if (toolbarCur != toolbarPrev)
        //{
        //    OdinCallsExample.SetFormat(toolbarCur);
        //    toolbarPrev = toolbarCur;
        //}
    }
    private void ShowValues()
    {
        (_currentValues1, _currentValues2) = _gameSettingsComplex.GetValues();
        objValues = _currentValues1 + _currentValues2;
        Debug.Log($"ShowValues() : ComplexityGame={_gameSettingsComplex.ComplexityGame}");
        Debug.Log($"ShowValues() : LevelSO={_gameSettingsComplex.LevelSO}");
        Debug.Log($"ShowValues() : {_gameSettingsComplex}");
    }

    private void Awake()
    {
        ShowValues();
    }
}
