using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RadialIndicator : MonoBehaviour
{
    private const string RADIAL_FILLING_KEY = "_RadialFilling";
    private const string ANIMATION_NAME = "Progress";

    ///
    /// Changed by animator
    ///
    [SerializeField]
    private float progress;

    private Animator animator;
    private Material material;

    public void UpdateVisual()
    {
        material.SetFloat(RADIAL_FILLING_KEY, progress);
    }

    public void Play(float speed)
    {
        animator.Play(ANIMATION_NAME, -1, 0);
        animator.speed = speed;
    }

    public void Stop()
    {
        progress = 0;
        UpdateVisual();
        animator.speed = 0;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        var renderer = GetComponent<Renderer>();
        material = renderer.material;

        Stop();
    }
}
