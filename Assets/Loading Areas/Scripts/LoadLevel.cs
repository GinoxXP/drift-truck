using UnityEngine;
using Zenject;

public class LoadLevel : ALoadScene
{
    private readonly string LEVEL_KEY = "Level{0}_{1}";

    [SerializeField]
    private int chapter;
    [SerializeField]
    private int level;

    private SaveSystem saveSystem;

    public int Chapter => chapter;
    public int Level => level;

    public override void Load(bool isPermanent = false)
    {
        saveSystem.SetLevelAccessState(chapter, level, true);
        levelName = string.Format(LEVEL_KEY, chapter, level);
        base.Load(isPermanent);
    }

    [Inject]
    private void Init(SaveSystem saveSystem)
    {
        this.saveSystem = saveSystem;
    }
}
