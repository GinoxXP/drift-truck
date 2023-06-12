using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RadialIndicator : MonoBehaviour
{
    private const string SIZE_MULTIPLIER_KEY = "_SizeMultiplier";

    private float radius;

    public float Radius
    {
        get { return radius; }
        set
        {
            radius = value;

            var renderer = GetComponent<Renderer>();
            var material = renderer.sharedMaterial;

            var multiplier = 1f + Solve();
            material.SetFloat(SIZE_MULTIPLIER_KEY, multiplier);
        }
    }

    private float Solve()
    {
        //y = 8000x^2 - 200x + 10

        var a = 8000;
        var b = 200;
        var c = 10 - radius;

        var discriminant = Mathf.Pow(b, 2) - 4 * a * c;

        if (discriminant < 0)
            return 0;

        float x1;
        float x2;

        if (discriminant == 0)
        {
            x1 = -b / (2 * a);
            x2 = x1;
        }
        else
        {
            x1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            x2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        }

        return x1 > x2 ? x1 : x2;
    }
}
