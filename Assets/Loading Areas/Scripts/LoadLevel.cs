using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Inventory))]
public class LoadLevel : MonoBehaviour
{
    private const float LOAD_LEVEL_DELAY = 1.5f;

    [SerializeField]
    private string levelName;

    private Inventory inventory;
    private Level level;
    private Car car;

    private void OnInventoryCurentCountChanged()
    {
        if(inventory.CurrentCount >= inventory.MaxCount)
        {
            car.Stop();
            level.LoadLevel(levelName, LOAD_LEVEL_DELAY);
        }
    }

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        inventory.CurentCountChanged += OnInventoryCurentCountChanged;
    }

    private void OnDestroy()
    {
        inventory.CurentCountChanged -= OnInventoryCurentCountChanged;
    }

    [Inject]
    private void Init(Level level, Car car)
    {
        this.level = level;
        this.car = car;
    }
}
