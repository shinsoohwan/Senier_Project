using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheck : MonoBehaviour
{

    public bool hazardcheck;

    // Use this for initialization
    void Start()
    {
        hazardcheck = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)  // 플레이어가 hazard 랑 충돌이 시작되면 hazardcheck 를 false로 바꾼다
    {
        if (other.gameObject.tag == "hazard" && hazardcheck == true)
        {
            hazardcheck = false;

           
        }

    }

    public void OnTriggerExit(Collider other)   // 플레이어가 hazard 랑 충돌이 끝나면 hazardcheck 를 false로 바꾼다
    {
        if (other.gameObject.tag == "hazard" && hazardcheck == false)
        {
            hazardcheck = true;

            
        }

    }

}
