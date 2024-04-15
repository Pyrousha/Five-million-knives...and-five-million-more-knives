using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private bool startFirstWaveOnPlay;


    [System.Serializable]
    private struct EnemyTypeAndPrefab
    {
        [field: SerializeField] public EnemyTypes Type { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }

    [System.Serializable]
    private struct EnemyTypeAndSpawnPosIndex
    {
        [field: SerializeField] public EnemyTypes Type { get; private set; }
        [field: SerializeField] public int SpawnPositionIndex { get; private set; }
        [field: SerializeField] public float TimeToWaitTillNextEnemy { get; private set; }
    }

    [System.Serializable]
    private enum EnemyTypes
    {
        Basic_Melee
    }

    [System.Serializable]
    private struct Wave
    {
        [field: SerializeField] public List<EnemyTypeAndSpawnPosIndex> Enemies { get; private set; }
    }


    [SerializeField] private List<EnemyTypeAndPrefab> enemiesList;
    private Dictionary<EnemyTypes, GameObject> enemiesDictionary;

    [SerializeField] private List<Transform> spawnPositions;

    [SerializeField] private List<Wave> waves;
    private int currWaveIndex;

    private int numEnemiesLeft;
    private bool waveDoneSpawning = true;

    private Routine waveRoutine;

    // Start is called before the first frame update
    void Start()
    {
        enemiesDictionary = new Dictionary<EnemyTypes, GameObject>();
        foreach (EnemyTypeAndPrefab enemy in enemiesList)
            enemiesDictionary.Add(enemy.Type, enemy.Prefab);

        if (startFirstWaveOnPlay)
        {
            currWaveIndex = 0;
            SpawnWave(waves[currWaveIndex]);
        }
    }

    private void SpawnWave(Wave _wave, float startupTime = 0f)
    {
        GameplayPopupController.Instance.ShowPopupText($"WAVE {currWaveIndex + 1}");

        waveRoutine.Stop();
        waveRoutine = Routine.Start(this, SpawnWaveRoutine(_wave, startupTime));
    }

    private IEnumerator SpawnWaveRoutine(Wave _wave, float startupTime)
    {
        waveDoneSpawning = false;

        if (startupTime > 0)
            yield return startupTime;

        foreach (EnemyTypeAndSpawnPosIndex enemy in _wave.Enemies)
        {
            Transform spawnedEnemy = Instantiate(enemiesDictionary[enemy.Type]).transform;
            spawnedEnemy.position = spawnPositions[enemy.SpawnPositionIndex].position;

            yield return enemy.TimeToWaitTillNextEnemy;
        }

        waveDoneSpawning = true;
    }

    public void OnEnemySpawned()
    {
        numEnemiesLeft++;
    }

    public void OnEnemyKilled()
    {
        numEnemiesLeft--;

        if (numEnemiesLeft == 0 && waveDoneSpawning)
        {
            currWaveIndex++;
            if (currWaveIndex < waves.Count)
            {
                SpawnWave(waves[currWaveIndex], 2f);
            }
            else
            {
                //No more waves! You win!!
                GameplayPopupController.Instance.OnGameWin();
            }
        }
    }
}
