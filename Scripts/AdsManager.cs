using GoogleMobileAds.Api;
using haiykut;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Common;

public class AdsManager : MonoBehaviour
{
    [SerializeField] Platform platform;
    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;
    AppOpenAd appOpenAd;
    AdSize bannerSize;
    [SerializeField] string nextSceneName;
    public static AdsManager instance;
    public CanvasGroup fadePanel;
    internal AdSize banner = AdSize.Banner;
    AdPosition adPosition = AdPosition.TopRight;
    DateTime expireTime;
    [SerializeField] GameObject logo;
    internal bool freeCar;
    int amount = 25;
    [SerializeField]internal bool isPausedGame;
    private void Awake()
    {
        if(instance != null)
            Destroy(instance);

        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(fadePanel.gameObject);
        DontDestroyOnLoad(logo.gameObject);
        PlayerPrefs.SetInt("playercar" + 2, 0);
        PlayerPrefs.SetInt("playercar" + 8, 0);
        //PlayerPrefs.DeleteAll();
    }
    void Start()
    {
        amount = 50;
        bannerSize = new AdSize((int)platform.bannerSize.x, (int)platform.bannerSize.y);
        MobileAds.Initialize(initStatus => { });
        NextScene();
        LoadLoadInterstitialAd();
        LoadRewardedAd();
    }
  
    #region AdMob Methods
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(platform.bannerId, banner, adPosition);
    }
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
    }
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    public void DestroyBannerView()
    {
        if (bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            bannerView.Destroy();
            bannerView = null;
        }
    }
    public void LoadLoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(platform.interstitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
                RegisterReloadHandler(interstitialAd);
                RegisterEventHandlers(interstitialAd);
            });
    }
    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            InterAdFunc(SceneManager.GetActiveScene().name);
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            InterAdFunc(SceneManager.GetActiveScene().name);
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadLoadInterstitialAd();
            InterAdFunc(SceneManager.GetActiveScene().name);
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadLoadInterstitialAd();
        };
    }
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(platform.rewardedId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
    }
    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                RewardPlayer();

            });
        }
    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
    {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

    #endregion

    #region My Methods
    public void RewardPlayer()
    {
        if (freeCar)
        {
            if (SceneManager.GetActiveScene().name == "Garage")
            {
                GarageManager gm = FindObjectOfType<GarageManager>();
                PlayerPrefs.SetInt("playercar" + gm.pointer, 1);
                gm.modeText.GetComponentInChildren<Text>().text = gm.modeMainText;
                gm.modeText.GetComponentInChildren<Text>().fontSize = 40;
                gm.modeText.GetComponentInChildren<Text>().color = Color.white;
                freeCar = false;
                if (SettingsManager.instance.sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }
            }
        }
        else
        {
            int diamond = PlayerPrefs.GetInt("diamond");
            diamond += amount;
            PlayerPrefs.SetInt("diamond", diamond);
            if (SceneManager.GetActiveScene().name == "Garage")
            {
                GarageManager gm = FindObjectOfType<GarageManager>();
                gm.diamondText.text = diamond.ToString();
                if (SettingsManager.instance.sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }
            }
            if (SceneManager.GetActiveScene().name == "scene_night")
            {
                GameManager.instance.pauseDiamondText.text = diamond.ToString();
                GameManager.instance.loseDiamondText.text = diamond.ToString();
                GameManager.instance.winDiamondText.text = diamond.ToString();
                StartCoroutine(PauseGame());
                IEnumerator PauseGame()
                {
                    yield return new WaitForSeconds(.01f);
                    if (isPausedGame)
                    {
                        Time.timeScale = 0;
                    }
                }
                if (SettingsManagerNew.Instance.sounds == 0)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }

            }
        }
        LoadRewardedAd();
    }
    void tween(float val, object obj)
    {
        if(val > 0.9f)
        {
            fadePanel.transform.GetChild(1).gameObject.SetActive(false);

        }
      
    }
    public void NextScene()
    {

        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 0, 1.5f);
        LeanTween.alphaCanvas(fadePanel, 0, 1.5f).setOnUpdate(tween).setOnUpdateParam(fadePanel.alpha).setOnComplete(() =>
        {
            Destroy(fadePanel.gameObject);
            Destroy(fadePanel);
            fadePanel = null;
            Destroy(logo.gameObject);
            Destroy(logo);
            logo = null;
        });
        //LeanTween.alphaText(fadePanel.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>(), 0, 1.5f);
        SceneManager.LoadScene(nextSceneName);
    }

    public void InterAdFunc(string sceneName)
    {
        if(sceneName == "Garage")
        {
            StartCoroutine(PauseGame());
            IEnumerator PauseGame()
            {
                yield return new WaitForSeconds(.01f);
                if (isPausedGame)
                {
                    Time.timeScale = 0;
                }
            }
          

        }
        else if(sceneName == "scene_night")
        {
            StartCoroutine(Guide());
            IEnumerator Guide()
            {
                yield return new WaitForSeconds(.1f);
                //Time.timeScale = 0.1f;
                StartCoroutine(GameManager.instance.Guide());
            }
            StartCoroutine(PauseGame());
            IEnumerator PauseGame()
            {
                yield return new WaitForSeconds(.01f);
                if (isPausedGame)
                {
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void BannerSetter(string sceneName)
    {
        if(sceneName == "Garage")
        {
            DestroyBannerView();
            banner = AdSize.Banner;
            adPosition = AdPosition.BottomLeft;
            LoadAd();
        }
        else if(sceneName == "scene_night")
        {
            DestroyBannerView();
            banner = AdSize.Banner;
            adPosition = AdPosition.BottomRight;
            LoadAd();
        }
    }
    #endregion
}
