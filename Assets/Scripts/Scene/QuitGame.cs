﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void OnApplicationQuit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
