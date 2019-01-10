using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeMountain : MonoBehaviour {

	void Start ()
    {
        SoundManager.instance.PlayMusic("Mountain");
    }
}
