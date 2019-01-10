using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Util;

public class SubPlayer : MonoBehaviour
{

    const int CHILDID_MODEL = 0;

    public int Speed;
    public AnimationClip walk;
    public Animation anim;

    private int count = 0;


    // Use this for initialization
    void Start()
    {
        anim = transform.GetChild(CHILDID_MODEL).GetComponent<Animation>();
        anim.clip = walk;
        anim.Play();

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "trigger")
        {
            Vector3 newVec = new Vector3
            (
                ((other.gameObject.name[7] - '0') % 2) * 100,
                transform.position.y,
                transform.position.z
            );

            transform.position = newVec;
        }
    }

}
