using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] spawnPoints;                        //Array of all the spawn points in the level.
    public WaveData[] waves;                                //Array of the waves which is generated below.
    private List<GameObject> curEnemies = new List<GameObject>();

    public WaveData curWave;                                //Current wave that is up.
    public int nextWaveIndex;                               //Index for waves[], which indicates the next wave.
    public List<int> enemiesToSpawn = new List<int>();      //List of enemies to spawn where the int is the index for the different enemies in a WaveData class.

    private float lastTimeSpawnEnemy;                       //Time at which the last enemy was spawned.
    private int curEnemyToSpawnIndex;                       //Index of the current enemy to spawn, relates to enemiesToSpawn[].
    private int curSpawnPointIndex;                         //Index of the current spawn point, relates to spawnPoints[].
    private int bossSpawnIndex;                             //CurEnemyToSpawnIndex where the boss spawns.

    public int remainingEnemies;                            //Amount of enemies that remain in the current wave.

    public bool spawnEnemies;                               //Are we spawning enemies? Cuts off once all the enemies of a wave have spawned.

    //Instance
    public static EnemySpawner inst;
    void Awake () { inst = this; }

    void Start ()
    {
        //Initialize the wave data.
        waves = GameManager.inst.level.waves;
    }

    //Starts spawning enemies in the next wave.
    public void SetNewWave ()
    {
        curWave = waves[nextWaveIndex];
        enemiesToSpawn.Clear();
        nextWaveIndex++;
        curEnemyToSpawnIndex = 0;

        //If we have cur enemies from last wave, destroy them.
        if(curEnemies.Count > 0)
        {
            //for(int x = 0; x < curEnemies.Count; ++x)
                //curEnemies[x].GetComponent<Enemy>().SinkAndDestroy();
        }

        curEnemies.Clear();

        //Start to generate a list of enemies to spawn.
        for(int i = 0; i < curWave.enemyTypes.Length; ++i)
        {
            for(int x = 0; x < curWave.enemyTypes[i].amountToSpawn; ++x)
            {
                enemiesToSpawn.Add(i);
            }
        }

        //Randomise that list, then start.
        IListExtensions.Shuffle<int>(enemiesToSpawn);

        if(curWave.boss.spawnBoss)
        {
            bossSpawnIndex = enemiesToSpawn.Count * Mathf.CeilToInt(0.01f * (float)curWave.boss.wavePercentToSpawnBoss);

            if(bossSpawnIndex == 0)
                bossSpawnIndex = 1;
        }

        remainingEnemies = enemiesToSpawn.Count;

        spawnEnemies = true;
    }

    void Update ()
    {
        //Are we currently spawning enemies?
        if(spawnEnemies && spawnPoints.Length > 0)
        {
            //Are we able to spawn an enemy? Timed interval based on wave's spawn rate.
            if(Time.time - lastTimeSpawnEnemy > curWave.enemySpawnRate)
            {
                if(curEnemyToSpawnIndex == enemiesToSpawn.Count)
                {
                    spawnEnemies = false;
                    return;
                }

                //Spawn the enemy at a random spawn point.
                lastTimeSpawnEnemy = Time.time;
                SpawnEnemy(curWave.enemyTypes[enemiesToSpawn[curEnemyToSpawnIndex]].enemy, spawnPoints[curSpawnPointIndex].transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)));

                //Spawn boss if we have to.
                if(curWave.boss.spawnBoss)
                {
                    if(curEnemyToSpawnIndex == bossSpawnIndex - 1)
                    {
                        SpawnEnemy(curWave.boss.boss, spawnPoints[curSpawnPointIndex].transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)));
                        remainingEnemies++;
                    }
                }

                ++curEnemyToSpawnIndex;
                ++curSpawnPointIndex;

                if(curSpawnPointIndex == spawnPoints.Length)
                    curSpawnPointIndex = 0;
            }
        }

        //If there are no enemies left, end the wave.
        if(GameManager.inst.waveInProgress)
        {
            if(remainingEnemies == 0 && GameManager.inst.curWaveTime > GameManager.inst.waveCountdownTime + 2)
            {
                GameManager.inst.EndWave();
            }
        }
    }

    //Creates a given enemy at a given spawn position.
    void SpawnEnemy (GameObject enemyObject, Vector3 pos)
    {
        GameObject enemy = Pool.Spawn(enemyObject, pos, Quaternion.identity);
        curEnemies.Add(enemy);
        enemy.GetComponent<Enemy>().Initialize();
    }
}

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for(var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}