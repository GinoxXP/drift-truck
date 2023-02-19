using System.Collections;
using UnityEngine;

public class UnloadingArea : MonoBehaviour
{
    private Inventory inventory;
    private RadialIndicator indicator;

    private IEnumerator unloadingCoroutine;
    private IEnumerator indicatorCoroutine;

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        indicator = GetComponentInChildren<RadialIndicator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Inventory>(out var carInventory))
        {
            unloadingCoroutine = Unloading(carInventory);
            StartCoroutine(unloadingCoroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Inventory>(out var _))
        {
            indicator.Progress = 0;
            StopAllCoroutines();
        }
    }

    private IEnumerator Unloading(Inventory carInventory)
    {
        while (carInventory.CurrentCount > 0 && inventory.CurrentCount < inventory.MaxCount)
        {
            if (indicatorCoroutine != null)
            {
                StopCoroutine(indicatorCoroutine);
                indicatorCoroutine = null;
                indicator.Progress = 0;
            }

            indicatorCoroutine = UpdateIndicatore(0.5f);
            StartCoroutine(indicatorCoroutine);
            yield return new WaitForSeconds(0.5f);
            carInventory.CurrentCount--;
            inventory.CurrentCount++;
        }

        yield return null;
    }

    private IEnumerator UpdateIndicatore(float fullCycleTime)
    {
        for (int i = 0; i < 100; i++)
        {
            var progress = (float)i / 100;
            indicator.Progress = progress;
            yield return null;
        }

        indicator.Progress = 0;

        yield return null;
    }
}
