using UnityEngine;
using Zenject;

public class ALoadScene : MonoBehaviour
{
    private const float LOAD_LEVEL_DELAY = 1.5f;

    protected string levelName;

    private Level level;

    public string LevelName => levelName;

    public virtual void Load(bool isPermanent = false)
    {
        level.LoadLevel(levelName, isPermanent ? 0 : LOAD_LEVEL_DELAY);
    }

    [Inject]
    private void Init(Level level)
    {
        this.level = level;
    }
}
