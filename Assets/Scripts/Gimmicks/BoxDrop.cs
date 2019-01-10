using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDrop : MonoBehaviour {

    public GameObject Box;
    
    bool isDrop = false;

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isDrop)
        {
            Drop();
          
        }
    }
    void Drop()
    {
        isDrop = true;
        //GameObject.Find("tree03").GetComponent<AudioSource>().Play();
        Destroy(Box,1);
    }
}
