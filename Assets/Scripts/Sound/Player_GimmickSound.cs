using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GimmickSound : MonoBehaviour {

    public AudioClip door;
    public AudioClip bridge;
    public AudioClip end;

    AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "switch")
        {
            audio.PlayOneShot(door);
        }
        else if (other.tag == "bridge")
        {
            audio.PlayOneShot(bridge);
        }
        else if (other.tag == "End")
        {
            audio.PlayOneShot(end);
        }
    }
}
