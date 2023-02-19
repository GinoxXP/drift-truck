using System.Collections;
using UnityEngine;

public class UnloadingArea : Area
{
    protected override IEnumerator CargoOperation(Inventory carInventory)
    {
        while (carInventory.CurrentCount > 0 && inventory.CurrentCount < inventory.MaxCount)
        {
            if (indicatorCoroutine != null)
            {
                StopCoroutine(indicatorCoroutine);
                indicatorCoroutine = null;
                indicator.Stop();
            }

            indicatorCoroutine = UpdateIndicatore(TimeDelay / carInventory.MaxCount);
            StartCoroutine(indicatorCoroutine);
            yield return new WaitForSeconds(TimeDelay);
            carInventory.CurrentCount--;
            inventory.CurrentCount++;
        }
        indicator.Stop();
        StopAllCoroutines();

        yield return null;
    }
}
