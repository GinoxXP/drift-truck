using UnityEngine;

public class CarPreview : MonoBehaviour
{
    [SerializeField]
    private GameObject niva;
    [SerializeField]
    private GameObject lada;
    [SerializeField]
    private GameObject gazel;
    [SerializeField]
    private GameObject truck;

    public void ActivateNiva()
    {
        DeactivateAll();
        niva.SetActive(true);
    }

    public void ActivateLada()
    {
        DeactivateAll();
        lada.SetActive(true);
    }

    public void ActivateGazel()
    {
        DeactivateAll();
        gazel.SetActive(true);
    }

    public void ActivateTruck()
    {
        DeactivateAll();
        truck.SetActive(true);
    }

    public void DeactivateAll()
    {
        niva.SetActive(false);
        lada.SetActive(false);
        gazel.SetActive(false);
        truck.SetActive(false);
    }

    private void Start()
    {
        DeactivateAll();
    }
}
