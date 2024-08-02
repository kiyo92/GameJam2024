using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homebrew;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Data", menuName = "Level Data", order = 1)]
public class LevelDataScriptableObject : ScriptableObject
{
        [Foldout("Info", true)]

    [Tooltip("Name of scene related to level.")]
    public string sceneName;

    [Tooltip("Name of level displayed on UI elements.")]
    public string levelDisplayName;

    [Tooltip("Is this level locked at the start of the game? Requiring another level to be completed in order to play this one.")]
    public bool lockedAtStart;

    [Tooltip("Level Data of the level that needs to be completed, in order for this one to be unlocked.")]
    public LevelDataScriptableObject levelNeededToCompleteToUnlock;

        [Foldout("Waves", true)]

    [Tooltip("Waves for the level.")]
    public WaveData[] waves;
}

[System.Serializable]
public class WaveData
{
    [Header("Enemies to Spawn")]
    [Tooltip("Enemies types to spawn during the wave.")]
    public WaveEnemyData[] enemyTypes;

    [Header("Spawn Rate")]
    [Tooltip("How often is an enemy spawned during the wave?")]
    public float enemySpawnRate = 1.0f;

    [Header("Boss")]
    [Tooltip("Boss data.")]
    public WaveBossData boss;
}

[System.Serializable]
public class WaveEnemyData
{
    [Tooltip("Enemy object to spawn.")]
    public GameObject enemy;

    [Tooltip("Amount to spawn during the wave.")]
    public int amountToSpawn;
}

[System.Serializable]
public class WaveBossData
{
    [Tooltip("Will a boss spawn this round?")]
    public bool spawnBoss;

    [Tooltip("Boss object to spawn.")]
    public GameObject boss;

    [Range(0.0f, 100.0f)]
    [Tooltip("How far into the wave will the boss spawn? 0 = start, 100 = end, 50 = half way.")]
    public float wavePercentToSpawnBoss = 100.0f;
}