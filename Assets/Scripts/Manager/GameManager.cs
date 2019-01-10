using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using Lean.Pool;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    void Awake()
    {
        if (GameManager.instance != null)
        {
            return;
        }

        GameManager.instance = this;

        DatatableManager.InitDatatableManager();
        SoundManager.InitSoundManager(GetComponent<AudioSource>());
    }

    // Use this for initialization
    void Start ()
    {
        Debug.Log("GameManager 실행 완료!");
    }

    // Update is called once per frame
    //void Update ()
    //{
    //}
}
