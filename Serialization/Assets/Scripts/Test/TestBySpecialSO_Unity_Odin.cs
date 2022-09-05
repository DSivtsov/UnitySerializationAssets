using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestBySpecialSO_Unity_Odin : MonoBehaviour
{
    private const string FilePathOdinSerialization = "SpecialSO_ByOdin.txt";
    private const string FilePathUnitySerialization = "SpecialSO_ByUnity.txt";
    public GameSettingsSpecialSO gameSettingsSpecialSO;
    [SerializeField] private TextMeshProUGUI tetxShowValues;
    [SerializeField] private List<Object> unityReferences = new List<Object>();
    private string _currentValues1, _currentValues2;

    #region StoreSpecialSOByUnityJSON
    public void Save()
    {
        ShowValues();
        //settingsSO.BeforeSerialize();
        Debug.Log("OdinCallsExample.SaveSpecialSOByUnity");
        OdinCallsExample.SaveSpecialSOByUnity(gameSettingsSpecialSO, FilePathUnitySerialization);
    }

    public void Load()
    {
        Debug.Log("OdinCallsExample.LoadSpecialSOByUnity");
        OdinCallsExample.LoadSpecialSOByUnity(gameSettingsSpecialSO, FilePathUnitySerialization);
        //settingsSO.AfterSerialize();
        ShowValues();
    }
    #endregion

    #region StoreSpecialSOByOdin
    public void SaveOdin()
    {
        ShowValues();
        Debug.Log("OdinCallsExample.SaveSpecialSOByOdin");
        OdinCallsExample.SaveSpecialSOByOdin(gameSettingsSpecialSO, FilePathOdinSerialization, ref unityReferences);
    }

    public void LoadOdin()
    {
        Debug.Log("OdinCallsExample.LoadSpecialSOByOdin");
        OdinCallsExample.LoadSpecialSOByOdin(gameSettingsSpecialSO, FilePathOdinSerialization, ref unityReferences);
        ShowValues();
    }
    #endregion

    public void Change()
    {
        ShowValues();
        gameSettingsSpecialSO.ChangeValues();
        ShowValues();
    }

    private void ShowValues()
    {
        (_currentValues1, _currentValues2) = gameSettingsSpecialSO.GetValues();
        tetxShowValues.text = _currentValues1 + _currentValues2;
        Debug.Log($"ShowValues() : {gameSettingsSpecialSO.LevelPlayer}");
        Debug.Log($"ShowValues() : {gameSettingsSpecialSO}");
    }

    private void Awake()
    {
        ShowValues();
    }
}
