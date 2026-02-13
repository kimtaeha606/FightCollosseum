using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int scorePerMonsterKill = 100;
    [SerializeField] private int currentScore;

    public int CurrentScore => currentScore;
    public event Action<int> ScoreChanged;

    private void OnEnable()
    {
        MonsterHealth.AnyMonsterDied += HandleMonsterDied;
    }

    private void OnDisable()
    {
        MonsterHealth.AnyMonsterDied -= HandleMonsterDied;
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentScore += amount;
        ScoreChanged?.Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        ScoreChanged?.Invoke(currentScore);
    }

    private void HandleMonsterDied(MonsterHealth _)
    {
        AddScore(scorePerMonsterKill);
    }
}
