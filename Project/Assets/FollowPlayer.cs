using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private float camXMin, camXMax, camYMin, camYMax;

    void Start()
    {
        GameObject background = GameObject.FindGameObjectWithTag("Background");
        float camHeight = 2 * Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        float bgWidth = background.GetComponent<Renderer>().bounds.size.x;
        float bgHeight = background.GetComponent<Renderer>().bounds.size.y;
        camXMin = background.transform.position.x - (bgWidth - camWidth) / 2;
        camXMax = background.transform.position.x + (bgWidth - camWidth) / 2;
        camYMin = background.transform.position.y - (bgHeight - camHeight) / 2;
        camYMax = background.transform.position.y + (bgHeight - camHeight) / 2;
    }

    private void FixedUpdate()
    {
        float camX = player.transform.position.x;
        camX = Mathf.Min(Mathf.Max(camX, camXMin), camXMax);
        float camY = player.transform.position.y;
        camY = Mathf.Min(Mathf.Max(camY, camYMin), camYMax);
        transform.position = new Vector3(camX, camY, transform.position.z);
    }
}
