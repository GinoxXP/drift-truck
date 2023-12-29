using UnityEngine;
using Zenject;

[RequireComponent(typeof(ALoadScene))]
public class FinishLine : MonoBehaviour
{
    private IAds ads;

    private ALoadScene loadLevel;

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        if (car == null)
            return;

        car.Stop();
        loadLevel.Load();
        ads.ShowVideoAd();
    }

    private void Start()
    {
        loadLevel = GetComponent<ALoadScene>();
    }

    [Inject]
    private void Init(IAds ads)
    {
        this.ads = ads;
    }
}
