using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private int currentMoney = 500;

    public int CurrentMoney => currentMoney;
    public event Action<int> MoneyChanged;

    public bool TrySpend(int cost)
    {
        if (cost <= 0)
        {
            return true;
        }

        if (currentMoney < cost)
        {
            return false;
        }

        currentMoney -= cost;
        MoneyChanged?.Invoke(currentMoney);
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentMoney += amount;
        MoneyChanged?.Invoke(currentMoney);
    }
}
