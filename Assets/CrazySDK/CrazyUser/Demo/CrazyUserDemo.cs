using CrazyGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CrazyUserDemo : MonoBehaviour
{
    public Text userAccountAvailableText;
    private readonly List<Action<PortalUser>> authListeners = new List<Action<PortalUser>>();

    public void Start()
    {
        if (userAccountAvailableText != null)
        {
            CrazyUser.Instance.IsUserAccountAvailable(b => { userAccountAvailableText.text = "User account available: " + b; });
        }
    }

    public void GetUser()
    {
        CrazyUser.Instance.GetUser((user) =>
        {
            if (user != null)
            {
                Debug.Log("Get user result: " + user);
            }
            else
            {
                Debug.Log("User is not signed in");
            }
        });
    }

    public void GetToken()
    {
        CrazyUser.Instance.GetUserToken((error, token) =>
        {
            if (error != null)
            {
                Debug.LogError("Get user token error: " + error);
                return;
            }

            Debug.Log("User token: " + token);
        });
    }

    public void GetXsollaUserToken()
    {
        CrazyUser.Instance.GetXsollaUserToken((error, token) =>
        {
            if (error != null)
            {
                Debug.LogError("Get Xsolla user token error: " + error);
                return;
            }

            Debug.Log("Xsolla user token: " + token);
        });
    }

    public void SyncUnityGameData()
    {
        CrazyUser.Instance.SyncUnityGameData();
    }

    public void ShowAuthPrompt()
    {
        CrazyUser.Instance.ShowAuthPrompt((error, user) =>
        {
            if (error != null)
            {
                Debug.LogError("Show auth prompt error: " + error);
                return;
            }

            Debug.Log("Auth prompt user: " + user);
        });
    }

    public void ShowAccountLinkPrompt()
    {
        CrazyUser.Instance.ShowAccountLinkPrompt((error, answer) =>
        {
            if (error != null)
            {
                Debug.LogError("Show account link prompt error: " + error);
                return;
            }

            Debug.Log("Account link answer: " + answer);
        });
    }

    public void AddAuthListener()
    {
        authListeners.Add((user) => { Debug.Log("Auth listener, user: " + user); });
        CrazyUser.Instance.AddAuthListener(authListeners.Last());
    }

    public void RemoveLastAuthListener()
    {
        if (authListeners.Count == 0)
        {
            return;
        }

        var lst = authListeners.Last();
        CrazyUser.Instance.RemoveAuthListener(lst);
        authListeners.Remove(lst);
    }

    public void GetSystemInfo()
    {
        CrazySDK.Instance.GetSystemInfo(systemInfo =>
        {
            Debug.Log(systemInfo.countryCode);
            // For browser and os, format is the same as https://github.com/faisalman/ua-parser-js
            Debug.Log(systemInfo.browser.name);
            Debug.Log(systemInfo.browser.version);
            Debug.Log(systemInfo.os.name);
            Debug.Log(systemInfo.os.version);
            Debug.Log(systemInfo.device.type);
        });
    }

    public void CheckIsQaTool()
    {
        CrazySDK.Instance.IsQaTool(isQaTool => { Debug.Log("Is QA Tool: " + isQaTool); });
    }

    public void AddScore()
    {
        CrazyUser.Instance.AddScore(152.1f);
    }
}