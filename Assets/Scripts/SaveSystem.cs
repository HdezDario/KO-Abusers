using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    public static bool Save(object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/SaveFiles"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveFiles");
        }

        if (Load(Application.persistentDataPath + "/SaveFiles/SaveData.koabusers") != null)
            Delete(Application.persistentDataPath + "/SaveFiles/SaveData.koabusers");

        try
        {
            string path = Application.persistentDataPath + "/SaveFiles/SaveData.ko";

            FileStream file = File.Create(path);

            formatter.Serialize(file, saveData);

            file.Close();

            return true;
        }

        catch
        {
            return false;
        }
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }

        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return null;
        }
    }

    public static bool Delete(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            File.Delete(path);
            Debug.LogFormat("File Deleted Succesfully at {0}", path);
            return true;
        }

        catch
        {
            Debug.LogErrorFormat("Failed to delete file at {0}", path);
            return false;
        }  
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }
}
