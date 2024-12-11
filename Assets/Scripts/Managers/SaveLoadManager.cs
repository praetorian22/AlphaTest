using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager
{
    public Action<List<int>> loadDataEvent;
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            loadDataEvent?.Invoke(data.scores);
            Debug.Log("Game data loaded!");
        }
        else
        {
            List<int> scores = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                scores.Add(0);
            }
            loadDataEvent?.Invoke(new List<int>(scores));
            Debug.LogError("There is no save data!");
        }
            
    }

    public void SaveData(List<int> records)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        SaveData data = new SaveData();
        data.scores = new List<int>(records);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }
}
[Serializable]
public class SaveData
{
    public List<int> scores;
}