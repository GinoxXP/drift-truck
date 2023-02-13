using System.Collections.Generic;
using UnityEngine;

public class CargoHolder : MonoBehaviour
{
    [SerializeField]
    private Transform cargoParent;
    [SerializeField]
    private Inventory inventory;

    private List<GameObject> cargoBoxes = new List<GameObject>();

    private void ChangeActiveBoxes()
    {
        for (int i = 0; i < cargoBoxes.Count; i++)
        {
            if (i < inventory.CurrentCount)
                cargoBoxes[i].SetActive(true);
            else
                cargoBoxes[i].SetActive(false);
        }
    }

    private void Start()
    {
        for (int i = 0; i < cargoParent.childCount; i++)
        {
            var cargoBox = cargoParent.GetChild(i).gameObject;
            cargoBoxes.Add(cargoBox);
        }

        ChangeActiveBoxes();

        inventory.CurentCountChanged += ChangeActiveBoxes;
    }

    private void OnDestroy()
    {
        inventory.CurentCountChanged -= ChangeActiveBoxes;
    }
}
