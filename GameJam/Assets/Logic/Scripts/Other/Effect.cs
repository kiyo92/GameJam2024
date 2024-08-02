using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [Header("Type")]
    public EffectType effectType;

    [Header("Display")]
    public string displayName;
    public string description;

    [Header("Values")]
    public float duration;

    [Header("Damage Over Time")]
    public bool damageOverTime;
    public DamageOverTime damageOverTimeOptions;

    [Header("Explosive")]
    public bool explosive;
    public ExplosiveOptions explosiveOptions;

    [Header("Stat Change")]
    public bool tempStatChange;
    public TempStatChange tempStatChangeOptions;

    [Header("Visual")]
    public GameObject effectParticle;

    private GameObject hitEntity;
    
    public Effect (EffectScriptableObject effect, GameObject hitEntity)
    {
        effectType = effect.effectType;

        displayName = effect.displayName;
        description = effect.description;

        duration = effect.duration;

        damageOverTime = effect.damageOverTime;
        damageOverTimeOptions = effect.damageOverTimeOptions;

        explosive = effect.explosive;
        explosiveOptions = effect.explosiveOptions;

        tempStatChange = effect.tempStatChange;
        tempStatChangeOptions = effect.tempStatChangeOptions;

        effectParticle = effect.effectParticle;

        this.hitEntity = hitEntity;

        if(damageOverTime)
            DamageOverTime();

        if(explosive)
            Explode();

        if(tempStatChange)
            TempStatChange();
    }

    //Damage the hit entity over time.
    void DamageOverTime ()
    {
        if(hitEntity.tag == "Enemy")
            hitEntity.GetComponent<Enemy>().DamageOverTime(damageOverTimeOptions.damage, damageOverTimeOptions.damageRate, duration, effectParticle ? effectParticle : null);
    }

    //Explosive
    void Explode ()
    {
        Explosion.Explode(explosiveOptions, hitEntity.transform.position);
    }

    //Temporarily change hit entity's stat.
    void TempStatChange ()
    {
        if(hitEntity.tag == "Enemy")
            hitEntity.GetComponent<Enemy>().TempStatChange(tempStatChangeOptions.statToChange, tempStatChangeOptions.statModifier, duration);
    }
}

public enum StatType
{
    MoveSpeed
}