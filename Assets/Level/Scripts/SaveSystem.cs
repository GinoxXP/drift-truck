using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private readonly string LEVEL_KEY = "LevelMaskChapter";

    public bool GetLevelAccessState(int chapter, int level)
    {
        var mask = GetLevelChapterMask(chapter);
        return mask[level-1] == '1';
    }

    public void SetLevelAccessState(int chapter, int level, bool state)
    {
        var mask = GetLevelChapterMask(chapter);

        if (mask == string.Empty)
            mask = "0000000000";

        mask = mask.Insert(level-1, state ? "1" : "0");
        mask = mask.Remove(level, 1);

        PlayerPrefs.SetString($"{LEVEL_KEY}{chapter}", mask);
    }

    private string GetLevelChapterMask(int chapter)
    {
        var mask = PlayerPrefs.GetString($"{LEVEL_KEY}{chapter}", string.Empty);

        if (mask == string.Empty)
            mask = "0000000000";

        return mask;
    }

    private void Start()
    {
        SetLevelAccessState(1, 1, true);
    }
}
