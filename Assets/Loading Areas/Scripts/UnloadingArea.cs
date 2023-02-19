using System.Collections;

public class UnloadingArea : Area
{
    protected override IEnumerator CargoOperation(Inventory carInventory)
        => ShiftCargo(carInventory, inventory);
}
