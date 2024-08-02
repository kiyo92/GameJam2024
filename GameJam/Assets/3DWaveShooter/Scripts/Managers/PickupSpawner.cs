using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject[] pickups;            //Array of available pickups to spawn.
    public GameObject[] pickupSpawnPoints;  //Positions to spawn the pickups at.

    public float averageSpawnRate;          //Average amount of time between spawning.
    public float spawnRateRandomness;       //Random range applied to average spawn rate.

    private float lastSpawnTime;            //Last time a pickup was spawned.
    private float nextSpawnTime;            //Next time a pickup will be spawned.

    private int lastPickupSpawned;          //Index of the last pickup spawned.
    private int lastPickupSpawnPoint;       //Index of the last pickup spawn point.

    void Update ()
    {
        if(GameManager.inst.waveInProgress)
        {
            if(Time.time - lastSpawnTime >= nextSpawnTime)
            {
                SpawnPickup();
            }
        }
    }

    void SpawnPickup ()
    {
        lastSpawnTime = Time.time;
        nextSpawnTime = GetNextPickupSpawnTime();

        int spawnPoint = Random.Range(0, pickupSpawnPoints.Length);
        int pickup = Random.Range(0, pickups.Length);

        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

        GameObject pickupObj = Instantiate(pickups[pickup], pickupSpawnPoints[spawnPoint].transform.position + offset, Quaternion.identity);

        lastPickupSpawned = pickup;
        lastPickupSpawnPoint = spawnPoint;
    }

    float GetNextPickupSpawnTime ()
    {
        return Time.time + averageSpawnRate + Random.Range(-spawnRateRandomness / 2, spawnRateRandomness / 2);
    }
}
