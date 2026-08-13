using UnityEngine;

public struct UpgradeData
{
    public float Cooldown;
    public float Duration;
    public float LastExecute;
    public bool IsActive;
    public bool IsReady(){return Cooldown + LastExecute < Time.time;}
    public UpgradeData(float duration, float cooldown)
    {
        IsActive = false;
        Duration = duration;
        Cooldown = cooldown;
        LastExecute = 0;
    }
}