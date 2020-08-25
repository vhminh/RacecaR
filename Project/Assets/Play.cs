using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    private Car runScript;
    void Start()
    {
        runScript = GetComponent<Car>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            runScript.UseBonus();
        }
    }

    private void FixedUpdate()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        if (v > 0)
        {
            runScript.GoForward();
        }
        else if (v < 0)
        {
            runScript.GoBackward();
        }
        if (h > 0)
        {
            runScript.SteerRight();
        }
        else if (h < 0)
        {
            runScript.SteerLeft();
        }
    }
}
