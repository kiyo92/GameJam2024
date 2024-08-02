using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homebrew;

[CreateAssetMenu(fileName = "Effect", menuName = "Effect", order = 1)]
public class EffectScriptableObject : ScriptableObject
{
        [Foldout("Type", true)]

    [Tooltip("Type of effect.")]
    public EffectType effectType;

        [Foldout("Display", true)]

    [Tooltip("Effect name displayed on UI elements.")]
    public string displayName;

    [Tooltip("Effect description displayed on UI elements.")]
    public string description;

        [Foldout("Values", true)]

    [Tooltip("Duration of effect. Stops after duration.")]
    public float duration;

        [Foldout("Damage Over Time", true)]

    [Tooltip("Damage the affected entity over time?")]
    public bool damageOverTime;

    [Tooltip("Damage over time options.")]
    public DamageOverTime damageOverTimeOptions;

        [Foldout("Explosive", true)]

    [Tooltip("Explosive?")]
    public bool explosive;

    [Tooltip("Explosive options.")]
    public ExplosiveOptions explosiveOptions;

        [Foldout("Stat Change", true)]

    [Tooltip("Temporarily change entity's stat for the duration of the effect.")]
    public bool tempStatChange;

    [Tooltip("Temp stat change options.")]
    public TempStatChange tempStatChangeOptions;

        [Foldout("Visual", true)]

    [Tooltip("Particle effect put on entity for the duration of the effect.")]
    public GameObject effectParticle; 
}

public enum EffectType
{
    DamageOverTime,
    Explosive,
    TempStatChange
}

[System.Serializable]
public class DamageOverTime
{
    [Tooltip("Amount of damage to deal.")]
    public int damage;

    [Tooltip("Rate at which damage is dealt.")]
    public float damageRate;
}

[System.Serializable]
public class ExplosiveOptions
{
    [Tooltip("Damage dealt.")]
    public int explosiveDamage;

    [Tooltip("Range of explosive.")]
    public float explosiveRange;

    [Tooltip("Force applied to rigidbodies in explosive range.")]
    public float explosiveForce;

    [Tooltip("Does the damage drop off from 100% to 0% across the explosive range?")]
    public bool explosiveDamageDropOff;

    public ExplosiveOptions (int damage, float range, float force)
    {
        explosiveDamage = damage;
        explosiveRange = range;
        explosiveForce = force;
    }
}

[System.Serializable]
public class TempStatChange
{
    [Tooltip("Stat to change.")]
    public StatType statToChange;

    [Tooltip("Modifier applied to entity's stat (0.5 = half, 1.0f = normal, 2.0 = double, etc).")]
    public float statModifier = 1.0f;
}