using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(LoadLevel))]
[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    private SaveSystem saveSystem;
    private LoadLevel loadLevel;
    private Button button;

    [SerializeField]
    private GameObject lockImage;
    [SerializeField]
    private GameObject text;

    private void Start()
    {
        button = GetComponent<Button>();
        loadLevel = GetComponent<LoadLevel>();

        if (saveSystem.GetLevelAccessState(loadLevel.Chapter, loadLevel.Level))
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
