using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DestinationController:MonoBehaviour
{
	void Start()
	{//THIS JUST SET THE CAERA POSITION
		if (unitToControl)
		{
			transform.position=unitToControl.position+Vector3.up*50-Vector3.forward*18;
			Vector3 _dir=(unitToControl.position-transform.position).normalized;
			transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(_dir),Time.deltaTime*60);
		}
	}
	
	public Transform unitToControl;//UNIT TO CONTROL MUST HAVE CORVOPATHFINDER AND UNITPATHFINDER SCRIPT
	public void Update()
	{
		//move the camera with keyboard
		transform.position+=Vector3.forward*15*Time.deltaTime*Input.GetAxis("Vertical")+Vector3.right*15*Time.deltaTime*Input.GetAxis("Horizontal")+transform.forward*Input.GetAxis("Mouse ScrollWheel")*Time.deltaTime*5;
		if (unitToControl)//if unit to control is setted
		{
			if (Input.GetMouseButtonDown(0))//if left press mouse button 
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);//generate a ray from the mouse position to the ground
				RaycastHit hit;
				if (Physics.Raycast (ray,out hit)) //send the ray
				{
					unitToControl.GetComponent<UnitPathfinder>().goTo(hit.point);//order to the unit to go to the ray hit
				}
			}
			else if (Input.GetMouseButtonDown(1))//if press right mouse button 
				unitToControl.GetComponent<UnitPathfinder>().stop();//order to the unit to go to the ray hit
			//easy, right?
		}
	}
}