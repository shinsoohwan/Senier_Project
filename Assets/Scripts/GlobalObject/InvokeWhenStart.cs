using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeWhenStart : MonoBehaviour {

    [SerializeField] private UnityEvent m_onStart = new UnityEvent();

    // Use this for initialization
    void Start ()
    {
        m_onStart.Invoke();
    }
}
