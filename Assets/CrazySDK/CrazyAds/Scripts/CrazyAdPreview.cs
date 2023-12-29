using UnityEngine;

public class CrazyAdPreview : MonoBehaviour
{
#if UNITY_EDITOR
    public string labelText;
    private readonly GUIStyle guiStyle = new GUIStyle();

    private void Start()
    {
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontSize = 18;
    }

    private void OnGUI()
    {
        GUI.color = new Color(0, 0, 0, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), labelText, guiStyle);
    }
#endif
}