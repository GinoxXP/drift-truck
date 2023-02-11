using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField]
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
}
