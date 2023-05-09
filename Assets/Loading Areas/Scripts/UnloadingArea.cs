using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(LoadLevel))]
public class UnloadingArea : Area
{
    private LoadLevel loadLevel;
    private Car car;

    protected override IEnumerator CargoOperation(Inventory carInventory)
        => ShiftCargo(carInventory, inventory);

    protected override void OnStart()
    {
        loadLevel = GetComponent<LoadLevel>();
        inventory.CurentCountChanged += OnInventoryCurentCountChanged;
    }

    private void OnDestroy()
    {
        inventory.CurentCountChanged -= OnInventoryCurentCountChanged;
    }

    private void OnInventoryCurentCountChanged()
    {
        if (inventory.CurrentCount >= inventory.MaxCount)
        {
            car.Stop();
            loadLevel.Load();
        }
    }

    [Inject]
    private void Init(Car car)
    {
        this.car = car;
    }
}
