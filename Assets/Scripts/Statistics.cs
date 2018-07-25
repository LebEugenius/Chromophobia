using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Statistics
{
    public int RecordLevel;
    public int TotalColoredsDestroyed;
    public int AgroColoredsDestroyed;

    public void Load()
    {
        RecordLevel = PlayerPrefs.GetInt("RecordLevel");
        TotalColoredsDestroyed = PlayerPrefs.GetInt("TotalColoredsDestroyed");
        AgroColoredsDestroyed = PlayerPrefs.GetInt("AgroColoredsDestroyed");
    }

    public void Save()
    {
        PlayerPrefs.SetInt("RecordLevel", RecordLevel);
        PlayerPrefs.SetInt("TotalColoredsDestroyed", TotalColoredsDestroyed);
        PlayerPrefs.SetInt("AgroColoredsDestroyed", AgroColoredsDestroyed);
    }

    public void UploadToServer()
    {
        Application.ExternalCall("kongregate.stats.submit", "Record", RecordLevel);
        Application.ExternalCall("kongregate.stats.submit", "TotalColoreds", TotalColoredsDestroyed);
        Application.ExternalCall("kongregate.stats.submit", "AgroColoreds", AgroColoredsDestroyed);
    }
}
