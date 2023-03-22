using UnityEngine;
using Zenject;

public class FollowingCamera : MonoBehaviour
{
    private Transform followingObject;
    private Vector3 deltaPosition;

    private void Start()
    {
        deltaPosition = transform.position - followingObject.position;
    }

    private void Update()
    {
        transform.position = followingObject.position + deltaPosition;
    }

    [Inject]
    private void Init(Car car)
    {
        followingObject = car.transform;
    }
}
