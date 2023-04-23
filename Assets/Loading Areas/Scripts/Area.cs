using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Area : MonoBehaviour
{
    private const float FULL_CARGO_OPERATION_TIME = 2.5f;

    [SerializeField]
    private float radius;
    [SerializeField]
    private SphereCollider sphereCollider;
    [SerializeField]
    private Transform area;

    protected Inventory inventory;
    protected RadialLoadingIndicator loadingIndicator;

    protected IEnumerator cargoOperationCoroutine;
    protected IEnumerator indicatorCoroutine;

    protected float TimeDelay { get; private set; }

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        loadingIndicator = GetComponentInChildren<RadialLoadingIndicator>();
    }

    private void OnValidate()
    {
        sphereCollider.radius = radius;
        area.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

        foreach (var indicator in GetComponentsInChildren<RadialIndicator>(false))
            indicator.Radius = radius;
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

        loadingIndicator.Stop();
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
                loadingIndicator.Stop();
            }

            indicatorCoroutine = UpdateIndicatore();
            StartCoroutine(indicatorCoroutine);
            yield return new WaitForSeconds(TimeDelay);
            fromInventory.CurrentCount--;
            toInventory.CurrentCount++;
        }
        loadingIndicator.Stop();
        StopAllCoroutines();

        yield return null;
    }

    protected IEnumerator UpdateIndicatore()
    {
        loadingIndicator.Play(1 / TimeDelay);
        while (true)
        {
            loadingIndicator.UpdateVisual();
            yield return null;
        }
    }
}
