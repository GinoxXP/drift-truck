using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(LoadScene))]
[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    private SaveSystem saveSystem;
    private LoadScene loadLevel;
    private Button button;

    [SerializeField]
    private GameObject lockImage;
    [SerializeField]
    private GameObject text;

    private void Start()
    {
        button = GetComponent<Button>();
        loadLevel = GetComponent<LoadScene>();

        var levelName = loadLevel.LevelName;
        var chapterLevelString = levelName.Replace("Level", string.Empty);
        var chapter = int.Parse(chapterLevelString.Split("_")[0]);
        var level = int.Parse(chapterLevelString.Split("_")[1]);

        if (saveSystem.GetLevelAccessState(chapter, level))
        {
            lockImage.SetActive(false);
        }
        else
        {
            button.enabled = false;
            text.SetActive(false);
        }
    }

    [Inject]
    private void Init(SaveSystem saveSystem)
    {
        this.saveSystem = saveSystem;
    }
}
