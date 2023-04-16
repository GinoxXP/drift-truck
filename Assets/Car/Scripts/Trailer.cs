using UnityEngine;

[RequireComponent (typeof(Car))]
public class Trailer : MonoBehaviour
{
    [SerializeField]
    private Transform trailer;
    [SerializeField]
    private Transform cargo;
    [SerializeField]
    private Transform origin;

    private Car car;

    private void Start()
    {
        car = GetComponent<Car>();

        car.TurnLeftEvent += OnTurnLeft;
        car.TurnRightEvent += OnTurnRight;
        car.DriveEvent += OnDrive;
    }

    private void OnDestroy()
    {
        car.TurnLeftEvent -= OnTurnLeft;
        car.TurnRightEvent -= OnTurnRight;
        car.DriveEvent -= OnDrive;
    }

    private void OnTurnLeft()
    {
        Turn(trailer, car.VisualRotationMaxDegree);

        for (int i = 0; i < cargo.childCount; i++)
            Turn(cargo.GetChild(i), car.VisualRotationMaxDegree);
    }

    private void OnTurnRight()
    {
        Turn(trailer, -car.VisualRotationMaxDegree);
        
        for (int i = 0; i < cargo.childCount; i++)
            Turn(cargo.GetChild(i), -car.VisualRotationMaxDegree);
    }

    private void OnDrive()
    {
        Drive(trailer);

        for (int i = 0; i < cargo.childCount; i++)
            Drive(cargo.GetChild(i));
    }

    private void Turn(Transform transform, float degree)
    {
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.Euler(transform.localRotation.eulerAngles.x, degree, transform.localRotation.eulerAngles.z),
            Time.deltaTime * car.VisualRotationSpeed);
    }

    private void Drive(Transform transform)
    {
        transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, transform.localRotation.eulerAngles.z),
                Time.deltaTime * car.VisualResetRotationSpeed);
    }
}
