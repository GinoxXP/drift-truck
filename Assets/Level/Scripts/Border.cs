using UnityEngine;
using Zenject;

public class Border : MonoBehaviour
{
    private const float PAUSE_BEFORE_RELOAD_LEVEL = 1.5f;

    private Level level;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Car>(out var car))
        {
            car.Crash();
            level.ReloadLevel(PAUSE_BEFORE_RELOAD_LEVEL);
        }
    }

    [Inject]
    private void Init(Level level)
    {
        this.level = level;
    }
}
