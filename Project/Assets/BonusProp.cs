using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusProp : MonoBehaviour
{
    public Car owner { get; set; }
    public int RemainingTime { get; set; } = int.MaxValue;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (gameObject != null && gameObject.activeSelf)
        {
            --RemainingTime;
            if (RemainingTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
