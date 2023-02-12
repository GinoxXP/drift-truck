using UnityEngine;
using Zenject;

public class Border : MonoBehaviour
{
    private Level level;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            level.ReloadLevel();
        }
    }

    [Inject]
    private void Init(Level level)
    {
        this.level = level;
    }
}
