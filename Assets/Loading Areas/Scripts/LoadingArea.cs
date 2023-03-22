using System.Collections;

public class LoadingArea : Area
{
    protected override IEnumerator CargoOperation(Inventory carInventory)
        => ShiftCargo(inventory, carInventory);
}
