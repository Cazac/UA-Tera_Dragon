﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Wave", menuName="WaveSetting")]
public class Wave : ScriptableObject
{
    public GameObject EnemyPrefab;
    public GameObject[] EnemiesPrefab;

    //Add node to list will set value of SpawnPosition
    public List<GameObject> SpawnPositionByNodes;
    public List<Vector3> SpawnPosition;
    [HideInInspector]
    public GameObject ParentGameobject;
    public int NumberOfEnemyPerPos; //Numver of enemy per position
    public float TimeUntilSpawn; //Count down timer until next wave
    public float WaveTimer; //Timer for current wave
    public float SpawnRate; //How many enemy should be spawn after some time

     /// <summary>
     ///Constructor with parameter for spawning one type of enemy
    /// </summary>
    /// <param name="enemyPrefab">Enemy prefab that will be spawned</param>
    /// <param name="spawnPosition">List of Vector3 positions that enemy will spawn respectively</param>
    /// <param name="parentGameobject">Parent to store spawned enemy</param>
    /// <param name="numberOfEnemyPerPos">Amount of enemy that can spawn in a specific position</param>
    public Wave(GameObject enemyPrefab, List<Vector3> spawnPosition, GameObject parentGameobject, int numberOfEnemyPerPos)
    {
        EnemyPrefab = enemyPrefab;
        SpawnPosition = spawnPosition;
        ParentGameobject = parentGameobject;
        NumberOfEnemyPerPos = numberOfEnemyPerPos;

        foreach (var node in SpawnPositionByNodes)
            SpawnPosition.Add(node.transform.position);
    }


     /// <summary>
     ///Constructor with parameter for spawning more than 1 type of enemy
    /// </summary>
    /// <param name="enemyPrefabs">Enemy prefab that will be spawned</param>
    /// <param name="spawnPosition">List of Vector3 positions that enemy will spawn respectively</param>
    /// <param name="parentGameobject">Parent to store spawned enemy</param>
    /// <param name="numberOfEnemyPerPos">Amount of enemy that can spawn in a specific position</param>
    public Wave(GameObject[] enemyPrefabs, List<Vector3> spawnPosition, GameObject parentGameobject, int numberOfEnemyPerPos)
    {
        EnemiesPrefab = enemyPrefabs;
        SpawnPosition = spawnPosition;
        ParentGameobject = parentGameobject;
        NumberOfEnemyPerPos = numberOfEnemyPerPos;

        foreach (var node in SpawnPositionByNodes)
            SpawnPosition.Add(node.transform.position);
    }
    
}
