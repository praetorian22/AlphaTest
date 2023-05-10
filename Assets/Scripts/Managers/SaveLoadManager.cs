using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager
{
    public Action<int> loadDataEvent;
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            loadDataEvent?.Invoke(data.score);
            Debug.Log("Game data loaded!");
        }
        else
        {
            loadDataEvent?.Invoke(0);
            Debug.LogError("There is no save data!");
        }
            
    }

    public void SaveData(int record)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        SaveData data = new SaveData();
        data.score = record;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }
}
[Serializable]
public class SaveData
{
    public int score;
}