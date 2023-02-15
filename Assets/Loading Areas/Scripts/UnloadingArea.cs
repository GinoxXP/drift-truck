using System.Collections;
using UnityEngine;

public class UnloadingArea : MonoBehaviour
{
    private Inventory inventory;
    private IEnumerator unloadingCoroutine;

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
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
            StopCoroutine(unloadingCoroutine);
        }
    }

    private IEnumerator Unloading(Inventory carInventory)
    {
        while (carInventory.CurrentCount > 0 && inventory.CurrentCount < inventory.MaxCount)
        {
            carInventory.CurrentCount--;
            inventory.CurrentCount++;
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }
}
