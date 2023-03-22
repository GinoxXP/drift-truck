using System.Collections;
using UnityEngine;

public abstract class Area : MonoBehaviour
{
    private const float FULL_CARGO_OPERATION_TIME = 2.5f;

    protected Inventory inventory;
    protected RadialIndicator indicator;

    protected IEnumerator cargoOperationCoroutine;
    protected IEnumerator indicatorCoroutine;

    protected float TimeDelay { get; private set; }

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        indicator = GetComponentInChildren<RadialIndicator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var carInventory = other.GetComponentInParent<Inventory>();
        if (carInventory == null)
            return;

        TimeDelay = FULL_CARGO_OPERATION_TIME / carInventory.MaxCount;
        cargoOperationCoroutine = CargoOperation(carInventory);
        StartCoroutine(cargoOperationCoroutine);
    }

    private void OnTriggerExit(Collider other)
    {
        var carInventory = other.GetComponentInParent<Inventory>();
        if (carInventory == null)
            return;

        indicator.Stop();
        StopAllCoroutines();
    }

    protected abstract IEnumerator CargoOperation(Inventory carInventory);

    protected IEnumerator ShiftCargo(Inventory fromInventory, Inventory toInventory)
    {
        while (fromInventory.CurrentCount > 0 && toInventory.CurrentCount < toInventory.MaxCount)
        {
            if (indicatorCoroutine != null)
            {
                StopCoroutine(indicatorCoroutine);
                indicatorCoroutine = null;
                indicator.Stop();
            }

            indicatorCoroutine = UpdateIndicatore();
            StartCoroutine(indicatorCoroutine);
            yield return new WaitForSeconds(TimeDelay);
            fromInventory.CurrentCount--;
            toInventory.CurrentCount++;
        }
        indicator.Stop();
        StopAllCoroutines();

        yield return null;
    }

    protected IEnumerator UpdateIndicatore()
    {
        indicator.Play(1 / TimeDelay);
        while (true)
        {
            indicator.UpdateVisual();
            yield return null;
        }
    }
}
