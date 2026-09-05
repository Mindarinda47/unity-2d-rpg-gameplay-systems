using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [SerializeField] private int gold;

    public int Gold => gold;

    public event Action<int> GoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddGold(int amount)
    {
        if (amount == 0)
            return;

        if (amount < 0)
        {
            Debug.LogWarning($"Áö±ÞÇÒ °ñµå°¡ ¿Ã¹Ù¸£Áö ¾Ê½À´Ï´Ù: {amount}");
            return;
        }

        gold += amount;

        GoldChanged?.Invoke(gold);

        Debug.Log($"°ñµå È¹µæ: {amount} / ÇöÀç °ñµå: {gold}");
    }

    public void SetGold(int amount)
    {
        gold = Mathf.Max(0, amount);
        GoldChanged?.Invoke(gold);
    }
}