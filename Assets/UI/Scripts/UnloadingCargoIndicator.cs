using TMPro;
using UnityEngine;

public class UnloadingCargoIndicator : MonoBehaviour
{
    private Inventory inventory;
    private TMP_Text text;

    private void ChangeText()
    {
        text.text = $"{inventory.CurrentCount}/{inventory.MaxCount}";
    }

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        inventory = GetComponentInParent<Inventory>();

        inventory.CurentCountChanged += ChangeText;
    }

    private void OnDestroy()
    {
        inventory.CurentCountChanged -= ChangeText;

    }
}
