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
        if (other.TryGetComponent<Inventory>(out var carInventory))
        {
            TimeDelay = FULL_CARGO_OPERATION_TIME / carInventory.MaxCount;
            cargoOperationCoroutine = CargoOperation(carInventory);
            StartCoroutine(cargoOperationCoroutine);
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

    protected abstract IEnumerator CargoOperation(Inventory carInventory);

    protected IEnumerator UpdateIndicatore(float fullCycleTime)
    {
        for (int i = 1; i <= 10; i++)
        {
            var progress = (float)i / 10;
            indicator.Progress = progress;
            yield return new WaitForSeconds(fullCycleTime / 10);
        }

        indicator.Progress = 0;

        yield return null;
    }
}
