using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    public GameObject tileSet_A;    //1번째 지형
    public GameObject tileSet_B;    // 2번째 지형

    public GameObject hazard_A;     // 1번지형에서 닿으면 스킬발동 안되는곳
    public GameObject hazard_B;     // 2번지형에서 닿으면 스킬발동 안되는곳
    

    void Start()
    {
        tileSet_A = GameObject.FindWithTag("tileSet_A");
        tileSet_B = GameObject.FindWithTag("tileSet_B");


        tileSet_A.SetActive(true);
        tileSet_B.SetActive(false);

        hazard_A.SetActive(false);
        hazard_B.SetActive(true);
    }

    void Update()
    {

    }

    public void TimeSkill()
    {

        SkillCheck skillCheck = GameObject.Find("Player").GetComponent<SkillCheck>();
        bool check = skillCheck.hazardcheck;



        if (check == true)
        {
            Switch();
            GameObject.Find("Audio Source_Skill").GetComponent<AudioSource>().Play();
        }
        else if (check == false) {
            SoundManager.instance.PlaySound("CannotChangeTime");
        }

    }

    void Switch()
    { // 실제로 바꾸는 함수
        if (tileSet_A.activeSelf == true)
        {   // A -> B
            tileSet_B.SetActive(true);
            tileSet_A.SetActive(false);

            hazard_B.SetActive(true);
            hazard_A.SetActive(false);

            
        }
        else if (tileSet_B.activeSelf == true)
        {   // B -> A
            tileSet_A.SetActive(true);
            tileSet_B.SetActive(false);

            hazard_A.SetActive(true);
            hazard_B.SetActive(false);

            

        }
    }


}
