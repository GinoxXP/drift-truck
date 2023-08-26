using UnityEngine;

[RequireComponent(typeof(ALoadScene))]
public class FinishLine : MonoBehaviour
{
    private ALoadScene loadLevel;

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        if (car == null)
            return;

        car.Stop();
        loadLevel.Load();
    }

    private void Start()
    {
        loadLevel = GetComponent<ALoadScene>();
    }
}
