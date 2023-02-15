using System.Collections;
using UnityEngine;

public class LoadingArea : MonoBehaviour
{
    private Inventory inventory;
    private IEnumerator loadingCoroutine;

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
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
            StopCoroutine(loadingCoroutine);
        }
    }

    private IEnumerator Loading(Inventory carInventory)
    {
        while (inventory.CurrentCount > 0 && carInventory.CurrentCount < carInventory.MaxCount)
        {
            inventory.CurrentCount--;
            carInventory.CurrentCount++;
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }
}
