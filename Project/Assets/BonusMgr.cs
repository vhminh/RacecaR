using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusMgr : MonoBehaviour
{
    public string bonusFilePath;
    public GameObject red;
    public GameObject green;
    public GameObject yellow;
    public GameObject blue;
    private Vector3 bgSize, bgPos;
    private List<Vector3Int> bonuses;
    private List<GameObject> bonusObjects;
    private List<bool> isActive;
    private List<int> coolDown;

    void Start()
    {
        // get background size and pos to calculate pixel to coordinate
        GameObject background = GameObject.FindGameObjectWithTag("Background");
        bgSize = background.GetComponent<Renderer>().bounds.size;
        bgPos = background.transform.position;
        // read the bonuses from file
        bonuses = new List<Vector3Int>();
        string[] lines = System.IO.File.ReadAllLines(bonusFilePath);
        foreach (string line in lines)
        {
            string[] nums = line.Split(';');
            int type = int.Parse(nums[0]);
            int x = int.Parse(nums[1]);
            int y = int.Parse(nums[2]);
            bonuses.Add(new Vector3Int(x, 1600 - y, type));
        }
        // display bonus to screen
        bonusObjects = new List<GameObject>();
        foreach (Vector3Int bonus in bonuses)
        {
            bonusObjects.Add(InstantiateBonus(bonus));
        }
        // init cooldown array
        isActive = new List<bool>(new bool [bonuses.Count]);
        for (int i = 0; i < isActive.Count; ++i)
        {
            isActive[i] = true;
        }
        coolDown = new List<int>(new int [bonuses.Count]);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < bonuses.Count; ++i)
        {
            if (isActive[i] && (bonusObjects[i] == null || !bonusObjects[i].activeSelf))
            {
                isActive[i] = false;
                coolDown[i] = Config.Instance.BonusCoolDownTime;
            }
            else if (!isActive[i])
            {
                --coolDown[i];
                if (coolDown[i] <= 0)
                {
                    isActive[i] = true;
                    bonusObjects[i] = InstantiateBonus(bonuses[i]);
                }
            }
        }
    }

    private GameObject InstantiateBonus(Vector3Int bonus)
    {
        int type = bonus.z;
        Object prefab;
        switch (type)
        {
            case 0:
                prefab = red;
                break;
            case 1:
                prefab = blue;
                break;
            case 2:
                prefab = green;
                break;
            case 3:
                prefab = yellow;
                break;
            default:
                prefab = red;
                break;
        }
        return (GameObject) Instantiate(prefab, PixelToCoord(bonus.x, bonus.y), Quaternion.identity);
    }

    private Vector2 PixelToCoord(float x, float y)
    {
        return new Vector2(bgPos.x - bgSize.x / 2 + bgSize.x / 1600 * x, bgPos.y - bgSize.y / 2 + bgSize.y / 1600 * y);
    }
}
