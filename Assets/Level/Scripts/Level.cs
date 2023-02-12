using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private IEnumerator reloadLevel;

    public void ReloadLevel(float time = 0)
    {
        if (reloadLevel != null)
        {
            StopCoroutine(reloadLevel);
            reloadLevel = null;
        }
        reloadLevel = ReloadLevelCoroutine(time);
        StartCoroutine(reloadLevel);
    }

    private IEnumerator ReloadLevelCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

        yield return null;
    }
}
