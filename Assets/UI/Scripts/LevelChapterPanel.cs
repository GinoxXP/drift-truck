using UnityEngine;
using UnityEngine.Events;

public class LevelChapterPanel : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onEnableAction;

    private void OnEnable()
    {
        onEnableAction?.Invoke();
    }
}
