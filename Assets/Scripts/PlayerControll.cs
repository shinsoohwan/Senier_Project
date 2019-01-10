using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]

public class PlayerControll : MonoBehaviour {

	public float moveSpeed = 5.0f;
	public Transform Player;
	public Anims anims;
	
	private Animation anim;
	private Transform targetPosition = null;
	const int CHILDID_MODEL = 0;
	
	
	public class Anims{		
		public AnimationClip idle;
		public AnimationClip walk;

	}
	
	void Awake(){
        anim = transform.GetChild(CHILDID_MODEL).GetComponent<Animation>();
		
	}
	
	// Use this for initialization
	void Start () {	
		targetPosition.position = Player.position;
		anim.clip = anims.idle;
		anim.Play ();
		
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(targetPosition.position, transform.position) >= 0.001f){
			anim.CrossFade ("walk");
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime , Space.Self);
		}	
		else{
			anim.CrossFade("idle");
		}
		
	}
	
	public void LD_Button(){
		Player.eulerAngles = new Vector3(0,180,0);
		targetPosition.position = new Vector3(targetPosition.position.x,targetPosition.position.y,targetPosition.position.z+1);
		Debug.Log(targetPosition.position);
		
	}
	public void LU_Button(){
		Player.eulerAngles = new Vector3(0,270,0);
		targetPosition.position = new Vector3(targetPosition.position.x,targetPosition.position.y,targetPosition.position.z+1);
		
	}
	public void RD_Button(){
		Player.eulerAngles = new Vector3(0,90,0);
		targetPosition.position = new Vector3(targetPosition.position.x,targetPosition.position.y,targetPosition.position.z+1);
		Debug.Log(targetPosition.position);
	}
	public void RU_Button(){
		Player.eulerAngles = new Vector3(0,0,0);
		targetPosition.position = new Vector3(targetPosition.position.x,targetPosition.position.y,targetPosition.position.z+1);
		Debug.Log(targetPosition.position);
	}

}

