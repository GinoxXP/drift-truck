using UnityEngine;

public class LoadScene : ALoadScene
{
    [SerializeField]
    protected new string levelName;

    private void Start()
    {
        base.levelName = levelName;
    }
}
