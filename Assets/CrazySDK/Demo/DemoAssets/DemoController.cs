using System.Collections;
using System.Collections.Generic;
using CrazyGames;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoController : MonoBehaviour
{

    public Text adblockText;

    public void ChangeDemoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMainDemo()
    {
        SceneManager.LoadScene("MainDemo");
    }

    public void LockPointer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (CrazySDK.Instance.AdblockDetectionExecuted() && adblockText != null)
        {
            adblockText.text = "Has adblock: " + CrazySDK.Instance.HasAdblock().ToString();
        }
    }
}