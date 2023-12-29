using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrazyGames
{
    /**
     * Site locking is checked on game start.
     */
    public class SiteLock
    {
        public static string sitelockVersion = "1.8.0";

        public static readonly string[] allowedLocalHosts = { "localhost" };

        // should match the order of the allowedLocalHosts
        public static readonly string[] allowedLocalHostsSignatures =
        {
            "BvN3uwS7R9GgXjcS9FmqFyrNmXP4+LIM+ZVAH7MHzel9KqxBySCplKiO3xCldMsZoL0/Cspx9g31mOOvobqZ27t0neApNC1/SGnUKDi/JcJcUm6/U++DhOA1tZs06v/TmPC2OGj9LJrk06CikKi/GYX3De67PKagAJrYJBcDm6M="
        };


        public static readonly string[] allowedRemoteHosts =
        {
            "1001juegos.com",
            "speelspelletjes.nl",
            "onlinegame.co.id",
            "developer.unity3dusercontent.com" // unity cloud build URL
        };

        // should match the order of the allowedRemoteHosts
        public static readonly string[] allowedRemoteHostsSignatures =
        {
            "KNaJj7P0w2GXcFpK/j9W8KTh/Rk2kb6ouLrwAUV0JDjmn5DEWewXRy+pxY8FAbv+g14tdgd8g6mj1YniA+Vtqiw0NMCn60EetTB14u86ILG08QEtE3DTLolpCmg2O05sVfLcmBIxxFQRD1RGy+CAEje4MYfRZnGaaM21KdprEQ8=",
            "LKNZOZMyMxNrifXr1pT6Qdn2QJhMjYSxFjKbf/q6L1sGi7f+I+TZHbr/vu8ZyL4rholEg9ioxinvcPuyGR8v1v8h7ATTAbTRgpmPBx4weToCIABwheTTf8opg2CFzurO6Q/JqBPN7ChmqYBc76RmDhZme9Q3Lih9Kkt+gT7S6io=",
            "SVb5hWSjBuNFXoLCM8xgt07jIB/mB9k2qcPvzGK6h7lkDziVcYulsVtrHQez4jh0mIMEFC4Ck2wYL+cukUpOeJpHtHxrnlNq79HCjeMfJWn8NHsLKebm4HlVPjPkySz7zV57PhS+47TnaB3a8RWXtFkUjELNHsNqf7ut8Pljhe8=",
            "iypCZwzt1w14ZKbRmJ0ALBWqc88xGc7fqT4SylhvANhiEdtHCJs72OaUP/Q/ZvjKFQNfdaPLrq985i8+1sRBjUYyM5SVu9GEpqcCFDzjpj3oMjpGTcXCrZxHykTHEQhjpf9sWWYu3t42+I7rF5S+o94TdKrnqR3x7QSl+kq0r9A="
        };

        /// Do we permit execution from local host or local file system?
        private static readonly bool allowLocalHost = true;

        private static string publicKey =
            "<RSAKeyValue><Modulus>pywrgOEyhw7YLQNGjvKGZIWsCf3HyUU9i5mn7OHjxSdOty1io9mJIul5kDQSrQFLDyucgafx/7h8t/q0hKuH5mjMFqO1SSvhC5IDjN4Uyl0LZ2bBDzFqLu8vzjVg5DVOCCoEr4s2y3i9SunSCfyRQi0Ga3EII/1lyZnZbYydC5c=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static bool DidRun { get; private set; }
        public static bool IsSiteLocked { get; private set; }

        public static void Check()
        {
            DidRun = true;

#if !UNITY_WEBGL
            return;
#endif

            if (IsOnWhitelistedDomain())
            {
                // if the game is running on dev's domain, don't proceed further with the sitelock
                return;
            }

            DebugLog("[CrazySDK] SiteLock v" + sitelockVersion);

            var url = Application.absoluteURL;
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                //String is not a valid URL. 
#if UNITY_EDITOR
                //print("URL Not Valid");
                return;
#else
                Crash(1);
                return;
#endif
            }

            // check if the allowed domains have been tampered 
            var domains = new List<string>();
            domains.AddRange(allowedLocalHosts);
            domains.AddRange(allowedRemoteHosts);
            var signatures = new List<string>();
            signatures.AddRange(allowedLocalHostsSignatures);
            signatures.AddRange(allowedRemoteHostsSignatures);
            for (var index = 0; index < domains.Count; index++)
            {
                if (!Encryption.Verify(domains[index], signatures[index], publicKey))
                {
                    Crash(2);
                }
            }

            var host = uri.Host;

            var splittedHost = host.Split("."[0]);
            var crazyIndex = -1;

            for (var i = 0; i < splittedHost.Length; i++)
            {
                var split = splittedHost[i].ToLower();
                if (split != "crazygames" && split != "dev-crazygames") continue;
                crazyIndex = i;
                break;
            }


            if (crazyIndex >= 0 &&
                splittedHost.Length == crazyIndex + 2 ||
                splittedHost.Length == crazyIndex + 3 && splittedHost[crazyIndex + 1].Length <= 3
               )
            {
#if UNITY_EDITOR
                Debug.Log("SUCCESS!  We Are On CrazyGames server!  ");
#endif
                return; //no more logic needed 
            }

#if UNITY_EDITOR
            Debug.Log("FAILED!  We Are Not On CrazyGames Server!");
#endif
            //so, continue with checking other allowed domains ...


#if !UNITY_EDITOR
            if (!IsOnValidHost())
            {
                Crash(3);
                return;
            }
#endif
        }

        private static bool IsOnValidHost()
        {
            return IsOnValidLocalHost() || IsOnValidRemoteHost();
        }

        /// Determine if the current host exists in the given list of permitted hosts.
        private static bool IsValidHost(string[] hosts)
        {
            if (Debug.isDebugBuild)
            {
                var msg = new StringBuilder();
                msg.Append("Checking against list of hosts: ");
                foreach (var url in hosts)
                {
                    msg.Append(url);
                    msg.Append(",");
                }

                DebugLog(msg.ToString());
            }

            // check current host against each of the given hosts
            var hostRegex = new Regex(@"^(\w+)://(?<hostname>[^/]+?)(?<port>:\d+)?/");
            var match = hostRegex.Match(Application.absoluteURL);
            if (!match.Success)
                // somehow our current url is not a valid url
                return false;

            var hostname = match.Groups["hostname"].Value;
            var splittedHost = hostname.Split("."[0]);
            return hosts.Any(host => DoesHostMatch(host, splittedHost));
        }

        private static bool DoesHostMatch(string allowedHost, string[] applicationHost)
        {
            var splitAllowed = allowedHost.Split("."[0]);

            if (applicationHost.Length < splitAllowed.Length) return false;

            for (var i = 0; i < splitAllowed.Length; i++)
            {
                var currentSplit = splitAllowed[i];
                var currentHost = applicationHost[applicationHost.Length - splitAllowed.Length + i];
                if (!currentSplit.Equals(currentHost)) return false;
            }

            return true;
        }

        /// Determine if the current host is a valid local host.
        private static bool IsOnValidLocalHost()
        {
            return allowLocalHost && IsValidHost(allowedLocalHosts);
        }

        /// <summary>
        ///     Determine if the current host is a valid remote host.
        /// </summary>
        /// <returns>True if the game is permitted to execute from the remote host.</returns>
        private static bool IsOnValidRemoteHost()
        {
            return IsValidHost(allowedRemoteHosts);
        }

        private static void Crash(int code)
        {
            IsSiteLocked = true;
            CrazySDK.Instance.SiteLockFreezeGame(code);
        }

        public static bool IsOnWhitelistedDomain()
        {
            var settings = GetSettings();
            var whitelistedDevDomains = new List<string>();
            if (settings == null || settings.whitelistedDomains == null) return false;

            foreach (var settingsDomain in settings.whitelistedDomains)
            {
                var domain = settingsDomain;

                if (!domain.StartsWith("http://") && !domain.StartsWith("https://"))
                {
                    domain = "http://" + domain;
                }

                try
                {
                    Uri uri = new Uri(domain);
                    whitelistedDevDomains.Add(uri.Host);
                }
                catch (Exception)
                {
                    Debug.LogError("[CrazySDK] Failed to parse whitelisted domain: " + settingsDomain);
                }
            }

            return IsValidHost(whitelistedDevDomains.ToArray());
        }

        private static void DebugLog(string message)
        {
            if (Application.isEditor && GetSettings().disableSdkLogs)
            {
                return;
            }

            Debug.Log(message);
        }

        private static CrazySettings _crazySettings;
        private static bool _settingsLoaded;

        private static CrazySettings GetSettings()
        {
            if (_settingsLoaded)
            {
                return _crazySettings;
            }

            _settingsLoaded = true;
            _crazySettings = Resources.Load<CrazySettings>("CrazyGamesSettings");
            if (_crazySettings == null)
            {
                Debug.LogError(
                    "Missing CrazySDK/Resources/CrazyGamesSettings file. Please be sure you imported the SDK correctly.");
                return null;
            }

            return _crazySettings;
        }
    }
}