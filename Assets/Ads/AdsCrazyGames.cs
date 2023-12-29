using CrazyGames;

public class AdsCrazyGames : IAds
{
    public void ShowVideoAd()
    {
        CrazyAds.Instance.beginAdBreak();
    }
}
