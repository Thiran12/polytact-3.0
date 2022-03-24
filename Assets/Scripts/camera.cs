using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camera : MonoBehaviour
{
    public CinemachineFreeLook cinemachineFreeLook;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Debug.Log("Pressed secondary button.");
            cinemachineFreeLook.m_XAxis.m_MaxSpeed = 300;
        }
        else
        {
            cinemachineFreeLook.m_XAxis.m_MaxSpeed = 0;
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            cinemachineFreeLook.m_YAxis.m_MaxSpeed = 100;
        }
       
    }
}
