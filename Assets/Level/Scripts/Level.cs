using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private bool isChangingLevel;

    public void LoadLevel(string levelName, float time = 0)
    {
        if (isChangingLevel)
            return;

        isChangingLevel = true;
        StartCoroutine(LoadLevelCoroutine(levelName, time));
    }

    public void ReloadLevel(float time = 0)
    {
        if (isChangingLevel)
            return;

        isChangingLevel = true;
        StartCoroutine(ReloadLevelCoroutine(time));
    }

    private IEnumerator LoadLevelCoroutine(string levelName, float time = 0)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(levelName);

        yield return null;
    }

    private IEnumerator ReloadLevelCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

        yield return null;
    }
}
