using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlay : MonoBehaviour
{
    public string aiPath;
    private Vector3 bgSize, bgPos;
    private ArrayList path;
    private Vector2 bounds;
    private int curNodeInPath;
    private Car controller;
    private GameObject[] cars;

    void Start()
    {
        // get background size and pos to calculate pixel to coordinate
        GameObject background = GameObject.FindGameObjectWithTag("Background");
        bgSize = background.GetComponent<Renderer>().bounds.size;
        bgPos = background.transform.position;
        // init the path
        bounds = new Vector2(1600, 1600);
        path = new ArrayList();
        string[] lines = System.IO.File.ReadAllLines(aiPath);
        foreach (string line in lines)
        {
            string[] nums = line.Split(' ');
            float x = float.Parse(nums[0]);
            float y = float.Parse(nums[1]);
            float offset = Config.Instance.AIOffset;
            x += Random.Range(-offset, offset);
            y += Random.Range(-offset, offset);
            path.Add(PixelToCoord(x, y));
        }
        curNodeInPath = 0;
        // get run script
        controller = GetComponent<Car>();
        // get all the cars
        cars = GameObject.FindGameObjectsWithTag("Car");
    }

    private Vector2 PixelToCoord(float x, float y)
    {
        return new Vector2(bgPos.x - bgSize.x/2 + bgSize.x / bounds.x * x, bgPos.y - bgSize.y/2 + bgSize.y / bounds.y * y);
    }

    private Vector2 Vec3ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    private void FixedUpdate()
    {
        // control movement
        int nextNodeInPath = (curNodeInPath + 1) % (path.Count - 1);
        Vector2 curPosition = Vec3ToVec2(transform.position);
        float distanceToCurNodeInPath = ((Vector2)path[curNodeInPath] - curPosition).magnitude;
        float distanceToNextNodeInPath = ((Vector2)path[nextNodeInPath] - curPosition).magnitude;
        if (distanceToNextNodeInPath < distanceToCurNodeInPath)
        {
            curNodeInPath = nextNodeInPath;
        }
        Vector2 facingDirection = controller.GetFacingDirection();
        Vector2 pathDirection = ((Vector2)path[(curNodeInPath + 1) % (path.Count - 1)] - curPosition).normalized;
        float steeringAngle = Vector2.SignedAngle(facingDirection, pathDirection);
        if (Mathf.Abs(steeringAngle) > Config.Instance.Steering)
        {

            if (steeringAngle > 0)
            {
                controller.SteerLeft();
            }
            else
            {
                controller.SteerRight();
            }

        }
        controller.GoForward();
        // use bonus
        switch (controller.currentBonus)
        {
            case Car.Bonus.RED:
                foreach (GameObject car in cars)
                {
                    if (car == gameObject)
                    {
                        continue;
                    }
                    Vector2 facingDir = controller.GetFacingDirection();
                    Vector2 diff = car.transform.position - transform.position;
                    if (Mathf.Abs(Vector2.SignedAngle(facingDir, diff)) < Config.Instance.AngleTriggerShoot)
                    {
                        controller.UseBonus();
                    }
                }
                break;
            case Car.Bonus.BLUE:
                controller.UseBonus();
                break;
            case Car.Bonus.GREEN:
                controller.UseBonus();
                break;
            case Car.Bonus.YELLOW:
                foreach (GameObject car in cars)
                {
                    if (car == gameObject)
                    {
                        continue;
                    }
                    Vector2 buttFacingDir = -controller.GetFacingDirection();
                    Vector2 diff = car.transform.position - transform.position;
                    if (Mathf.Abs(Vector2.SignedAngle(buttFacingDir, diff)) < Config.Instance.AngleTriggerOil)
                    {
                        controller.UseBonus();
                    }
                }
                break;
            default:
                break;
        }
    }
}
