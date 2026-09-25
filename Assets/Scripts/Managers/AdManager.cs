using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System.Threading.Tasks;
using CardGameArchive;
using System;

public class AdManager : MonoBehaviour
{
	public static AdManager Instance { get; private set; }

	readonly TaskCompletionSource<bool> startupTCS = new();
	Task<bool> startupTask => startupTCS.Task;

#if UNITY_EDITOR
	[SerializeField] bool bypassAds = false; 
#endif
	bool skipIntersitialAds = false;

	public bool AdPlaying { get; private set; } = false;

	RewardedAd rewardedAd;
	public bool RewardAdReady => rewardedAd != null && rewardedAd.CanShowAd();
	bool loadingRewardAd = false;

	InterstitialAd interstitialAd;
	public bool InterstitialAdReady => interstitialAd != null && interstitialAd.CanShowAd();
	bool loadingInterstitialAd = false;
	[SerializeField] float interstitalAdCooldownTime = 60f;
	float lastInterstitialAdTime = 0;
	public bool InterstitialAdCooldownComplete => Time.time >= lastInterstitialAdTime + interstitalAdCooldownTime;

#if UNITY_ANDROID
	string rewardAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Official Test ID
	string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Official Test ID
#else
	string rewardAdUnitId = "unused";
	string interstitialAdUnitId = "unused";
#endif

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	void Start()
	{
		GameTaskManager.Instance.AddTask(startupTask);

		// Set request parameters
		var requestParameters = new ConsentRequestParameters
		{
			TagForUnderAgeOfConsent = false
		};

		// 1. Update consent info from Google's UMP servers
		ConsentInformation.Update(requestParameters, (FormError updateError) =>
		{
			if (updateError != null)
			{
				Debug.LogError($"Consent update failed: {updateError.Message}");
				InitializeMobileAds();
				return;
			}

			// 2. Present consent form if required by region (EEA/UK)
			ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
			{
				if (formError != null)
				{
					Debug.LogError($"Consent form error: {formError.Message}");
				}

				// 3. Initialize Mobile Ads if consent obtained or not required
				if (ConsentInformation.CanRequestAds())
				{
					InitializeMobileAds();
				}
				else
				{
					startupTCS.TrySetResult(false);
				}
			});
		});
	}

	void InitializeMobileAds()
	{
		MobileAds.Initialize(initStatus =>
		{
			Debug.Log("Google Mobile Ads SDK Initialized safely.");
			PreLoadRewardedAd();
			PreLoadInterstitialAd();
			startupTCS.TrySetResult(true);
		});
	}

	#region Rewarded Ad
	void PreLoadRewardedAd()
	{
		if (loadingRewardAd)
			return;

		loadingRewardAd = true;

		if (rewardedAd != null)
		{
			rewardedAd.Destroy();
			rewardedAd = null;
		}

		var adRequest = new AdRequest();
		RewardedAd.Load(rewardAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
		{
			loadingRewardAd = false;
			if (error != null || ad == null)
			{
				Debug.LogError("Rewarded ad failed to load: " + error);
				return;
			}

			rewardedAd = ad;
			RegisterAdEvents(rewardedAd);
		});
	}

	void RegisterAdEvents(RewardedAd ad)
	{
		ad.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Rewarded ad closed, loading next ad.");
			PreLoadRewardedAd();
			OnAdEnd();
		};

		ad.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Rewarded ad failed to show: " + error);
			PreLoadRewardedAd();
			OnAdEnd();
		};
	}

	public void ShowRewardedAd(Action rewardAction)
	{
#if UNITY_EDITOR
		if (bypassAds)
		{
			Debug.Log("Ad Bypassed in Editor. Reward granted immediately.");
			rewardAction.Invoke();
			return;
		}
#endif

		if (!RewardAdReady)
		{
			PreLoadRewardedAd();
		}
		else
		{
			OnAdStart();
			rewardedAd.Show((Reward reward) =>
			{
				rewardAction.Invoke();
			});
		}
	}
	#endregion

	#region Interstitial Ad
	void PreLoadInterstitialAd()
	{
		if (loadingInterstitialAd)
			return;

		loadingInterstitialAd = true;

		if (interstitialAd != null)
		{
			interstitialAd.Destroy();
			interstitialAd = null;
		}

		var adRequest = new AdRequest();
		InterstitialAd.Load(interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
		{
			loadingInterstitialAd = false;
			if (error != null || ad == null)
			{
				Debug.LogError("Interstitial ad failed to load: " + error);
				return;
			}

			interstitialAd = ad;
			RegisterAdEvents(interstitialAd);
		});
	}

	void RegisterAdEvents(InterstitialAd ad)
	{
		ad.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Interstitial ad closed, loading next ad.");
			PreLoadInterstitialAd();
			OnAdEnd();
		};

		ad.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Interstitial ad failed to show: " + error);
			PreLoadInterstitialAd();
			OnAdEnd();
		};
	}

	public void ShowInterstitialAd()
	{
#if UNITY_EDITOR
		if (bypassAds)
		{
			return;
		}
#endif

		if (skipIntersitialAds)
			return;

		if (!InterstitialAdReady)
		{
			PreLoadInterstitialAd();
		}
		else if (InterstitialAdCooldownComplete)
		{
			lastInterstitialAdTime = Time.time;
			OnAdStart();
			interstitialAd.Show();
		}
	}
#endregion

	void OnAdStart()
	{
		AudioManager.Instance.Mute();
		Time.timeScale = 0;
		AdPlaying = true;
	}

	void OnAdEnd()
	{
		AudioManager.Instance.Unmute();
		Time.timeScale = 1;
		AdPlaying = false;
	}

	public void OpenPrivacyOptions()
	{
		ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
		{
			if (showError != null)
			{
				Debug.LogError($"Error displaying privacy options: {showError.Message}");
			}
		});
	}
}