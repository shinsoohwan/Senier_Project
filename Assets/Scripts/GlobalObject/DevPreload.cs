using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevPreload : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        GameObject check = GameObject.Find("GameManager");
        if (check == null)
        {
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        }
    }

    void Update()
    {
        Scene loadingScene = SceneManager.GetSceneByName("Loading");
        if (!loadingScene.isLoaded)
            return;

        SceneManager.UnloadSceneAsync(loadingScene);
        Destroy(this);
    }
}