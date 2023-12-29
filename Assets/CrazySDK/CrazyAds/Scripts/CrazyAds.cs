// ReSharper disable once RedundantUsingDirective

using System;
// ReSharper disable once RedundantUsingDirective
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CrazyGames
{
    public class CrazyAds : Singleton<CrazyAds>
    {
        public delegate void AdBreakCompletedCallback();

        private AdBreakCompletedCallback onCompletedAdBreak;

        public delegate void AdErrorCallback();

        private AdErrorCallback onAdError;

        private bool origRunInBackground;
        private float origTimeScale;
        private float origAudioListenerVolume;
        private List<CrazyBanner> banners;

        private bool _adRequestInProgress;

        private void Awake()
        {
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.adError, ev => AdError(((AdErrorEvent)ev).message, ( (AdErrorEvent)ev).reason));
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.adFinished, ev => AdFinished());
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.adStarted, ev => AdStarted());
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.inGameBannerError,
                ev => BannerError(((BannerErrorEvent)ev).error));
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.inGameBannerRendered,
                ev => BannerRendered(((BannerRenderedEvent)ev).id));
            banners = new List<CrazyBanner>();
        }

        #region adbreak

        public void beginAdBreakRewarded(AdBreakCompletedCallback completedCallback = null,
            AdErrorCallback errorCallback = null)
        {
            beginAdBreak(completedCallback, errorCallback, CrazyAdType.rewarded);
        }

        public void beginAdBreak(AdBreakCompletedCallback completedCallback = null,
            AdErrorCallback errorCallback = null,
            CrazyAdType adType = CrazyAdType.midgame)
        {
#if !UNITY_WEBGL
            return;
#endif
            
            if (_adRequestInProgress)
            {
                CrazySDK.Instance.DebugLog("Ad request in progress, ignore " + adType + " request.");
                return;
            }

            CrazySDK.Instance.DebugLog("Requesting CrazyAd Type: " + adType);

            onCompletedAdBreak = completedCallback;
            onAdError = errorCallback;
            _adRequestInProgress = true;

            origTimeScale = Time.timeScale;
            origAudioListenerVolume = AudioListener.volume;
            origRunInBackground = Application.runInBackground;

            Time.timeScale = 0;
            AudioListener.volume = 0;
            Application.runInBackground = true;

#if UNITY_EDITOR
            SimulateAdBreak(adType);
#else
        CrazySDK.Instance.RequestAd(adType);
#endif
        }

#if UNITY_EDITOR
        private IEnumerator InvokeRealtimeCoroutine(UnityAction action, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action();
        }

        private void SimulateAdBreak(CrazyAdType adType)
        {
            var adTypeStr = adType.ToString();

            if (CrazySDK.Instance.GetSettings().disableAdPreviews)
            {
                Debug.Log("[CrazySDK] " + char.ToUpper(adTypeStr[0]) + adTypeStr.Substring(1) +
                          " ad displayed successfully (simulation is disabled in CrazyGamesSettings).");
                completedAdRequest(CrazySDKEvent.adCompleted);
                return;
            }

            Debug.Log("CrazyAds: Editor ad simulation, game will resume in 3 seconds");
            var adPreview = new GameObject("CrazyAdPreview").AddComponent<CrazyAdPreview>();
            adPreview.labelText = char.ToUpper(adTypeStr[0]) + adTypeStr.Substring(1) + " ad simulation, the game will resume in 3 seconds";

            AdStarted();
            StartCoroutine(InvokeRealtimeCoroutine(EndSimulation, 3));
        }

        private void EndSimulation()
        {
            DestroyImmediate(GameObject.Find("CrazyAdPreview"));
            CrazySDK.Instance.DebugLog("Ad Finished");
            completedAdRequest(CrazySDKEvent.adCompleted);
        }
#endif


        private void completedAdRequest(CrazySDKEvent e)
        {
            _adRequestInProgress = false;

            Time.timeScale = origTimeScale;
            AudioListener.volume = origAudioListenerVolume;
            Application.runInBackground = origRunInBackground;

            if (e == CrazySDKEvent.adError && onAdError != null) onAdError.Invoke();
            else if (onCompletedAdBreak != null) onCompletedAdBreak.Invoke();
        }

        private void AdError(string message, string reason)
        {
            CrazySDK.Instance.DebugLog("Ad Error: " + message + " Reason:" + reason);
            completedAdRequest(CrazySDKEvent.adError);
        }

        private void AdFinished()
        {
            CrazySDK.Instance.DebugLog("Ad Finished");
            completedAdRequest(CrazySDKEvent.adFinished);
        }

        private void AdStarted()
        {
            CrazySDK.Instance.DebugLog("Ad Started");
        }

        #endregion

        #region Banners

        public void updateBannersDisplay()
        {
#if UNITY_EDITOR
            foreach (var banner in banners) banner.SimulateRender();
#else
        var visibleBanners = banners.Where(b => b.isVisible()).Select((crazyBanner) =>
        {
            var size = "";
            switch (crazyBanner.Size)
            {
                case CrazyBanner.BannerSize.Leaderboard_728x90:
                    size = "728x90";
                    break;
                case CrazyBanner.BannerSize.Medium_300x250:
                    size = "300x250";
                    break;
                case CrazyBanner.BannerSize.Mobile_320x50:
                    size = "320x50";
                    break;
                case CrazyBanner.BannerSize.Large_Mobile_320x100:
                    size = "320x100";
                    break;
                case CrazyBanner.BannerSize.Main_Banner_468x60:
                    size = "468x60";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var bannerTransform = (RectTransform)crazyBanner.transform.Find("Banner");
            var anchor = new Vector2(
                (bannerTransform.anchorMin.x + bannerTransform.anchorMax.x) / 2,
                (bannerTransform.anchorMin.y + bannerTransform.anchorMax.y) / 2
            );
            return new Banner(
                crazyBanner.id,
                size,
                crazyBanner.Position,
                anchor,
                bannerTransform.pivot
            );
        }).ToArray();
        CrazySDK.Instance.RequestBanners(visibleBanners);
#endif
        }

        public void registerBanner(CrazyBanner banner)
        {
            if (!banners.Contains(banner))
                banners.Add(banner);
        }

        public void unregisterBanner(CrazyBanner banner)
        {
            banners.Remove(banner);
        }

        public delegate void BannerRenderedCallback(string id);

        public void listenToBannerRendered(BannerRenderedCallback callback)
        {
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.inGameBannerRendered, ev =>
            {
                var renderedEv = (BannerRenderedEvent)ev;
                callback(renderedEv.id);
            });
        }

        public delegate void BannerErrorCallback(string id, string error);

        public void listenToBannerError(BannerErrorCallback callback)
        {
            CrazySDK.Instance.AddEventListener(CrazySDKEvent.inGameBannerError, ev =>
            {
                var errorEv = (BannerErrorEvent)ev;
                callback(errorEv.id, errorEv.error);
            });
        }

        private void BannerError(string error)
        {
            CrazySDK.Instance.DebugLog("Banner error: " + error);
        }

        private void BannerRendered(string id)
        {
            CrazySDK.Instance.DebugLog("Banner with id " + id + " successfully rendered");
        }

        #endregion
    }
}