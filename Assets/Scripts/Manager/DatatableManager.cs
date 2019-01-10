using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class DatatableManager
{
    public class DatatableBaseInfo
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }

        public virtual string Dump()
        {
            string result = "ClassID:" + ClassID + ", ClassName:" + ClassName;

            return result;
        }
    }

    public static DatatableManager instance;

    public static void InitDatatableManager()
    {
        if (DatatableManager.instance != null)
            Debug.Log("DatatableManager already initialized");

        DatatableManager.instance = new DatatableManager();
    }

    DatatableManager()
    {

    }

    public T LoadJson<T>(string datatableName)
    {
        TextAsset text = Resources.Load<TextAsset>(datatableName);

        if (text != null)
        {
            //return = JsonUtility.FromJson<T>(File.ReadAllText(filePath)); //Json .Net for Unity를 못 쓸 경우 대체제
            
            return JsonConvert.DeserializeObject<T>(text.text);
        }
        else
        {
            Debug.LogError("Cannot load " + datatableName + "!");
            return default(T);
        }
    }

    public Dictionary<string, T> LoadDatatableByClassName<T>(string datatableName) where T : DatatableBaseInfo
    {
        Dictionary<string, T> result = new Dictionary<string, T>();
        List<T> list = LoadJson<List<T>>(datatableName);
        foreach (T t in list)
        {
            result.Add(t.ClassName, t);
        }

        return result;
    }

    public Dictionary<int, T> LoadDatatableByClassID<T>(string datatableName) where T : DatatableBaseInfo
    {
        Dictionary<int, T> result = new Dictionary<int, T>();
        List<T> list = LoadJson<List<T>>(datatableName);
        foreach (T t in list)
        {
            result.Add(t.ClassID, t);
        }

        return result;
    }

}
