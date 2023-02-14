using UnityEngine;
using Zenject;

[RequireComponent(typeof(Inventory))]
public class LoadLevel : MonoBehaviour
{
    [SerializeField]
    private string levelName;

    private Inventory inventory;
    private Level level;

    private void OnInventoryCurentCountChanged()
    {
        if(inventory.CurrentCount >= inventory.MaxCount)
            level.LoadLevel(levelName);
    }

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        inventory.CurentCountChanged += OnInventoryCurentCountChanged;
    }

    private void OnDestroy()
    {
        inventory.CurentCountChanged -= OnInventoryCurentCountChanged;
    }

    [Inject]
    private void Init(Level level)
    {
        this.level = level;
    }
}
