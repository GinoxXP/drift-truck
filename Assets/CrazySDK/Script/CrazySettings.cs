using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CrazyGames
{
    /**
    * Contains public data that can be modified by the game developers that use the SDK.
    */
    public class CrazySettings : ScriptableObject
    {
        /**
         * SiteLock doesn't block these domains, and the SDK is not initialized here. Used for the domains where game developers host their games.
         */
        [Tooltip("SiteLock doesn't block these domains. Here you can add the domains where your game is hosted.")]
        public string[] whitelistedDomains;

        [Header("User account testing (editor only)")]
        [Tooltip("Select the response you want to receive when calling CrazyUser.Instance.IsUserAccountAvailable")]
        public CrazySettingsAccountAvailableResponse accountAvailableResponse;

        [Tooltip("Select the response you want to receive when calling CrazyUser.Instance.ShowAccountLinkPrompt")]
        public CrazySettingsLinkAccountResponse linkAccountResponse;

        [Tooltip("Select the response you want to receive when calling CrazyUser.Instance.GetUser")]
        public CrazySettingsTestUser getUserResponse;

        [Tooltip("Select the response you want to receive when calling CrazyUser.Instance.ShowAuthPrompt")]
        public CrazySettingsAuthPromptResponse authPromptResponse;

        [Tooltip("Select the response you want to receive when calling CrazyUser.Instance.GetToken")]
        public CrazySettingsGetTokenResponse getTokenResponse;

        [Header("Other settings (editor only)")]
        [Tooltip("You can disable the SDK logs in the editor if you find them annoying. Some important logs will still be displayed.")]
        public bool disableSdkLogs;

        [Tooltip("You can disable the video ad previews in the editor if you find them annoying.")]
        public bool disableAdPreviews;

        [Tooltip("You can change what device type the systemInfo.device.type returns in the editor. See the docs for more info.")]
        public CrazySettingsDeviceType deviceType;
    }

    public enum CrazySettingsAccountAvailableResponse
    {
        Yes,
        No,
    }

    public enum CrazySettingsLinkAccountResponse
    {
        Yes,
        No,
        UserLoggedOut
    }

    public enum CrazySettingsTestUser
    {
        User1,
        User2,
        UserLoggedOut
    }

    public enum CrazySettingsAuthPromptResponse
    {
        User1,
        User2,
        UserCancelled
    }

    public enum CrazySettingsGetTokenResponse
    {
        User1,
        User2,
        UserLoggedOut,
        ExpiredToken
    }

    public enum CrazySettingsDeviceType
    {
        desktop,
        tablet,
        mobile
    }
}