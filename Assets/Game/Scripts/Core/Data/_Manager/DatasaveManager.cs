using UnityEngine;
using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System.IO;
using BayatGames.SaveGameFree.Encoders;
using Newtonsoft.Json;

public class DatasaveManager : MonoSingleton<DatasaveManager>
{
    private bool encode = true;
    private string password = "dungeon_labs";

    private List<IDatasave> datasaves = new List<IDatasave>();
    
    public void Init(Transform parent = null)
    {
        DataManager.Save = this;
        if (parent) transform.SetParent(parent);

#if UNITY_EDITOR
        encode = false;
#endif
        SaveGame.Encode = encode;
        SaveGame.EncodePassword = password;
        SaveGame.Serializer = new SaveGameJsonSerializer();
        LoadData();
    }
    public void LoadData()
    {
        //General = Load<GeneralSave>("General");
        Add();
    }
    private void Update()
    {
    }
    public void Add()
    {
        datasaves.Clear();
    }
    public TData Load<TData>(string key) where TData : BaseDatasave
    {
        TData data = SaveGame.Load<TData>(key, (TData)Activator.CreateInstance(typeof(TData), key));
        datasaves.Add(data);

        return data;
    }

    public void FixData()
    {
        foreach (var save in datasaves)
        {
            save.Fix();
        }
    }

    public void SaveData()
    {
        datasaves.Clear();
        Add();
        foreach (var save in datasaves)
        {
            save.Save();
        }

    }
    public List<IDatasave> GetSave()
    {
        return datasaves;
    }
    public void NextDay()
    {
        Debug.Log("Next Day");
        foreach (var save in datasaves)
        {
            save.NextDay();
            save.Save();
        }
    }

    public void OnLoaded()
    {
        foreach (var save in datasaves)
        {
            save.OnLoaded();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            SaveData();
        }
    }


    #region FIND FILE TO CUSTOM SAVE
    [Button]
    public string GetFileRawData(string fileName)
    {
        try
        {
            if (SaveGame.Exists(fileName))
            {
                var filePath = $"{Application.persistentDataPath}/Save/{fileName}";
                var data = System.IO.File.ReadAllText(filePath, SaveGame.DefaultEncoding);
                if (encode)
                {
                    var result = "";
                    var decoded = SaveGame.Encoder.Decode(data, password);
                    var stream = new System.IO.MemoryStream(Convert.FromBase64String(decoded), true);
                    using (var reader = new System.IO.StreamReader(stream, SaveGame.DefaultEncoding))
                    {
                        result = reader.ReadToEnd();
                    }

                    stream.Dispose();
                    return result;
                }

                return data;
            }

            return string.Empty;
        }
        catch (Exception e)
        {
            Debug.LogError($"Get file raw data Failed {e.Message} {e.StackTrace}\n{fileName}");
            return string.Empty;
        }
    }
    public T LoadFromRawData<T>(string rawData)
    {
        try
        {
            var stream = new System.IO.MemoryStream(SaveGame.DefaultEncoding.GetBytes(rawData));
            var saveObj = SaveGame.Serializer.Deserialize<T>(stream, SaveGame.DefaultEncoding);
            stream.Dispose();
            return saveObj;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load raw data Failed {e.Message} {e.StackTrace}\n{rawData}");
            return default(T);
        }
    }

    public void ClearData()
    {
        datasaves.Clear();
        SaveGame.Clear();
        LoadData();
    }

    #endregion
}