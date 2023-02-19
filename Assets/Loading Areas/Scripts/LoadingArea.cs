using System.Collections;
using UnityEngine;

public class LoadingArea : Area
{
    protected override IEnumerator CargoOperation(Inventory carInventory)
    {
        while (inventory.CurrentCount > 0 && carInventory.CurrentCount < carInventory.MaxCount)
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
            inventory.CurrentCount--;
            carInventory.CurrentCount++;
        }
        indicator.Stop();
        StopAllCoroutines();

        yield return null;
    }
}
