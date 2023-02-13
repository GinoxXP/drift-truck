using System.Collections.Generic;
using UnityEngine;

public class CargoHolder : MonoBehaviour
{
    [SerializeField]
    private Transform cargoParent;
    [SerializeField]
    private Inventory inventory;

    private List<GameObject> cargoBoxes = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < cargoParent.childCount; i++)
        {
            var cargoBox = cargoParent.GetChild(i).gameObject;
            cargoBoxes.Add(cargoBox);

            if (i > inventory.CurrentCount)
                cargoBox.SetActive(false);
        }
    }
}
