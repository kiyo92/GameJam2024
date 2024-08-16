using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int curHp;                   //Enemy's current health.
    public int maxHp;                   //Enemy's maximum health.
    public float moveSpeed;             //Enemy's movement speed in units per second.
    public EnemyState state;            //Current enemy state.
    public int moneyGivenOnDeath;       //How much money the player gets for killing this enemy.

    [Header("Attack")]
    public int attackDamage;            //Damage dealt to target.
    public float attackRate;            //Rate at which the enemy attacks their target.

    [Header("Bools")]
    public bool canMove;                //Can the enemy move?
    public bool canAttack;              //Can the enemy attack their target?
    private bool impactStunned;         //Is the enemy currently impact stunned (reduced speed when damaged)?

    private bool hasDamageOverTimeEffect;   //Does the enemy have a damage over time effect already on it?

    [Header("Components")]
    public Rigidbody rig;               //Enemy's Rigidbody component.
    public EnemyAI ai;                  //Enemy's EnemyAI component.
    public AudioSource audioSource;     //Enemy's Audio Source component.
    public Animator anim;               //Enemy's Animator component.
    public Material defaultMaterial;    //Enemy's default material.
    public MeshSetter meshSetter;       //Enemy's MeshSetter.cs component.
    public GameObject hightlight;

    void Start ()
    {
        //Get missing components.
        if(!rig) rig = GetComponent<Rigidbody>();
        if(!ai) ai = GetComponent<EnemyAI>();
        if(!audioSource) audioSource = GetComponent<AudioSource>();
        if(!anim) anim = transform.Find("EnemyModel").GetComponent<Animator>();
        if(!meshSetter) meshSetter = GetComponent<MeshSetter>();
    }

    private void Update()
    {
        if (Player.inst.currentEnemy == gameObject) {
            hightlight.SetActive(true);
            return;
        }
        hightlight.SetActive(false);
    }

    //Sets up the enemy as if they've just been spawned.
    public void Initialize ()
    {
        canMove = true;
        canAttack = true;
        state = EnemyState.Chasing;
        anim.SetBool("Moving", true);

        //transform.Find("EnemyModel").localPosition = Vector3.zero;
        //transform.Find("EnemyModel").localEulerAngles = new Vector3(0, 90, 0);
        anim.SetTrigger("Respawn");

        ai.target = Player.inst.gameObject;

        meshSetter.SetDefaultMaterial();

        curHp = maxHp;
        rig.isKinematic = false;
    }

    //Called when the enemy gets damaged from any source in the world.
    public void TakeDamage (int damage)
    {
        if(curHp - damage > 0)
        {
            curHp -= damage;
        }
        else
        {
            if(state != EnemyState.Dead)
                Die();
        }

        //Particle effect.
        Pool.Spawn(ParticleManager.inst.bloodImpact, transform.position, Quaternion.identity);
    }

    //Called when the enemy gets damaged from any source in the world.
    public void TakeDamage (int damage, Vector3 impactPos, Vector3 forward)
    {
        if(curHp - damage > 0)
        {
            curHp -= damage;
        }
        else
        {
            if(state != EnemyState.Dead)
                Die();
        }

        //Particle effect.
        Pool.Spawn(ParticleManager.inst.bloodImpact, impactPos, Quaternion.identity).transform.forward = forward;

        //Sound effect.
        AudioManager.inst.Play(audioSource, AudioManager.inst.enemyImpactSFX[Random.Range(0, AudioManager.inst.enemyImpactSFX.Length)]);
    }

    //Called when the enemy's health reaches 0. Kills them.
    public void Die ()
    {
        if(state == EnemyState.Dead)
            return;

        curHp = 0;
        state = EnemyState.Dead;

        //testing
        //GetComponent<MeshRenderer>().material.color = Color.black;
        anim.SetTrigger("Die");

        //Give money.
        Player.inst.AddMoney(moneyGivenOnDeath);

        //Notify the enemy spawner that we died.
        EnemySpawner.inst.remainingEnemies--;

        //Sound effect.
        AudioManager.inst.Play(audioSource, AudioManager.inst.enemyDeathSFX[Random.Range(0, AudioManager.inst.enemyDeathSFX.Length)]);

        DestroyObject(gameObject, 1);
    }

    //Temporarily changes a stat for period of time.
    public void TempStatChange (StatType stat, float modifier, float duration)
    {
        StartCoroutine(StatChangeTimer(stat, modifier, duration));
    }

    //Same as above.
    IEnumerator StatChangeTimer (StatType stat, float modifier, float duration)
    {
        //Apply modofier to stat.
        switch(stat)
        {
            case StatType.MoveSpeed: moveSpeed *= modifier; break;
        }

        //Wait for duration.
        yield return new WaitForSeconds(duration);

        //Return stat back to normal.
        switch(stat)
        {
            case StatType.MoveSpeed: moveSpeed /= modifier; break;
        }
    }

    //Damages the enemy over time.
    public void DamageOverTime (int damage, float rate, float duration)
    {
        if(!hasDamageOverTimeEffect)
            StartCoroutine(DamageOverTimeTimer(damage, rate, duration, null));
    }

    //Damages the enemy over time.
    public void DamageOverTime (int damage, float rate, float duration, GameObject visual)
    {
        if(!hasDamageOverTimeEffect)
            StartCoroutine(DamageOverTimeTimer(damage, rate, duration, visual));
    }

    //Same as above.
    IEnumerator DamageOverTimeTimer (int damage, float rate, float duration, GameObject visual)
    {
        //How many times will the player be damaged?
        int damageCount = Mathf.FloorToInt(duration / rate);

        hasDamageOverTimeEffect = true;

        GameObject visualObj = null;

        //If we have a visual, spawn it.
        if(visual)
            visualObj = Pool.Spawn(visual, transform.position, Quaternion.identity, transform);

        //Loop through that number
        for(int i = 0; i < damageCount; ++i)
        {
            //If we're dead stop doing this.
            if(state == EnemyState.Dead)
                yield break;

            //Damage us, of course.
            TakeDamage(damage);

            //Wait the rate.
            yield return new WaitForSeconds(rate);
        }

        //If we had a visual, destroy it.
        if(visualObj)
            Pool.Destroy(visualObj);

        hasDamageOverTimeEffect = false;
    }

    //Sinks enemy below ground and then destroy them.
    //Used after round ends.
    public void SinkAndDestroy ()
    {
        StartCoroutine(SinkAndDestroyTimer());
    }

    IEnumerator SinkAndDestroyTimer ()
    {
        rig.isKinematic = true;

        while(transform.position.y > -2)
        {
            transform.position += Vector3.down * Time.deltaTime;
            yield return null;
        }

        Pool.Destroy(gameObject);
    }
}

public enum EnemyState
{
    Chasing,
    Attacking,
    Dead
}