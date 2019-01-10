using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeForest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        SoundManager.instance.PlayMusic("Forest");
    }
}
