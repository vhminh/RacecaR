using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    public GameObject _bomb;
    public GameObject _oil;
    public GameObject _shield;
    public GameObject _trail;
    public Image _currentBonusView;
    public Text _lapCountView;
    public Text _resultView;
    public Sprite _bombIcon;
    public Sprite _oilIcon;
    public Sprite _shieldIcon;
    public Sprite _nitroIcon;
    private Rigidbody2D rb;
    private bool isOver = false;
    private float rot = 0;
    private float v = 0;
    private float h = 0;
    private bool useBonus = false;
    public int lapCount { get; private set; } = 1;
    public int curLap = 1;
    private int curCheckpoint = 0;

    public enum Bonus
    {
        NONE, RED, BLUE, GREEN, YELLOW
    }
    public Bonus currentBonus { get; private set; } = Bonus.NONE;

    private GameObject shield;
    private GameObject trail;
    public bool hasShield { get; private set; } = false;


    private int shieldRemaining = -1;
    private int nitroRemaining = -1;
    private int spinRemaining = -1;

    void Start()
    {
        trail = Instantiate(_trail, transform.position, Quaternion.identity) as GameObject;
        rb = GetComponent<Rigidbody2D>();
        if (IsPlayer())
        {
            _currentBonusView.enabled = false;
        }
    }

    public bool IsPlayer()
    {
        return GetComponent<Play>() != null;
    }

    public bool AcceptInput()
    {
        return spinRemaining <= 0 && !isOver;
    }

    public void Stop()
    {
        v = h = 0;
    }

    public void GoForward()
    {
        if (AcceptInput())
        {
            v = 1;
        }
    }

    public void GoBackward()
    {
        if (AcceptInput())
        {
            v = -1;
        }
    }

    public void SteerLeft()
    {
        if (AcceptInput())
        {
            h = -1;
        }
    }

    public void SteerRight()
    {
        if (AcceptInput())
        {
            h = 1;
        }
    }

    public void UseBonus()
    {
        if (AcceptInput())
        {
            useBonus = true;
        }
    }

    public Vector2 GetFacingDirection()
    {
        return (Quaternion.AngleAxis(rot, Vector3.forward) * Vector2.up).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        Bonus bonus;
        switch (collider.tag)
        {
            case "BonusRed":
                bonus = Bonus.RED;
                break;
            case "BonusBlue":
                bonus = Bonus.BLUE;
                break;
            case "BonusGreen":
                bonus = Bonus.GREEN;
                break;
            case "BonusYellow":
                bonus = Bonus.YELLOW;
                break;
            case "Bomb":
                BonusProp bomb = collider.GetComponent<BonusProp>();
                if (bomb.owner != this)
                {
                    if (!hasShield && collider.gameObject != null && collider.gameObject.activeSelf)
                    {
                        spinRemaining = Config.Instance.SpinDuration;
                        Destroy(collider);
                    }
                }
                return;
            case "Oil":
                BonusProp oil = collider.GetComponent<BonusProp>();
                if (oil.owner != this)
                {
                    if (!hasShield && collider.gameObject != null && collider.gameObject.activeSelf)
                    {
                        spinRemaining = Config.Instance.SpinDuration;
                        Destroy(collider);
                    }
                }
                return;
            case "Finish line":
                onCheckpointReached(0);
                return;
            case "Checkpoint1":
                onCheckpointReached(1);
                return;
            case "Checkpoint2":
                onCheckpointReached(2);
                return;
            default:
                return;
        }
        if (currentBonus == Bonus.NONE)
        {
            currentBonus = bonus;
            Destroy(collider);
            if (IsPlayer())
            {
                _currentBonusView.enabled = true;
                _currentBonusView.sprite = GetIconFromBonus(bonus);
            }
        }
    }

    private void onCheckpointReached(int idx)
    {
        if (idx == (curCheckpoint + 1) % 3)
        {
            curCheckpoint = (curCheckpoint + 1) % 3;
            if (curCheckpoint == 0)
            {
                ++curLap;
            }
        }
        else if (curCheckpoint == (idx + 1) % 3)
        {
            if (curCheckpoint == 0)
            {
                --curLap;
            }
        }
        lapCount = Mathf.Max(lapCount, curLap);
        if (lapCount > Config.Instance.Laps)
        {
            isOver = true;
            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            int position = 0;
            foreach (GameObject car in cars)
            {
                Car c = car.GetComponent<Car>();
                if (c.lapCount > Config.Instance.Laps)
                {
                    ++position;
                }
            }
            string[] orders = { "1st", "2nd", "3rd", "4th" };
            if (_resultView != null)
            {
                _resultView.text = "You are at the " + orders[position - 1] + " place";
            }
        }
    }

    private Sprite GetIconFromBonus(Bonus b)
    {
        switch (b)
        {
            case Bonus.RED:
                return _bombIcon;
            case Bonus.BLUE:
                return _shieldIcon;
            case Bonus.GREEN:
                return _nitroIcon;
            case Bonus.YELLOW:
                return _oilIcon;
        }
        return null;
    }

    void FixedUpdate()
    {
        if (isOver)
        {
            Stop();
        }
        // update bonus
        if (shieldRemaining > 0)
        {
            --shieldRemaining;
        }
        if (nitroRemaining > 0)
        {
            --nitroRemaining;
        }
        if (spinRemaining > 0)
        {
            --spinRemaining;
        }
        // check bonuses
        if (useBonus)
        {
            float carSize = GetComponent<Renderer>().bounds.size.y;
            switch (currentBonus)
            {
                case Bonus.RED:
                    {
                        GameObject bomb = Instantiate(_bomb, transform.position + transform.up * carSize / 2, Quaternion.identity) as GameObject;
                        BonusProp prop = bomb.GetComponent<BonusProp>();
                        prop.owner = this;
                        bomb.GetComponent<Rigidbody2D>().velocity = transform.up * Config.Instance.FireBombForce;
                        prop.RemainingTime = int.MaxValue;
                        break;
                    }
                case Bonus.BLUE:
                    {
                        if (shield == null)
                        {
                            shield = Instantiate(_shield, transform.position, Quaternion.identity) as GameObject;
                        }
                        shieldRemaining = Mathf.Max(shieldRemaining, 0) + Config.Instance.ShieldDuration;
                        break;
                    }
                case Bonus.GREEN:
                    nitroRemaining = Config.Instance.NitroDuration;
                    break;
                case Bonus.YELLOW:
                    {
                        GameObject oil = Instantiate(_oil, transform.position - transform.up * carSize / 2, Quaternion.identity) as GameObject;
                        BonusProp prop = oil.GetComponent<BonusProp>();
                        prop.owner = this;
                        prop.RemainingTime = Config.Instance.OilDuration;
                    }
                    break;
            }
            if (IsPlayer())
            {
                _currentBonusView.enabled = false;
            }
            currentBonus = Bonus.NONE;
            useBonus = false;
        }
        // apply bonus
        float acceleration;
        if (nitroRemaining > 0)
        {
            acceleration = Config.Instance.Acceleration + Config.Instance.AdditionalNitroAcc;
            if (!trail.activeSelf)
            {
                trail.SetActive(true);
            }
        }
        else
        {
            acceleration = Config.Instance.Acceleration;
            trail.SetActive(false);
        }
        hasShield = shieldRemaining > 0;
        if (shieldRemaining <= 0 && shield != null)
        {
            Destroy(shield);
            shield = null;
        }
        if (spinRemaining > 0)
        {
            transform.Rotate(new Vector3(0, 0, Config.Instance.NumSpinRounds * 360 / Config.Instance.SpinDuration));
        }
        // car run
        rot -= h * Config.Instance.Steering * v;
        rb.MoveRotation(rot);
        Vector2 facingDirection = GetFacingDirection();
        float speed = rb.velocity.magnitude;
        if (v < 0 || (v == 0 && Vector2.Angle(rb.velocity, facingDirection) > 90))
        {
            speed *= -1;
        }
        rb.velocity = facingDirection * (speed * (1 - Config.Instance.Fiction) + acceleration * v);
        Stop();
    }

    private void Update()
    {
        if (IsPlayer() && _lapCountView != null && !isOver)
        {
            _lapCountView.text = "Lap: " + lapCount + "/" + Config.Instance.Laps;
        }
        float carSize = transform.GetComponent<Renderer>().bounds.size.y;
        trail.transform.position = transform.position - transform.up * carSize / 2;
        trail.transform.rotation = transform.rotation;
        if (shield != null)
        {
            shield.transform.position = transform.position;
        }
    }
}
