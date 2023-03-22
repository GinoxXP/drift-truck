using TMPro;
using UnityEngine;

public class LoadingCargoIndicator : MonoBehaviour
{
    private Inventory inventory;
    private TMP_Text text;

    private void ChangeText()
    {
        text.text = $"{inventory.CurrentCount}";
    }

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        inventory = GetComponentInParent<Inventory>();

        inventory.CurentCountChanged += ChangeText;
        ChangeText();
    }

    private void OnDestroy()
    {
        inventory.CurentCountChanged -= ChangeText;

    }
}
