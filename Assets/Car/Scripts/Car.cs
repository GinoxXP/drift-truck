using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float minSpeed;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float driftAcceleration;
    [SerializeField]
    private float rotationSpeed;
    [Space]
    [Header("Visual")]
    [SerializeField]
    private Transform visual;
    [SerializeField]
    private float visualRotationMaxDegree;
    [SerializeField]
    private float visualRotationSpeed;
    [SerializeField]
    private float visualResetRotationSpeed;
    [Space]
    [Header("Particles")]
    [SerializeField]
    private ParticleSystem burstParticles;
    [SerializeField]
    private ParticleSystem smokeParticles;
    [SerializeField]
    private ParticleSystem rubberParticles;

    private new Rigidbody rigidbody;
    private float speed;
    private bool isStop = true;
    private bool isTurningLeft;
    private bool isTurningRight;
    private Vector2 pointerPosition;
    private IEnumerator moveCoroutine;

    public void Crash()
    {
        enabled = false;

        burstParticles.Play();
        smokeParticles.Play();
        rubberParticles.Stop();

        rigidbody.velocity = Vector3.zero;
    }

    public void Stop()
    {
        enabled = false;

        rubberParticles.Stop();

        rigidbody.velocity = Vector3.zero;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isStop)
        {
            isStop = false;
        }

        var direction = context.ReadValue<float>();

        if (direction == 0)
        {
            isTurningLeft = false;
            isTurningRight = false;
        }
        else if (direction < 0)
        {
            isTurningLeft = true;
            isTurningRight = false;
        }
        else if (direction > 0)
        {
            isTurningLeft = false;
            isTurningRight = true;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (isStop)
        {
            isStop = false;
        }

        if (context.started)
        {
            if(moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = Move();
            StartCoroutine(moveCoroutine);
        }

        if (context.canceled)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            isTurningLeft = false;
            isTurningRight = false;
            return;
        }
    }

    public void OnPointer(InputAction.CallbackContext context)
    {
        pointerPosition = context.ReadValue<Vector2>();
    }

    private IEnumerator Move()
    {
        while (true)
        {
            if (pointerPosition.x < Screen.width / 2)
            {
                isTurningLeft = true;
                isTurningRight = false;
            }
            else
            {
                isTurningLeft = false;
                isTurningRight = true;
            }
            yield return null;
        }
    }

    private void TurnLeft()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
        visual.localRotation = Quaternion.Lerp(
            visual.localRotation,
            Quaternion.FromToRotation(visual.forward, Quaternion.Euler(0, -visualRotationMaxDegree, 0) * visual.forward),
            Time.deltaTime * visualRotationSpeed);

        if (!rubberParticles.isPlaying)
            rubberParticles.Play();
    }

    private void TurnRight()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        visual.localRotation = Quaternion.Lerp(
            visual.localRotation,
            Quaternion.FromToRotation(visual.forward, Quaternion.Euler(0, visualRotationMaxDegree, 0) * visual.forward),
            Time.deltaTime * visualRotationSpeed);

        if (!rubberParticles.isPlaying)
            rubberParticles.Play();
    }

    private void Drive()
    {
        visual.localRotation = visual.localRotation = Quaternion.Lerp(
                visual.localRotation,
                Quaternion.identity,
                Time.deltaTime * visualResetRotationSpeed);
        rubberParticles.Stop();
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isStop)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }

        speed = rigidbody.velocity.magnitude;

        if (isTurningLeft || isTurningRight)
        {
            if (isTurningLeft)
                TurnLeft();
            else if (isTurningRight)
                TurnRight();

            speed += driftAcceleration * Time.deltaTime;
        }
        else
        {
            Drive();

            if (speed < maxSpeed)
                speed += acceleration * Time.deltaTime;
        }

        if (speed < minSpeed)
            speed = minSpeed;

        rigidbody.velocity = transform.forward * speed;
    }
}
