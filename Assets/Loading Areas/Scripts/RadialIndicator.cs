using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RadialIndicator : MonoBehaviour
{
    private const string RADIAL_FILLING_KEY = "_RadialFilling";

    private Material material;
    private float progress;

    public float Progress
    {
        get => progress;
        set
        {
            progress = value;
            material.SetFloat(RADIAL_FILLING_KEY, progress);
        }
    }

    private void Start()
    {
        var renderer = GetComponent<Renderer>();
        material = renderer.material;
        Progress = 0;
    }
}
