using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public int openSpeed = 4;
    public int operatingTime = 1;
    enum ActionState { closed, opened, opening };
    ActionState actionState = ActionState.closed;

    private void doorOpening()
    {
        transform.position += Vector3.down * Time.deltaTime * openSpeed;
    }

    IEnumerator Delaying()
    {
        //Debug.Log("딜레이"+ actionState);
        while (true)
        {
            doorOpening();
            yield return new WaitForEndOfFrame();
        } 
    }
    IEnumerator StopDelaying()
    {
        yield return new WaitForSeconds(operatingTime);     
        StopCoroutine("Delaying");
        actionState = ActionState.opened;
        StartCoroutine("Erasing");
    }
    
    IEnumerator Erasing()
    {
        yield return new WaitUntil(() => actionState == ActionState.opened);
        Destroy(gameObject);
    }

    IEnumerator Open()
    {
        bool loop = true;
        while (loop)
        {
            switch (actionState)
            {
                case ActionState.closed:
                    yield return null;
                    actionState = ActionState.opening;
                    break;
                case ActionState.opening:
                    StartCoroutine("Delaying");
                    StartCoroutine("StopDelaying");
                    loop = false;
                    break;
            }
        }
    }

    
    public void Triggered()
    {
        switch (actionState)
        {
            case ActionState.closed:
                break;
            default:
                return;
        }

        StartCoroutine("Open");
        //actionState = ActionState.opening;

    }
}