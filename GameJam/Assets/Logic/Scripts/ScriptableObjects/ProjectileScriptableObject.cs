using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homebrew;

[CreateAssetMenu(fileName = "Projectile", menuName = "Projectile", order = 1)]
public class ProjectileScriptableObject : ScriptableObject
{
        [Foldout("Type", true)]

    [Tooltip("Type of projectile.")]
    public ProjectileType type;

        [Foldout("Values", true)]

    [Tooltip("Damage dealth on impact.")]
    public int damage;

    [Tooltip("Projectile velocity when shot (type = Projectile).")]
    public float speed;

    [Tooltip("Duration until the projectile gets destroyed (type = Projectile).")]
    public float destroyTime = 2.0f;

    [Tooltip("Length of the raycast to hit enemies (type = Raycast).")]
    public float raycastLength = 10.0f;

        [Foldout("Multiple Projectiles", true)]

    [Tooltip("Are there multiple projectiles?")]
    public bool shootMultipleProjectiles;

    [Tooltip("Multiple projectiles data.")]
    public MultipleProj multipleProjectiles;

    [System.Serializable]
    public class MultipleProj
    {
        [Range(2, 12)]
        [Tooltip("Amount of projectiles to shoot.")]
        public int projectileCount = 2;

        [Range(1.0f, 45.0f)]
        [Tooltip("Angle from weapon to shoot the multiple projectiles.")]
        public float projectileSpreadAngle = 30;
    }

        [Foldout("Projectile Object (Type = Projectile)", true)]

    [Tooltip("Game object of the projectile.")]
    public GameObject projectilePrefab;

        [Foldout("Trail (Type = Raycast)", true)]

    [Tooltip("Do we draw a projectile trail when shot?")]
    public bool drawTrail;

    [Tooltip("Projectile trail data.")]
    public ProjTrail trail;

    [System.Serializable]
    public class ProjTrail
    {
        [Tooltip("Colour of the trail.")]
        public Color trailColor = Color.yellow;
    }

        [Foldout("Effects", true)]

    [Tooltip("Do we apply effects to hit entities?")]
    public bool applyEffects;

    [Tooltip("Effects to apply to hit entities.")]
    public EffectScriptableObject[] effectsToApply;
}

public enum ProjectileType
{
    [Tooltip("Raycast hit detection.")]
    Raycast,

    [Tooltip("Physics based projectile object.")]
    Projectile
}