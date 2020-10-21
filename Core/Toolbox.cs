// Written by Parker Staszkiewicz
// Based on the Toolbox concept detailed by Ramy Daghstani

using System.Collections.Generic;
using UnityEngine;

public class Toolbox : MonoBehaviour
{
    private Dictionary<string, Component> tools;

    public static Toolbox Instance
    {
        get { return GetInstance(); }
        set { _instance = value; }
    }

    private static Toolbox _instance;

    private static Toolbox GetInstance()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("Toolbox");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<Toolbox>();
        }

        return _instance;
    }

    private void Awake()
    {
        tools = new Dictionary<string, Component>();

        // Create required tools on awake
        AddObject<LevelManager>("Level Manager");
    }

    public ObjType GetObject<ObjType>(string objName) where ObjType : Component
    {
        return tools[objName] as ObjType;
    }

    public ObjType AddObject<ObjType>(string objName) where ObjType : Component
    {
        var tool = new GameObject(objName);
        tool.transform.SetParent(transform);
        ObjType obj = tool.AddComponent<ObjType>();
        tools.Add(objName, obj);
        return obj;
    }
}
