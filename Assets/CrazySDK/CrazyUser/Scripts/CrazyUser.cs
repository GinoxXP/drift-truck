using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CrazyGames
{
    public class CrazyUser : Singleton<CrazyUser>
    {
        private readonly List<Action<PortalUser>> getUserCallbacks = new List<Action<PortalUser>>();
        private readonly List<Action<PortalUser>> authListeners = new List<Action<PortalUser>>();
        private readonly List<Action<string, string>> getUserTokenCallbacks = new List<Action<string, string>>();
        private readonly List<Action<string, string>> getXsollaUserTokenCallbacks = new List<Action<string, string>>();
        private Action<string, PortalUser> authPromptCallback;
        private Action<string, bool> accountLinkPromptCallback;
        private readonly List<Action<bool>> getUserAccountAvailableCallbacks = new List<Action<bool>>();

        private readonly PortalUser demoUser1 = new PortalUser()
        {
            username = "User1",
            profilePictureUrl = "https://images.crazygames.com/userportal/avatars/1.png"
        };

        private readonly PortalUser demoUser2 = new PortalUser()
        {
            username = "User2",
            profilePictureUrl = "https://images.crazygames.com/userportal/avatars/2.png"
        };

        private readonly string user1Token =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJpZFVzZXIxIiwidXNlcm5hbWUiOiJVc2VyMSIsImdhbWVJZCI6InlvdXJHYW1lSWQiLCJwcm9maWxlUGljdHVyZVVybCI6Imh0dHBzOi8vaW1hZ2VzLmNyYXp5Z2FtZXMuY29tL3VzZXJwb3J0YWwvYXZhdGFycy8xLnBuZyIsImlhdCI6MTY2ODU5MzMxNCwiZXhwIjo0ODI0Mjg4NTE0fQ.u4N2DzCC6Vmo6Gys-XSl8rHQR0NUJAcWQWr29eLd54qMDPbCopPG0kye8TAidOU6dWAqNWO_kERbl75nTxNcJjbW4yqBS_bIPingIhuCCJsjvnQPkalfmVotmoZGQP21Q9MyZPfpjZNogioA3a0vm6APXAqzudTA9lTioztnT4YvgndISngOMQVNoDCJ_DgCbKy8GFQDcCN-AHFFb0WIVWiTYszv-9JZohUzULt-ueYL31pXVGHQ5C4rHESEg7LYzx1IaLKw1zcoYGxul0RxR35nm3yD_bGa6fQVzCfnKnhEBRifUZsIN1LfIHfNB23ZOh1llG7PEOdvtCSfIxP9ZK6t4gRkGn1VsqZFAMhq1LgJebO8hcm_Iqx0wF3WkdMysoQuWThTNKnwmphv9pguuALILYJluUP8UQll3qiF6gzoLPy1MfD_9l4TPZeP9Bv50B-Tm6Lk3OW248jyuFRKP_VgwZutTb5pJ7LggFcqWFXsBv5ZG3V2zsfVwpAPDhpmb4ykjoB2xLSuxjrvs1dzMhl02QAQjqTUgHj4fstgX-2jYowDyyPjj6JbT2ZC7vrrdmPvc8AcNvyCszamfRYjexElGaeJDDt6vtRuJw_JVwsCLaYHGif4UaKCoe6BECg3mRVUkH08Nm-qQPQw9slpYZmxckFEQQPCGkkPhgOBFkE";

        private readonly string user2Token =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJpZFVzZXIyIiwidXNlcm5hbWUiOiJVc2VyMiIsImdhbWVJZCI6InlvdXJHYW1lSWQiLCJwcm9maWxlUGljdHVyZVVybCI6Imh0dHBzOi8vaW1hZ2VzLmNyYXp5Z2FtZXMuY29tL3VzZXJwb3J0YWwvYXZhdGFycy8yLnBuZyIsImlhdCI6MTY2ODU5MzMxNCwiZXhwIjo0ODI0Mjg4NTE0fQ.kh60HYKR8txKvLoCB6dQ9hQG8Mu1UgtneTGs3Y15HvBWrZoLKp3x3pTf_Vhq8xzs7fQYJKr94zYAxxFRztHey7Tv7PBBmPESUFo8Le-_s2xDy982sFhpM6XDt84ohhvEuJEsOW8wIcCaCK6wzm6UWY6n1bpw1cO1KNASyZRSkDRhfyzDVJ5e167OLgGe3euodTHgClJPDv0ZYhle9KH86PepWamCm0429VrzyOu6QdbtFcRlRNZVnTtgNrCpyvss4AyDhnY5qV9yng7xHVt4zlocP_Z7btBL_kxrzYskhJi6KYuQAYmqLxqHSDnehlIvgO4cdEpJA_FOTeACTohhEu8zjXRrfdAFvEe0W6qqUo5HNFoElRoxYWf11WGSdrJCjpF4Qei9BPgprFaVH2Xi-ITAjKyElQxeKs5p6VmvaMAGwdqZgM4fm7OSex8QQGK0HFJ7wFoCTV5PLl-MBVvTSTfemJMWEwc8od124gwT_uGdDKrASovT2pijgBsAi3jxwgfEr1RPq8uCgZtksrTqaAM9TMv9Z6Zv35pdgTrWzTrOn-G-uc4EPZq7iQaEnglWEFj8Qsm_nMQMgtIM7MYG8KwPC4if3-Yc8KwaAL_taVvkqyMaV3W5j4MX9b1bbf_fw3jrGt74MACb7FtgzKudxoz2CXKZqTppadxS_IOnlMk";

        private readonly string expiredToken =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJpZFVzZXIxIiwidXNlcm5hbWUiOiJVc2VyMSIsImdhbWVJZCI6InlvdXJHYW1lSWQiLCJwcm9maWxlUGljdHVyZVVybCI6Imh0dHBzOi8vaW1hZ2VzLmNyYXp5Z2FtZXMuY29tL3VzZXJwb3J0YWwvYXZhdGFycy8xLnBuZyIsImlhdCI6MTY2ODU5MzQ5MSwiZXhwIjoxNjY4NTkzNDkyfQ.l_0cyeD-suEM7n9l-Vb2nP5vTJi-e3HwZQWLUESJMdVTX1zPDuQhwnSgHhcGVGFnhG5Wvtni-ElpM8HnVNvY7hRthbeP23n2ScAJBvAX10vrzPuLJRn_Nj_5GcRQpK4fH813Ij8ZWuOaS2hD4gKaEUessZs5n5hNBTQN9T5j4wkNvfhuw9vqtVOha2aPveqeVy1eA5XAWI7IirHi31-Dw60MSVgsp3r4tpYEHTlUPktzLsQvO9Sk9IE7iybg9ycoFoS6L1eAvxGWVF1BMHRerPwdOV9CN0rtrqrTM3pyb1fpmFfgQpoA8AgPuVrU58mwyeTpUQ4WSrPrltGjxxfiGQOATBDBrJk5V173BfUgBEgAEP0TifWAQt02iijJa9G6q-V8p0GWto1EYSdvEDmG0YhoRBVxnOQH3U1Fu0yxMWGMm9VmZVVhVN8PpLjitEhP4Gn33IafpS05d1-Q0NFMb9-FvQCdtXjTaGbaBVIeBN-aO0r4ERvoBE9R0AUrywd9Z2zK_qKRvp35NyryLjnedsYt5Xrc9TA2uDMR77TjByyqsdQ_qv4zhLfhuiMiweXyPfYzltAiNJmEUohxlP7OvH33B6xpT7Qz2ZyEeMHBrQRQGGlT6MowcMYx_2LFNSK8PwZJNlMs0Uw_uCIu-4TvqleVleIg7sLhWiijw1cxtmM";


        /// <summary>
        /// User account is available only on CrazyGames domains. It is not available on your whitelisted domains from CrazyGamesSettings,
        /// or on any partner websites the embed the game from CrazyGames domains.
        /// </summary>
        /// <param name="callback"></param>
        public void IsUserAccountAvailable(Action<bool> callback)
        {
            if (Application.isEditor)
            {
                var settings = CrazySDK.Instance.GetSettings();
                CrazySDK.Instance.DebugLog("Is user account available simulation for Unity editor, returning: " +
                                           settings.accountAvailableResponse);
                switch (settings.accountAvailableResponse)
                {
                    case CrazySettingsAccountAvailableResponse.Yes:
                        callback(true);
                        return;
                    case CrazySettingsAccountAvailableResponse.No:
                        callback(false);
                        return;
                    default:
                        Debug.LogError("Unhandled case " + settings.accountAvailableResponse +
                                       ", returning 'true'");
                        callback(true);
                        return;
                }
            }

            if (!CrazySDK.IsOnCrazyGames)
            {
                // instantly return, otherwise the callback is put on hold waiting for the SDK init event which never arrives
                callback(false);
            }

            if (CrazySDK.Instance.IsInitialized)
            {
                callback(CrazySDK.Instance.InitObj.userAccountAvailable);
            }
            else
            {
                getUserAccountAvailableCallbacks.Add(callback);
            }
        }

        public void CallInitCallbacks()
        {
            getUserAccountAvailableCallbacks.ForEach(cb => { cb?.Invoke(CrazySDK.Instance.InitObj.userAccountAvailable); });
            getUserAccountAvailableCallbacks.Clear();
        }

        /// <summary>
        /// Retrieves the user that is currently signed in.
        /// The callback receives null if the user is not signed in.<br/><br/>
        /// You can customize the user returned in the editor in the CrazyGamesSettings file.<br/><br/>
        /// This feature is in <b>BETA</b>, please reach out to us if you are interested in using it.
        /// </summary>
        public void GetUser(Action<PortalUser> callback)
        {
            if (Application.isEditor)
            {
                var settings = CrazySDK.Instance.GetSettings();
                CrazySDK.Instance.DebugLog("Get user simulation for Unity editor, returning user: " + settings.getUserResponse);
                switch (settings.getUserResponse)
                {
                    case CrazySettingsTestUser.User1:
                        callback(demoUser1);
                        return;
                    case CrazySettingsTestUser.User2:
                        callback(demoUser2);
                        return;
                    case CrazySettingsTestUser.UserLoggedOut:
                        callback(null);
                        return;
                    default:
                        Debug.LogError("Unhandled user " + settings.getUserResponse + ", returning null user");
                        callback(null);
                        return;
                }
            }

            CrazySDK.Instance.WrapGFFeature(() =>
            {
                // the game may call this method multiple times quickly, but jslib doesn't handle multiple callbacks from the sdk, so store in game a queue of callbacks
                getUserCallbacks.Add(callback);
                if (getUserCallbacks.Count == 1)
                {
                    GetUserSDK();
                }
            });
        }

        /// <summary>
        /// Retrieve the user token for in-game user accounts.
        /// To use this feature, be sure the in-game account feature is enabled for your game.
        /// Callback param1 = error message (null if everything is fine), param2 = token.
        /// Possible error codes: "userNotAuthenticated", "unexpectedError".<br/><br/>
        /// This feature is in <b>BETA</b>, please reach out to us if you are interested in using it.
        /// </summary>
        public void GetUserToken(Action<string, string> callback)
        {
            if (Application.isEditor)
            {
                var settings = CrazySDK.Instance.GetSettings();
                CrazySDK.Instance.DebugLog("Get user token simulation for Unity editor, returning: " + settings.getTokenResponse);
                switch (settings.getTokenResponse)
                {
                    case CrazySettingsGetTokenResponse.User1:
                        callback(null, user1Token);
                        return;
                    case CrazySettingsGetTokenResponse.User2:
                        callback(null, user2Token);
                        return;
                    case CrazySettingsGetTokenResponse.UserLoggedOut:
                        callback("userNotAuthenticated", null);
                        return;
                    case CrazySettingsGetTokenResponse.ExpiredToken:
                        callback(null, expiredToken);
                        return;
                    default:
                        Debug.LogError("Unhandled case " + settings.getTokenResponse +
                                       ", returning 'userNotAuthenticated'");
                        callback("userNotAuthenticated", null);
                        return;
                }
            }

            CrazySDK.Instance.WrapGFFeature(() =>
            {
                // the game may call this method multiple times quickly, but jslib doesn't handle multiple callbacks from the sdk, so store in game a queue of callbacks
                getUserTokenCallbacks.Add(callback);
                if (getUserTokenCallbacks.Count == 1)
                {
                    GetUserTokenSDK();
                }
            });
        }

        /// <summary>
        /// Retrieve Xsolla user token, used to interact with Xsolla SDK.
        /// Callback param1 = error message (null if everything is fine), param2 = token.<br/><br/>
        /// This feature is in <b>BETA</b>, please reach out to us if you are interested in using it.
        /// </summary>
        public void GetXsollaUserToken(Action<string, string> callback)
        {
            if (Application.isEditor)
            {
                CrazySDK.Instance.DebugLog("Get Xsolla user token simulation for Unity editor");
                callback(null, "Demo editor Xsolla token");
                return;
            }

            CrazySDK.Instance.WrapGFFeature(() =>
            {
                // the game may call this method multiple times quickly, but jslib doesn't handle multiple callbacks from the sdk, so store in game a queue of callbacks
                getXsollaUserTokenCallbacks.Add(callback);
                if (getXsollaUserTokenCallbacks.Count == 1)
                {
                    GetXsollaUserTokenSDK();
                }
            });
        }

        /// <summary>
        /// Show auth prompt so the user can sign in.
        /// Callback param1 = error message (null if everything is fine), param2 = user object.
        /// Possible error messages: "showAuthPromptInProgress", "userAlreadySignedIn", "userCancelled".<br/><br/>
        /// You can customize the response returned in the editor in the CrazyGamesSettings files.
        /// </summary>
        /// <param name="callback"></param>
        public void ShowAuthPrompt(Action<string, PortalUser> callback)
        {
            if (Application.isEditor)
            {
                var settings = CrazySDK.Instance.GetSettings();
                CrazySDK.Instance.DebugLog("Show auth prompt simulation for Unity editor, returning: " + settings.authPromptResponse);
                switch (settings.authPromptResponse)
                {
                    case CrazySettingsAuthPromptResponse.User1:
                        callback(null, demoUser1);
                        return;
                    case CrazySettingsAuthPromptResponse.User2:
                        callback(null, demoUser2);
                        return;
                    case CrazySettingsAuthPromptResponse.UserCancelled:
                        callback("userCancelled", null);
                        return;
                    default:
                        Debug.LogError("Unhandled case " + settings.authPromptResponse + ", returning 'userCancelled'");
                        callback("userCancelled", null);
                        return;
                }
            }

            CrazySDK.Instance.WrapGFFeature(() =>
            {
                if (authPromptCallback != null)
                {
                    // this is the only error we need to handle here, since JSlib doesn't handle multiple callbacks from the SDK
                    callback("showAuthPromptInProgress", null);
                }

                authPromptCallback = callback;
                ShowAuthPromptSDK();
            });
        }

        /// <summary>
        /// Shows a prompt asking the user if they want to link their CrazyGames account to current in-game account.
        /// Callback param1 = error message (null if everything is fine), param2 = link response.
        /// Possible error messages: "showAccountLinkPromptInProgress", "userNotAuthenticated"
        /// </summary>
        public void ShowAccountLinkPrompt(Action<string, bool> callback)
        {
            if (Application.isEditor)
            {
                var settings = CrazySDK.Instance.GetSettings();
                CrazySDK.Instance.DebugLog("Show auth prompt simulation for Unity editor, returning: " + settings.linkAccountResponse);
                switch (settings.linkAccountResponse)
                {
                    case CrazySettingsLinkAccountResponse.Yes:
                        callback(null, true);
                        return;
                    case CrazySettingsLinkAccountResponse.No:
                        callback(null, false);
                        return;
                    case CrazySettingsLinkAccountResponse.UserLoggedOut:
                        callback("userNotAuthenticated", false);
                        return;
                    default:
                        Debug.LogError("Unhandled case " + settings.linkAccountResponse +
                                       ", returning 'userNotAuthenticated'");
                        callback("userNotAuthenticated", false);
                        return;
                }
            }

            CrazySDK.Instance.WrapGFFeature(() =>
            {
                if (accountLinkPromptCallback != null)
                {
                    // this is the only error we need to handle here, since JSlib doesn't handle multiple callbacks from the SDK
                    callback("showAccountLinkPromptInProgress", false);
                }

                accountLinkPromptCallback = callback;
                ShowAccountLinkPromptSDK();
            });
        }

        /// <summary>
        /// Register a new listener that is called everytime the user signs in on the platform. When the user signs out, the page is refreshed, so for sign out there is no auth listener call.
        /// </summary>
        public void AddAuthListener(Action<PortalUser> listener)
        {
            authListeners.Add(listener);
        }

        /// <summary>
        /// Requests to save the user's PlayerPref file to Automatic progress save (APS).
        /// </summary>
        public void SyncUnityGameData()
        {
            if (Application.isEditor)
            {
                CrazySDK.Instance.DebugLog("Requested to sync UnityGameData");
                return;
            }

            CrazySDK.Instance.WrapGFFeature(() =>
            {
                SyncUnityGameDataSDK();
            });
        }

        public void RemoveAuthListener(Action<PortalUser> listener)
        {
            authListeners.Remove(listener);
        }

        public void AddScore(float score)
        {
            if (Application.isEditor)
            {
                CrazySDK.Instance.DebugLog("Submit score: " + score);
            }
            else
            {
                AddUserScoreSDK(score);
            }
        }

        private void JSLibCallback_GetUser(string responseStr)
        {
            var response = JsonUtility.FromJson<UserCallbackReply>(responseStr);
            var error = response.error;
            if (!string.IsNullOrEmpty(error))
            {
                // an error normally doesn't occur when requesting the user
                Debug.LogError("Get user error: " + error);
            }

            var tempList =
                getUserCallbacks.Select(c => c)
                    .ToList(); // use a temp list, the main list may get modified if a callback will call get user again
            getUserCallbacks.Clear();
            tempList.ForEach(c => c(response.User));
        }

        private void JSLibCallback_GetUserToken(string responseStr)
        {
            var response = JsonUtility.FromJson<TokenCallbackReply>(responseStr);

            var tempList =
                getUserTokenCallbacks.Select(c => c)
                    .ToList(); // use a temp list, the main list may get modified if a callback will call get user token again
            getUserTokenCallbacks.Clear();
            tempList.ForEach(c => c(response.error, response.token));
        }

        private void JSLibCallback_GetXsollaUserToken(string responseStr)
        {
            var response = JsonUtility.FromJson<TokenCallbackReply>(responseStr);

            var tempList =
                getXsollaUserTokenCallbacks.Select(c => c)
                    .ToList(); // use a temp list, the main list may get modified if a callback will call get user token again
            getXsollaUserTokenCallbacks.Clear();
            tempList.ForEach(c => c(response.error, response.token));
        }

        private void JSLibCallback_ShowAccountLinkPrompt(string responseStr)
        {
            var response = JsonUtility.FromJson<LinkAccountResponseCallbackReply>(responseStr);
            var linkAccountResponse = response.Response;
            accountLinkPromptCallback(response.error,
                linkAccountResponse != null && linkAccountResponse.response == "yes");
            accountLinkPromptCallback = null;
        }

        private void JSLibCallback_ShowAuthPrompt(string responseStr)
        {
            var response = JsonUtility.FromJson<UserCallbackReply>(responseStr);
            authPromptCallback(response.error, response.User);
            authPromptCallback = null;
        }

        private void JSLibCallback_AuthListener(string responseStr)
        {
            var response = JsonUtility.FromJson<UserCallbackReply>(responseStr);

            var tempList =
                authListeners.Select(c => c)
                    .ToList(); // use a temp list, the main list may get modified if a callback adds/removes an auth listener
            tempList.ForEach(c => c(response.User));
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string ShowAuthPromptSDK();

        [DllImport("__Internal")]
        private static extern string ShowAccountLinkPromptSDK();

        [DllImport("__Internal")]
        private static extern string GetUserSDK();

        [DllImport("__Internal")]
        private static extern string GetUserTokenSDK();

        [DllImport("__Internal")]
        private static extern string GetXsollaUserTokenSDK();

        [DllImport("__Internal")]
        private static extern void AddUserScoreSDK(float score);

        [DllImport("__Internal")]
        private static extern void SyncUnityGameDataSDK();

#else
        // Preventing build to fail when using another platform than WebGL
        private void ShowAuthPromptSDK()
        {
        }

        private void ShowAccountLinkPromptSDK()
        {
        }

        private void GetUserSDK()
        {
        }

        private void GetUserTokenSDK()
        {
        }

        private void GetXsollaUserTokenSDK()
        {
        }

        private void AddUserScoreSDK(float score)
        {
        }

        private void SyncUnityGameDataSDK()
        {
        }
#endif


        [Serializable]
        private class TokenCallbackReply
        {
            public string error;
            public string token;
        }

        [Serializable]
        private class UserCallbackReply
        {
            public string error;
            public string userStr;

            // JsonUtility creates a class with empty fields if value is null, so send value as string and manually convert it to class if needed
            public PortalUser User =>
                (userStr == null || userStr == "null") ? null : JsonUtility.FromJson<PortalUser>(userStr);
        }

        [Serializable]
        private class LinkAccountResponse
        {
            public string response;
        }

        [Serializable]
        private class LinkAccountResponseCallbackReply
        {
            public string error;
            public string linkAccountResponseStr;

            // JsonUtility creates a class with empty fields if value is null, so send value as string and manually convert it to class if needed
            public LinkAccountResponse Response => (linkAccountResponseStr == null || linkAccountResponseStr == "null")
                ? null
                : JsonUtility.FromJson<LinkAccountResponse>(linkAccountResponseStr);
        }
    }

    [Serializable]
    public class PortalUser
    {
        public string username;
        public string profilePictureUrl;

        public override string ToString()
        {
            return base.ToString() + "Username = " + username + ", profile picture url = " +
                   profilePictureUrl;
        }
    }
}