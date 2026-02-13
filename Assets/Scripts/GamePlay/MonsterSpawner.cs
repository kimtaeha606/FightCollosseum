using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    private class MonsterEntry
    {
        public GameObject prefab;
        [Min(0f)] public float difficultyScore = 1f;
    }

    [Header("References")]
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private Transform player;

    [Header("Spawn Setup")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private List<MonsterEntry> monsterPrefabs = new List<MonsterEntry>();
    [SerializeField] private float baseSpawnPerTime = 1f;
    [SerializeField] private int maxAliveMonsters = 30;

    [Header("Difficulty Weighting")]
    [SerializeField] private float hardBiasStrength = 2f;
    [SerializeField] private float easyWeightPower = 2f;

    private readonly List<MonsterHealth> aliveMonsters = new List<MonsterHealth>();
    private float spawnTimer;

    private void Reset()
    {
        difficultyManager = FindObjectOfType<DifficultyManager>();
    }

    private void Awake()
    {
        if (difficultyManager == null)
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        CleanupDeadMonsters();

        float spawnMultiplier = GetSpawnMultiplier();
        float spawnPerTime = Mathf.Max(0f, baseSpawnPerTime) * spawnMultiplier;
        if (spawnPerTime <= 0f || !CanSpawn())
        {
            return;
        }

        spawnTimer += Time.deltaTime * spawnPerTime;

        while (spawnTimer >= 1f && CanSpawn())
        {
            spawnTimer -= 1f;
            SpawnOne(spawnMultiplier);
        }
    }

    private float GetSpawnMultiplier()
    {
        float multiplier = difficultyManager != null ? difficultyManager.SpawnMultiplier : 1f;
        return Mathf.Max(0f, multiplier);
    }

    private bool CanSpawn()
    {
        return spawnPoints.Count > 0 &&
               monsterPrefabs.Count > 0 &&
               aliveMonsters.Count < Mathf.Max(1, maxAliveMonsters);
    }

    private void SpawnOne(float spawnMultiplier)
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        GameObject selectedPrefab = SelectMonsterPrefab(spawnMultiplier);
        if (spawnPoint == null || selectedPrefab == null)
        {
            return;
        }

        GameObject instance = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);

        MonsterAI monsterAI = instance.GetComponent<MonsterAI>();
        if (monsterAI != null && player != null)
        {
            monsterAI.SetTarget(player);
        }

        MonsterHealth monsterHealth = instance.GetComponent<MonsterHealth>();
        if (monsterHealth != null)
        {
            aliveMonsters.Add(monsterHealth);
            monsterHealth.Died += () => aliveMonsters.Remove(monsterHealth);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index];
    }

    private GameObject SelectMonsterPrefab(float spawnMultiplier)
    {
        float minDifficulty = float.MaxValue;
        float maxDifficulty = float.MinValue;

        for (int i = 0; i < monsterPrefabs.Count; i++)
        {
            MonsterEntry entry = monsterPrefabs[i];
            if (entry == null || entry.prefab == null)
            {
                continue;
            }

            float score = Mathf.Max(0f, entry.difficultyScore);
            minDifficulty = Mathf.Min(minDifficulty, score);
            maxDifficulty = Mathf.Max(maxDifficulty, score);
        }

        if (minDifficulty == float.MaxValue)
        {
            return null;
        }

        float difficultyProgress01 = Mathf.Clamp01(1f - (1f / Mathf.Max(1f, spawnMultiplier)));

        float totalWeight = 0f;
        float[] weights = new float[monsterPrefabs.Count];

        GameObject fallbackPrefab = null;

        for (int i = 0; i < monsterPrefabs.Count; i++)
        {
            MonsterEntry entry = monsterPrefabs[i];
            if (entry == null || entry.prefab == null)
            {
                weights[i] = 0f;
                continue;
            }

            if (fallbackPrefab == null)
            {
                fallbackPrefab = entry.prefab;
            }

            float normalizedDifficulty = Mathf.InverseLerp(minDifficulty, maxDifficulty, Mathf.Max(0f, entry.difficultyScore));
            float easyWeight = Mathf.Pow(1f - normalizedDifficulty, Mathf.Max(0.01f, easyWeightPower));
            float hardBias = 1f + difficultyProgress01 * hardBiasStrength * normalizedDifficulty;
            float finalWeight = Mathf.Max(0.0001f, easyWeight * hardBias);

            weights[i] = finalWeight;
            totalWeight += finalWeight;
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float pick = Random.value * totalWeight;
        for (int i = 0; i < monsterPrefabs.Count; i++)
        {
            pick -= weights[i];
            if (pick <= 0f)
            {
                return monsterPrefabs[i].prefab;
            }
        }

        return fallbackPrefab;
    }

    private void CleanupDeadMonsters()
    {
        for (int i = aliveMonsters.Count - 1; i >= 0; i--)
        {
            if (aliveMonsters[i] == null || aliveMonsters[i].IsDead)
            {
                aliveMonsters.RemoveAt(i);
            }
        }
    }
}
