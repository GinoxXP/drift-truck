using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int maxCount;
    [SerializeField]
    private int currentCount;

    public event Action CurentCountChanged;

    public int MaxCount
    {
        get { return maxCount; }
        set { maxCount = value; }
    }

    public int CurrentCount
    {
        get { return currentCount; }
        set
        {
            currentCount = value;
            CurentCountChanged?.Invoke();
        }
    }

    private void Start()
    {
        MaxCount = maxCount;
        CurrentCount = currentCount;
    }
}
