using UnityEngine;
using CrazyGames;

public class CrazyBannerDemoController : MonoBehaviour
{
    public CrazyBanner bannerPrefab;

    private void Awake()
    {
        CrazyAds.Instance.listenToBannerError(BannerError);
        CrazyAds.Instance.listenToBannerRendered(BannerRendered);
        var banners = FindObjectsOfType<CrazyBanner>();
        foreach (var banner in banners) banner.MarkVisible(true);
    }

    public void UpdateBannersDisplay()
    {
        CrazyAds.Instance.updateBannersDisplay();
    }

    public void DisableLastBanner()
    {
        var banners = FindObjectsOfType<CrazyBanner>();
        foreach (var banner in banners)
            if (banner.isVisible())
            {
                banner.MarkVisible(false);
                return;
            }
    }

    public void AddBanner()
    {
        var banner = Instantiate(bannerPrefab, new Vector3(), new Quaternion(), GameObject.Find("Banners").transform);
        banner.Size = (CrazyBanner.BannerSize) Random.Range(0, 3);
        banner.Position = new Vector2(Random.Range(-461, 461), Random.Range(-243, 243));
        banner.MarkVisible(true);
    }

    public void MarkAllForRefresh()
    {
        var banners = FindObjectsOfType<CrazyBanner>();
        foreach (var banner in banners) banner.MarkForRefresh();
    }

    private void BannerError(string id, string error)
    {
        Debug.Log("Banner error for id " + id + ": " + error);
    }

    private void BannerRendered(string id)
    {
        Debug.Log("Banner rendered for id " + id);
    }
}