using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    private static Config _instance;
    
    public static Config Instance { get { return _instance; } }
    // Acceleration
    [SerializeField]
    private float _acceleration;
    public float Acceleration { get { return _acceleration; } }
    // Steering angle
    [SerializeField]
    private float _steering;
    public float Steering { get { return _steering; } }
    // Fiction
    [SerializeField]
    private float _fiction;
    public float Fiction { get { return _fiction; } }
    // Bonus cooldown
    [SerializeField]
    private int _bonusCoolDownTime; // in frames
    public int BonusCoolDownTime { get { return _bonusCoolDownTime; } }
    // Shield duration
    [SerializeField]
    private int _shieldDuration; // in frames
    public int ShieldDuration { get { return _shieldDuration; } }
    // Nitro duration
    [SerializeField]
    private int _nitroDuration; // in frames
    public int NitroDuration { get { return _nitroDuration; } }
    // Nitro speed
    [SerializeField]
    private float _additionalNitroAcc;
    public float AdditionalNitroAcc { get { return _additionalNitroAcc; } }
    // Oil duration
    [SerializeField]
    private int _oilDuration; // in frames
    public int OilDuration { get { return _oilDuration; } }
    // AI offset
    [SerializeField]
    private int _aiOffset; // offset of nodes in the path with the center of the road
    public int AIOffset { get { return _aiOffset; } }
    // Bomb fire force
    [SerializeField]
    private float _fireBombForce;
    public float FireBombForce { get { return _fireBombForce; } }
    // Spin duration
    [SerializeField]
    private int _spinDuration;
    public int SpinDuration { get { return _spinDuration; } }
    // Num spin rounds
    [SerializeField]
    private int _nSpinRounds;
    public int NumSpinRounds { get { return _nSpinRounds; } }
    // The angle to trigger bot shoot when other car is ahead
    [SerializeField]
    private float _angleTriggerShoot;
    public float AngleTriggerShoot { get { return _angleTriggerShoot; } }
    // The angle to trigger bot oil when other car is behind
    [SerializeField]
    private float _angleTriggerOil;
    public float AngleTriggerOil { get { return _angleTriggerOil; } }
    // Number of laps
    [SerializeField]
    private int _laps;
    public int Laps { get { return _laps; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }
}
