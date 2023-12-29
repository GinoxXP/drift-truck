using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrazyGames
{
    public enum CrazyAdType
    {
        midgame,
        rewarded
    }

    public enum CrazySDKEvent
    {
        adStarted, // fired when ad starts playing
        adFinished, // fired when ad has finished (either when completed or when user pressed skip)
        adCompleted, // fired when user has completely watched the ad
        adError, // fired when an error occurs, also fired when no ad is available
        adblockDetectionExecuted, // fired when adblock detection has run
        inGameBannerError, // fired when an error happened when rendering in-game banners
        inGameBannerRendered, // fired when a banner has been rendered
    }

    public class CrazySDK : Singleton<CrazySDK>
    {
        public static string sdkVersion = "4.16.0";
        public bool debug;
        private bool adblockDetectionExecuted;

        private Dictionary<CrazySDKEvent, List<EventCallback>> eventListeners;
        private bool hasAdblock;
        private bool noGameframe;
        private readonly List<Action<SystemInfo>> getSystemInfoCallbacks = new List<Action<SystemInfo>>();
        private readonly List<Action<bool>> isQaToolCallbacks = new List<Action<bool>>();
        private bool isOnWhitelistedDomain;

        public InitializationObject InitObj { get; private set; }
        public bool IsInitialized => InitObj != null;


#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern bool InitSDK(string version, string objectName);

        [DllImport("__Internal")]
        private static extern void RequestAdSDK(string str);

        [DllImport("__Internal")]
        private static extern void HappyTimeSDK();

        [DllImport("__Internal")]
        private static extern void GameplayStartSDK();

        [DllImport("__Internal")]
        private static extern void GameplayStopSDK();

        [DllImport("__Internal")]
        private static extern void RequestInviteUrlSDK(string url);

        [DllImport("__Internal")]
        private static extern void ShowInviteButtonSDK(string url);

        [DllImport("__Internal")]
        private static extern void HideInviteButtonSDK();

        [DllImport("__Internal")]
        private static extern void CopyToClipboardSDK(string text);

        [DllImport("__Internal")]
        private static extern string GetUrlParametersSDK();

        [DllImport("__Internal")]
        private static extern string RequestBannersSDK(string bannerRequestJSON);
#else
        // Preventing build to fail when using another platform than WebGL
        private bool InitSDK(string version, string objectName) { return false; }
        private void RequestAdSDK(string str) { }
        private void HappyTimeSDK() { }
        private void GameplayStartSDK() { }
        private void GameplayStopSDK() { }
        private void RequestInviteUrlSDK(string url) { }
        private void ShowInviteButtonSDK(string url) { }
        private void HideInviteButtonSDK() { }
        private void CopyToClipboardSDK(string text) { }
        private string GetUrlParametersSDK() { return ""; }
        private string RequestBannersSDK(string bannerRequestJSON) { return ""; }
#endif

        public void Awake()
        {
            if (SiteLock.IsSiteLocked)
            {
                // when the SiteLock will call the crash method from here, prevent SDK initialization
                return;
            }

#if !UNITY_EDITOR && UNITY_WEBGL
            if (!SiteLock.DidRun)
            {
                SiteLockFreezeGame(4);
                return;
            }
#endif

            eventListeners = new Dictionary<CrazySDKEvent, List<EventCallback>>();

#if (!UNITY_EDITOR)
            isOnWhitelistedDomain = SiteLock.IsOnWhitelistedDomain();
            debug = GetUrlParameter("sdk_debug") == "true";
            noGameframe = !InitSDK(sdkVersion, "CrazyGames.CrazySDK");
            if (noGameframe)
            {
                DebugLog("No Crazygames gameframe detected, cannot run SDK.");
            }
            else
            {
                if (Debug.isDebugBuild)
                    Debug.LogWarning("[CrazySDK] Development Build");
            }
#else
            isOnWhitelistedDomain = false;
            debug = true;
            var deviceType = "desktop";
            var settings = GetSettings();
            if (settings != null)
            {
                switch (settings.deviceType)
                {
                    case CrazySettingsDeviceType.tablet:
                        deviceType = "tablet";
                        break;
                    case CrazySettingsDeviceType.mobile:
                        deviceType = "mobile";
                        break;
                    case CrazySettingsDeviceType.desktop:
                    default:
                        deviceType = "desktop";
                        break;
                }
            }

            InitObj = new InitializationObject
            {
                gameLink = "https://www.crazygames.com/game/yourFabulousGameHere",
                locale = "en-US",
                systemInfo =
                    new SystemInfo
                    {
                        countryCode = "demo",
                        browser = new Software { name = "demo", version = "demo" },
                        os = new Software { name = "demo", version = "demo" },
                        device = new Device { type = deviceType }
                    },
                userAccountAvailable = true,
                isQaTool = false
            };
            CallInitCallbacks();
            DebugLog("Running into Unity Editor, Crazygames SDK is emulated");
#endif
        }

        #region init

        // Called from jslib
        public void InitCallback(string initObjJSON)
        {
            DebugLog("Init callback from gameframe");
            DebugLog("Init obj: " + initObjJSON);
            InitObj = JsonUtility.FromJson<InitializationObject>(initObjJSON);
            if (string.IsNullOrEmpty(InitObj.locale))
            {
                DebugLog("The locale wasn't provided in the InitCallback, using 'en-US'");
                InitObj.locale = "en-US";
            }
            else
            {
                // Yandex sends locale  in this format "en-US", html5 sdk will send it in this format "en_US" 
                InitObj.locale = InitObj.locale.Replace("_", "-");
            }

            CallInitCallbacks();
        }

        private void CallInitCallbacks()
        {
            getSystemInfoCallbacks.ForEach(cb => { cb?.Invoke(InitObj.systemInfo); });
            getSystemInfoCallbacks.Clear();
            isQaToolCallbacks.ForEach(cb => { cb?.Invoke(InitObj.isQaTool); });
            isQaToolCallbacks.Clear();
            CrazyUser.Instance.CallInitCallbacks();
        }

        #endregion

        private CrazySettings crazySettings;
        private bool settingsLoaded;

        public CrazySettings GetSettings()
        {
            if (settingsLoaded)
            {
                return crazySettings;
            }

            settingsLoaded = true;
            crazySettings = Resources.Load<CrazySettings>("CrazyGamesSettings");
            if (crazySettings == null)
            {
                Debug.LogError(
                    "Missing CrazySDK/Resources/CrazyGamesSettings file. Please be sure you imported the SDK correctly.");
                return null;
            }

            return crazySettings;
        }

        #region callableActions

        public void RequestAd(CrazyAdType adType = CrazyAdType.midgame)
        {
            DebugLog("Requesting " + adType + " ad");
#if (!UNITY_EDITOR)
            WrapGFFeature(() => { RequestAdSDK(adType.ToString()); });
#else
            AdEvent("adError");
#endif
        }

        public void HappyTime()
        {
            DebugLog("Happy time!");

#if (!UNITY_EDITOR)
            WrapGFFeature(HappyTimeSDK);
#endif
        }

        public void GameplayStart()
        {
            DebugLog("Gameplay start called");

#if (!UNITY_EDITOR)
		    WrapGFFeature(GameplayStartSDK);
#endif
        }

        public void GameplayStop()
        {
            DebugLog("Gameplay stop called");

#if (!UNITY_EDITOR)
		    WrapGFFeature(GameplayStopSDK);
#endif
        }

        private string GenerateInviteLink(Dictionary<string, string> parameters)
        {
            var queryFromParams = parameters
                .Select(parameter => string.Format("{0}={1}", parameter.Key, parameter.Value)).Aggregate(
                    "utm_source=invite", (current, paramString) => string.Format("{0}&{1}", current, paramString));
            var gameLink = InitObj.gameLink;
            var template = gameLink.Contains("?") ? "{0}&{1}" : "{0}?{1}";
            return string.Format(template, gameLink, queryFromParams);
        }

        public string InviteLink(Dictionary<string, string> parameters)
        {
            DebugLog("Invite link called");

            if (InitObj == null)
            {
                DebugLog("Initialization object not received from gameframe, cannot generate invite link");
                return null;
            }

            var inviteLink = GenerateInviteLink(parameters);
            DebugLog("Invite link is " + inviteLink);

#if (!UNITY_EDITOR)
            WrapGFFeature(() => { RequestInviteUrlSDK(inviteLink); });
#else
            Debug.Log("Invite link simulation for unity editor");
#endif
            return inviteLink;
        }

        public string ShowInviteButton(Dictionary<string, string> parameters)
        {
            DebugLog("Show invite button called");

            if (InitObj == null)
            {
                DebugLog("Initialization object not received from gameframe, cannot generate invite link");
                return null;
            }

            var inviteLink = GenerateInviteLink(parameters);
            DebugLog("Invite button link is " + inviteLink);

#if (!UNITY_EDITOR)
            WrapGFFeature(() => { ShowInviteButtonSDK(inviteLink); });
#else
            Debug.Log("Invite button simulation for unity editor");
#endif
            return inviteLink;
        }

        public void HideInviteButton()
        {
            DebugLog("Hide invite button called");
#if (!UNITY_EDITOR)
            WrapGFFeature(() => { HideInviteButtonSDK(); });
#endif
        }

        public void CopyToClipboard(string text)
        {
#if (!UNITY_EDITOR)
		    CopyToClipboardSDK(text);
#else
            GUIUtility.systemCopyBuffer = text;
#endif
        }

        public bool IsInviteLink()
        {
#if (!UNITY_EDITOR)
            var utmSource = GetUrlParameter("utm_source");
            return utmSource == "invite";
#else
            Debug.Log("Cannot parse url in Unity editor, try running it in a browser");
            return false;
#endif
        }

        public string GetInviteLinkParameter(string key)
        {
#if (!UNITY_EDITOR)
            return GetUrlParameter(key);
#else
            Debug.Log("Cannot parse url in Unity editor, try running it in a browser");
            return "";
#endif
        }

        public void RequestBanners(Banner[] banners)
        {
            DebugLog("Requesting banners to HTML SDK v2" + CrazyJsonHelper.ToJson(banners));
#if (!UNITY_EDITOR)
            WrapGFFeature(() => { RequestBannersSDK(CrazyJsonHelper.ToJson(banners)); });
#else
            Debug.Log("Cannot render banners inside Unity editor");
#endif
        }

        #endregion


        #region eventManagement

        public delegate void EventCallback(SDKGFEvent ev);

        public void AddEventListener(CrazySDKEvent eventType, EventCallback callback)
        {
            DebugLog("Adding event listener for " + eventType);

            if (!eventListeners.ContainsKey(eventType)) eventListeners.Add(eventType, new List<EventCallback>());

            eventListeners[eventType].Add(callback);
        }

        public void RemoveEventListener(CrazySDKEvent eventType, EventCallback callback)
        {
            DebugLog("Removing event listener for " + eventType);

            if (eventListeners.ContainsKey(eventType)) eventListeners[eventType].Remove(callback);
        }

        public void RemoveEventListenersForEvent(CrazySDKEvent eventType)
        {
            DebugLog("Removing all event listener for " + eventType);

            eventListeners.Remove(eventType);
        }

        public void RemoveAllEventListeners()
        {
            DebugLog("Removing all event listeners");

            eventListeners.Clear();
        }

        // Called from jslib
        public void AdEvent(string eventJSON)
        {
            DebugLog("Received event from gameframe");
            DebugLog(eventJSON);
            var receivedEvent = JsonUtility.FromJson<GFEvent>(eventJSON);

            var parsedEventType =
                (CrazySDKEvent)Enum.Parse(typeof(CrazySDKEvent), receivedEvent.name);
            SDKGFEvent evt;
            switch (parsedEventType)
            {
                case CrazySDKEvent.inGameBannerError:
                    var errorEv = JsonUtility.FromJson<GFBannerErrorEvent>(eventJSON);
                    evt = new BannerErrorEvent(errorEv.id, errorEv.error);
                    break;
                case CrazySDKEvent.inGameBannerRendered:
                    evt = new BannerRenderedEvent(JsonUtility.FromJson<GFBannerRenderedEvent>(eventJSON).id);
                    break;
                case CrazySDKEvent.adError:
                    var adErrorEv = JsonUtility.FromJson<GFAdErrorEvent>(eventJSON);
                    evt = new AdErrorEvent(adErrorEv.message, adErrorEv.reason);
                    break;
                default:
                    evt = new SDKGFEvent(parsedEventType);
                    break;
            }

            CallCallbacks(evt);
        }

        private void CallCallbacks(SDKGFEvent ev)
        {
            DebugLog("Calling callbacks for event " + ev.type);
            if (!eventListeners.ContainsKey(ev.type)) return;
            foreach (var callback in eventListeners[ev.type]) callback(ev);
        }

        #endregion


        #region adblock

        public bool HasAdblock()
        {
            if (!adblockDetectionExecuted) DebugLog("Adblock detection has not finished");

            return hasAdblock;
        }

        public bool AdblockDetectionExecuted()
        {
            return adblockDetectionExecuted;
        }

        // Called from jslib
        public void AdblockDetected()
        {
            Adblock(true);
        }

        // Called from jslib
        public void AdblockNotDetected()
        {
            Adblock(false);
        }

        private void Adblock(bool detected)
        {
            DebugLog("Adblock detection executed");
            DebugLog("Adblock present: " + detected);
            adblockDetectionExecuted = true;
            hasAdblock = detected;
            CallCallbacks(new SDKGFEvent(CrazySDKEvent.adblockDetectionExecuted));
        }

        #endregion


        #region utils

        private string GetUrlParameter(string key)
        {
            var _regex = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");
            var paramsStr = GetUrlParametersSDK();
            var match = _regex.Match(paramsStr);
            var parameters = new Dictionary<string, string>();
            while (match.Success)
            {
                parameters.Add(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }

            return parameters.ContainsKey(key) ? parameters[key] : null;
        }

        public void WrapGFFeature(Action action)
        {
            if (noGameframe)
            {
                DebugLog("Cannot call Crazygames SDK features as gameframe hasn't been detected.");
                return;
            }

            action();
        }

        public void DebugLog(string msg)
        {
            if (isOnWhitelistedDomain && noGameframe)
            {
                // some devs don't want logs from the SDK on their domains, so to be sure, completely disable all logging when running on whitelisted domains if there isn't a gameframe
                return;
            }

            if (Application.isEditor && GetSettings().disableSdkLogs)
            {
                return;
            }

            if (Debug.isDebugBuild || debug) Debug.Log("[CrazySDK] " + msg);
        }

        #endregion

        /// <summary>
        /// This method was previously called GetUserInfo.
        /// </summary>
        /// <param name="callback"></param>
        public void GetSystemInfo(Action<SystemInfo> callback)
        {
            if (IsInitialized)
            {
                callback(InitObj.systemInfo);
            }
            else
            {
                getSystemInfoCallbacks.Add(callback);
            }
        }

        public void IsQaTool(Action<bool> callback)
        {
            if (IsInitialized)
            {
                callback(InitObj.isQaTool);
            }
            else
            {
                isQaToolCallbacks.Add(callback);
            }
        }

        public string Locale
        {
            get
            {
                if (InitObj != null) return InitObj.locale;
                DebugLog("CrazySDK is not initialized yet, returning default locale 'en-US'");
                return "en-US";
            }
        }

        /**
         * Returns 'true' if the game is running on CrazyGames domains, or on our partner websites embedding the game.
         * Also returns 'true' in the Editor.
         * Returns 'false' when the game is running on any whitelisted domains in CrazyGamesSettings file.
         */
        public static bool IsOnCrazyGames => Application.isEditor || (Application.platform == RuntimePlatform.WebGLPlayer && !SiteLock.IsOnWhitelistedDomain());

        /**
         * Redirects can be prevented, so this method starts an infinite loop to freeze the game if it was stolen.
         */
        public void SiteLockFreezeGame(int code)
        {
            StartCoroutine(SiteLockFreezeGameCoroutine(code));
        }

        private IEnumerator SiteLockFreezeGameCoroutine(int code)
        {
            Debug.Log($"SiteLock v{SiteLock.sitelockVersion} activated, please check CrazySDK Unity docs for more info. Code {code}");

            // this wait is required so the crash log manages to reach the browser console
            yield return new WaitForSecondsRealtime(0.5f);

            Crash(0);
        }

        private static void Crash(int i)
        {
            Crash(i++);
        }
    }

    [Serializable]
    public class Banner
    {
        public string id;
        public string size;
        public Vector2 position;
        public Vector2 anchor;
        public Vector2 pivot;

        public Banner(string id, string size, Vector2 position, Vector2 anchor, Vector2 pivot)
        {
            this.id = id;
            this.size = size;
            this.position = position;
            this.anchor = anchor;
            this.pivot = pivot;
        }
    }

    [Serializable]
    public class GFEvent
    {
        public string name;
    }

    [Serializable]
    public class GFAdErrorEvent : GFEvent
    {
        public string message;
        public string reason;
    }

    [Serializable]
    public class GFBannerErrorEvent : GFEvent
    {
        public string id;
        public string error;
    }

    [Serializable]
    public class GFBannerRenderedEvent : GFEvent
    {
        public string id;
    }

    public class SDKGFEvent
    {
        public CrazySDKEvent type;

        public SDKGFEvent(CrazySDKEvent type)
        {
            this.type = type;
        }
    }

    public class AdErrorEvent : SDKGFEvent
    {
        public string message;
        public string reason;

        public AdErrorEvent(string message, string reason) : base(CrazySDKEvent.adError)
        {
            this.message = message;
            this.reason = reason;
        }
    }

    public class BannerErrorEvent : SDKGFEvent
    {
        public readonly string error;
        public readonly string id;

        public BannerErrorEvent(string id, string error) : base(CrazySDKEvent.inGameBannerError)
        {
            this.id = id;
            this.error = error;
        }
    }

    public class BannerRenderedEvent : SDKGFEvent
    {
        public string id;

        public BannerRenderedEvent(string id) : base(CrazySDKEvent.inGameBannerRendered)
        {
            this.id = id;
        }
    }

    [Serializable]
    public class InitializationObject
    {
        public string gameLink;
        public string locale;
        public SystemInfo systemInfo;
        public bool userAccountAvailable;
        public bool isQaTool;
    }

    [Serializable]
    public class SystemInfo
    {
        public string countryCode;
        public Software browser;
        public Software os;
        public Device device;
    }

    [Serializable]
    public class Software
    {
        public string name;
        public string version;
    }

    [Serializable]
    public class Device
    {
        /**
         * Possible values: "desktop", "tablet", "mobile"
         */
        public string type;
    }
}