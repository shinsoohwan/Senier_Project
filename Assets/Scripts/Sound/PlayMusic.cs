using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour {

    public string musicName;

	// Use this for initialization
	void Start ()
    {
        SoundManager.instance.PlayMusic(musicName);
	}
}
