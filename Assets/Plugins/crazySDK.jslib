/**
 * This loads the HTML SDK v2 and will act as a wrapper arround it.
 */
mergeInto(LibraryManager.library, {
  InitSDK: function (version, objectName) {
    if (!window.Crazygames) {
      return false;
    }

    // to avoid warnings about Unity stringify beeing obsolete
    if (typeof UTF8ToString !== 'undefined') {
      window.unityStringify = UTF8ToString;
    } else {
      window.unityStringify = Pointer_stringify;
    }

    window.UnitySDK = {
      version: window.unityStringify(version),
      objectName: window.unityStringify(objectName),
      userObjectName: 'CrazyGames.CrazyUser',
      isSdkLoaded: false,
      waitingForLoad: [],
      pointerLockElement: undefined,
      onSdkScriptLoaded: function () {
        this.isSdkLoaded = true;
        this.waitingForLoad.forEach(function (callback) {
          callback();
        });
        this.waitingForLoad = [];
      },
      ensureLoaded: function (callback) {
        if (this.isSdkLoaded) {
          callback();
        } else {
          this.waitingForLoad.push(callback);
        }
      },
      unlockPointer: function () {
        this.pointerLockElement = document.pointerLockElement || null;
        if (this.pointerLockElement && document.exitPointerLock) {
          document.exitPointerLock();
        }
      },
      lockPointer: function () {
        if (this.pointerLockElement && this.pointerLockElement.requestPointerLock) {
          this.pointerLockElement.requestPointerLock();
        }
      },
      // Unity doesn't support Dictionary serialization to JSON,
      // so the invite params will be extracted here from the invite link
      // generated in Unity
      urlParamsToJson: function (url) {
        var queryParams = url.split('?')[1];
        if (!queryParams) {
          return {};
        }
        var paramsArray = queryParams.split('&');
        var paramsObject = {};
        for (var i = 0; i < paramsArray.length; i++) {
          var param = paramsArray[i];
          var keyValue = param.split('=');
          paramsObject[keyValue[0]] = decodeURIComponent(keyValue[1]);
        }
        return paramsObject;
      }
    };

    if (window.crazySdkInitOptions) {
      window.crazySdkInitOptions.wrapper = {
        engine: 'unity',
        sdkVersion: window.unityStringify(version)
      };
    } else {
      window.crazySdkInitOptions = {
        wrapper: {
          engine: 'unity',
          sdkVersion: window.unityStringify(version)
        }
      };
    }

    // load the HTML SDK v2
    var script = document.createElement('script');
    script.src = 'https://sdk.crazygames.com/crazygames-sdk-v2.js';
    document.head.appendChild(script);
    script.addEventListener('load', function () {
      window.UnitySDK.onSdkScriptLoaded();
    });

    // send the init reply to Unity after the JS sdk is loaded & initialized
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.addInitCallback(function (initObject) {
        SendMessage(window.UnitySDK.objectName, 'InitCallback', JSON.stringify(initObject));
      });
    });

    // check adblock status after the js sdk is loaded
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.ad.hasAdblock(function (error, result) {
        if (error) {
          console.error('Adblock usage error (callback)', error);
        } else {
          SendMessage(window.UnitySDK.objectName, result ? 'AdblockDetected' : 'AdblockNotDetected');
        }
      });
    });

    // register the auth listener. Only one auth listener is registered in the SDK itself. It will call the other auth listeners registered in game.
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.user.addAuthListener(function (user) {
        SendMessage(window.UnitySDK.userObjectName, 'JSLibCallback_AuthListener', JSON.stringify({ userStr: JSON.stringify(user) }));
      });
    });

    return true;
  },

  /** SDK.ad module */
  RequestAdSDK: function (adType) {
    var adTypeStr = window.unityStringify(adType);
    var callbacks = {
      adStarted: function () {
        window.UnitySDK.unlockPointer();
        SendMessage(window.UnitySDK.objectName, 'AdEvent', JSON.stringify({ name: 'adStarted' }));
      },
      adFinished: function () {
        window.UnitySDK.lockPointer();
        SendMessage(window.UnitySDK.objectName, 'AdEvent', JSON.stringify({ name: 'adFinished' }));
      },
      adError: function (error, errorData) {
        const message = errorData ? errorData.message : error;
        const reason = errorData ? errorData.reason : undefined;
        SendMessage(window.UnitySDK.objectName, 'AdEvent', JSON.stringify({ name: 'adError', message: message, reason: reason }));
      }
    };
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.ad.requestAd(adTypeStr, callbacks);
    });
  },

  /** SDK.banner module */
  RequestBannersSDK: function (bannersJSON) {
    var banners = JSON.parse(window.unityStringify(bannersJSON));
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.banner.requestOverlayBanners(banners, function (bannerId, message, error) {
        // the callback is called for every banner in array
        switch (message) {
          case 'bannerRendered':
            SendMessage(window.UnitySDK.objectName, 'AdEvent', JSON.stringify({ name: 'inGameBannerRendered', id: bannerId }));
            break;
          case 'bannerError':
            SendMessage(window.UnitySDK.objectName, 'AdEvent', JSON.stringify({ name: 'inGameBannerError', id: bannerId, error: error }));
            break;
          default:
            console.error('Unknown banner message in Jslib:', message);
        }
      });
    });
  },

  /** SDK.game module */
  HappyTimeSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.game.happytime();
    });
  },
  GameplayStartSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.game.gameplayStart();
    });
  },
  GameplayStopSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.game.gameplayStop();
    });
  },
  RequestInviteUrlSDK: function (url) {
    var urlParams = window.UnitySDK.urlParamsToJson(window.unityStringify(url));
    // the URL is already generated in Unity, call this method to get the logs/events
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.game.inviteLink(urlParams);
    });
  },
  ShowInviteButtonSDK: function (url) {
    var urlParams = window.UnitySDK.urlParamsToJson(window.unityStringify(url));
    // the URL is already generated in Unity, call this method to get the logs/events
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.game.showInviteButton(urlParams);
    });
  },
  HideInviteButtonSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.game.hideInviteButton();
    });
  },

  /** SDK.user module */
  ShowAuthPromptSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.user.showAuthPrompt(function (error, user) {
        SendMessage(window.UnitySDK.userObjectName, 'JSLibCallback_ShowAuthPrompt', JSON.stringify({ error: error, userStr: JSON.stringify(user) }));
      });
    });
  },
  ShowAccountLinkPromptSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.user.showAccountLinkPrompt(function (error, response) {
        // response format { response: 'yes' | 'no'}
        SendMessage(window.UnitySDK.userObjectName, 'JSLibCallback_ShowAccountLinkPrompt', JSON.stringify({ error: error, linkAccountResponseStr: JSON.stringify(response) }));
      });
    });
  },
  GetUserSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.user.getUser(function (error, user) {
        SendMessage(window.UnitySDK.userObjectName, 'JSLibCallback_GetUser', JSON.stringify({ error: error, userStr: JSON.stringify(user) }));
      });
    });
  },
  GetUserTokenSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.user.getUserToken(function (error, token) {
        SendMessage(window.UnitySDK.userObjectName, 'JSLibCallback_GetUserToken', JSON.stringify({ error: error, token: token }));
      });
    });
  },
  GetXsollaUserTokenSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      window.CrazyGames.SDK.user.getXsollaUserToken(function (error, token) {
        SendMessage(window.UnitySDK.userObjectName, 'JSLibCallback_GetXsollaUserToken', JSON.stringify({ error: error, token: token }));
      });
    });
  },
  AddUserScoreSDK: function (score) {
    window.UnitySDK.ensureLoaded(function () {
      var scoreToAdd = score;
      window.CrazyGames.SDK.user.addScore(scoreToAdd);
    });
  },

  /** other */
  CopyToClipboardSDK: function (text) {
    const elem = document.createElement('textarea');
    elem.value = window.unityStringify(text);
    document.body.appendChild(elem);
    elem.select();
    elem.setSelectionRange(0, 99999);
    document.execCommand('copy');
    document.body.removeChild(elem);
  },
  GetUrlParametersSDK: function () {
    const urlParameters = window.location.search;
    var bufferSize = lengthBytesUTF8(urlParameters) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(urlParameters, buffer, bufferSize);
    return buffer;
  },
  SyncUnityGameDataSDK: function () {
    window.UnitySDK.ensureLoaded(function () {
      // html-v3 syncUnityGameData function is available on the data module
      window.CrazyGames.SDK.syncUnityGameData();
    });
  }
});
