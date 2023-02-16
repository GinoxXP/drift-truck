using UnityEngine;

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

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isStop)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                isStop = false;

            rigidbody.velocity = Vector3.zero;
            return;
        }

        speed = rigidbody.velocity.magnitude;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
            visual.localRotation = Quaternion.Lerp(
                visual.localRotation,
                Quaternion.FromToRotation(visual.forward, Quaternion.Euler(0, -visualRotationMaxDegree, 0) * visual.forward),
                Time.deltaTime * visualRotationSpeed);

            if (!rubberParticles.isPlaying)
                rubberParticles.Play();

            speed += driftAcceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            visual.localRotation = Quaternion.Lerp(
                visual.localRotation,
                Quaternion.FromToRotation(visual.forward, Quaternion.Euler(0, visualRotationMaxDegree, 0) * visual.forward),
                Time.deltaTime * visualRotationSpeed);

            if (!rubberParticles.isPlaying)
                rubberParticles.Play();

            speed += driftAcceleration * Time.deltaTime;
        }
        else
        {
            visual.localRotation = visual.localRotation = Quaternion.Lerp(
                visual.localRotation,
                Quaternion.identity,
                Time.deltaTime * visualResetRotationSpeed);
            rubberParticles.Stop();

            if (speed <= maxSpeed)
                speed += acceleration * Time.deltaTime;
        }

        if (speed < minSpeed)
            speed = minSpeed;

        rigidbody.velocity = transform.forward * speed;
    }
}
