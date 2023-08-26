using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    private enum Control
    {
        Left,
        Right,
    }

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
    private List<Transform> rotationWheels;
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
    private Vector2 pointerPosition;
    private List<Control> controlList = new();

    public event Action TurnLeftEvent;

    public event Action TurnRightEvent;

    public event Action DriveEvent;

    public float VisualRotationMaxDegree => visualRotationMaxDegree;
    public float VisualRotationSpeed => visualRotationSpeed;
    public float VisualResetRotationSpeed => visualResetRotationSpeed;

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

    public void OnTurnLeft(InputAction.CallbackContext context)
        => Move(context, Control.Left);

    public void OnTurnRight(InputAction.CallbackContext context)
        => Move(context, Control.Right);

    public void OnClick(InputAction.CallbackContext context)
    {
        if (IsPointerOverUIObject(pointerPosition))
            return;

        if (isStop)
        {
            isStop = false;
            return;
        }

        Control currentControl;

        if (pointerPosition.x < Screen.width / 2)
            currentControl = Control.Left;
        else
            currentControl = Control.Right;

        Move(context, currentControl);
    }

    public void OnPointer(InputAction.CallbackContext context)
    {
        pointerPosition = context.ReadValue<Vector2>();
    }

    private void Move(InputAction.CallbackContext context, Control control)
    {
        if (isStop)
        {
            isStop = false;
            return;
        }

        if (context.canceled)
        {
            controlList.Remove(control);
            return;
        }

        if (context.started)
        {
            controlList.Add(control);
            return;
        }
    }

    private bool IsPointerOverUIObject(Vector2 position)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void TurnLeft()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
        visual.localRotation = Quaternion.Lerp(
            visual.localRotation,
            Quaternion.Euler(0, -visualRotationMaxDegree, 0),
            Time.deltaTime * visualRotationSpeed);

        foreach(var wheel in rotationWheels)
        {
            wheel.localRotation = Quaternion.Lerp(
                wheel.localRotation,
                Quaternion.Euler(0, visualRotationMaxDegree, 0),
                Time.deltaTime * visualRotationSpeed);
        }

        if (!rubberParticles.isPlaying)
            rubberParticles.Play();

        TurnLeftEvent?.Invoke();
    }

    private void TurnRight()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        visual.localRotation = Quaternion.Lerp(
            visual.localRotation,
            Quaternion.Euler(0, visualRotationMaxDegree, 0),
            Time.deltaTime * visualRotationSpeed);

        foreach (var wheel in rotationWheels)
        {
            wheel.localRotation = Quaternion.Lerp(
                wheel.localRotation,
                Quaternion.Euler(0, -visualRotationMaxDegree, 0),
                Time.deltaTime * visualRotationSpeed);
        }

        if (!rubberParticles.isPlaying)
            rubberParticles.Play();

        TurnRightEvent?.Invoke();
    }

    private void Drive()
    {
        visual.localRotation = visual.localRotation = Quaternion.Lerp(
                visual.localRotation,
                Quaternion.identity,
                Time.deltaTime * visualResetRotationSpeed);

        foreach (var wheel in rotationWheels)
        {
            wheel.localRotation = wheel.localRotation = Quaternion.Lerp(
                wheel.localRotation,
                Quaternion.identity,
                Time.deltaTime * visualResetRotationSpeed);
        }

        rubberParticles.Stop();

        DriveEvent?.Invoke();
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

        if (!controlList.IsEmpty())
        {
            var lastControl = controlList.Last();
            if (lastControl == Control.Left)
                TurnLeft();
            else if (lastControl == Control.Right)
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
