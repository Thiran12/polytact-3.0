using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Time.timeScale = 3;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
         Time.timeScale = 1;
        }
    }
}
