using System;
using UnityEngine;
using UnityEngine.UI;

namespace CrazyGames
{
    public class CrazyBanner : MonoBehaviour
    {
        public enum BannerSize
        {
            Leaderboard_728x90,
            Medium_300x250,
            Mobile_320x50,
            Main_Banner_468x60,
            Large_Mobile_320x100
        }

        public string id;

        [SerializeField] private BannerSize _size;

        private bool visible;

        public BannerSize Size
        {
            get { return _size; }
            set
            {
                _size = value;
                var banner = (RectTransform)transform.Find("Banner");
                switch (value)
                {
                    case BannerSize.Mobile_320x50:
                        banner.sizeDelta = new Vector2(320, 50);
                        break;
                    case BannerSize.Medium_300x250:
                        banner.sizeDelta = new Vector2(300, 250);
                        break;
                    case BannerSize.Leaderboard_728x90:
                        banner.sizeDelta = new Vector2(728, 90);
                        break;
                    case BannerSize.Main_Banner_468x60:
                        banner.sizeDelta = new Vector2(468, 60);
                        break;
                    case BannerSize.Large_Mobile_320x100:
                        banner.sizeDelta = new Vector2(320, 100);
                        break;
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                var banner = (RectTransform)transform.Find("Banner");
                return banner.anchoredPosition;
            }
            set
            {
                var banner = (RectTransform)transform.Find("Banner");
                banner.anchoredPosition = value;
            }
        }

        private void Awake()
        {
            var image = transform.GetComponentInChildren<Image>();
            image.color = Color.clear;
            id = Guid.NewGuid().ToString();
            CrazyAds.Instance.registerBanner(this);
        }

        private void OnDestroy()
        {
            var image = transform.GetComponentInChildren<Image>();
            image.color = Color.clear;
            if (CrazyAds.Instance)
                CrazyAds.Instance.unregisterBanner(this);
        }

        public void SimulateRender()
        {
            if (visible)
            {
                var image = transform.GetComponentInChildren<Image>();
                image.color = new Color32(46, 37, 68, 255);
            }
            else
            {
                var image = transform.GetComponentInChildren<Image>();
                image.color = Color.clear;
            }
        }

        public void MarkForRefresh()
        {
            id = Guid.NewGuid().ToString();
        }

        public void MarkVisible(bool visibility)
        {
            visible = visibility;
        }

        public bool isVisible()
        {
            return visible;
        }
    }
}