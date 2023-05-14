using UnityEngine;
using Zenject;

public class LoadLevel : MonoBehaviour
{
    private const float LOAD_LEVEL_DELAY = 1.5f;

    [SerializeField]
    private string levelName;

    private Level level;

    public string LevelName => levelName;

    public void Load(bool isPermanent = false)
    {
        level.LoadLevel(levelName, isPermanent ? 0 : LOAD_LEVEL_DELAY);
    }

    [Inject]
    private void Init(Level level)
    {
        this.level = level;
    }
}
