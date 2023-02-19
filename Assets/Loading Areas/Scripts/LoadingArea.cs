using System.Collections;
using UnityEngine;

public class LoadingArea : MonoBehaviour
{
    private Inventory inventory;
    private RadialIndicator indicator;

    private IEnumerator loadingCoroutine;
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
            loadingCoroutine = Loading(carInventory);
            StartCoroutine(loadingCoroutine);
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

    private IEnumerator Loading(Inventory carInventory)
    {
        while (inventory.CurrentCount > 0 && carInventory.CurrentCount < carInventory.MaxCount)
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
            inventory.CurrentCount--;
            carInventory.CurrentCount++;
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
