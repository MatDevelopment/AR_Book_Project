using UnityEngine;

public class SUI_particleSystemsHolder : MonoBehaviour
{
    [Header("Exhaust Particle settings")]
    [SerializeField] private ParticleSystem exhaustBigFlame;
    [SerializeField] private ParticleSystem exhaustSmallFlame;

    [SerializeField] private float smallFlameMaxSpeed = 2f;
    [SerializeField] private float bigFlameMaxSpeed = 5.75f;

   

    public void SetBigFlameSpeed(float speed)
    {
        var main = exhaustBigFlame.main;
        main.startSpeed = (float)ExtensionMethods.Remap(speed,0, 1, 0, bigFlameMaxSpeed);
    }
    public void SetSmallFlameSpeed(float speed)
    {
        var main = exhaustSmallFlame.main;
        main.startSpeed = (float)ExtensionMethods.Remap(speed, 0, 1, 0, smallFlameMaxSpeed);
    }

    public void SetFlamesToZero()
    {
        SetBigFlameSpeed(0);
        SetSmallFlameSpeed(0);
    }
}

