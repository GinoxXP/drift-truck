using UnityEngine;
using Zenject;

public class LoadLevel : MonoBehaviour
{
    private const float LOAD_LEVEL_DELAY = 1.5f;

    [SerializeField]
    private string levelName;

    private Level level;

    public void Load()
    {
        level.LoadLevel(levelName, LOAD_LEVEL_DELAY);
    }

    [Inject]
    private void Init(Level level)
    {
        this.level = level;
    }
}
