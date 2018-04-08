
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obslolete
#pragma warning disable 0108 
#pragma warning disable 0649 //never used

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AppAdvisory.Ads
{
	public class AdsInit : MonoBehaviour 
	{
		public ADIDS _ADIDS;

		public void SetADIDS(ADIDS t)
		{
			this._ADIDS = t;
		}


		/*
		private List<InterstitialNetwork> InitInterstitialNetworks()
		{
			List<InterstitialNetwork> interstitialNetworks = new List<InterstitialNetwork> ();

			#if ENABLE_ADMOB
			if(_ADIDS.useAdmobAsInterstitialNetwork)
			{
				interstitialNetworks.Add(InterstitialNetwork.Admob);
			}
			#endif

			#if CHARTBOOST
			if(_ADIDS.useChartboostAsInterstitialNetwork) 
			{
				interstitialNetworks.Add(InterstitialNetwork.Chartboost);
			}
			#endif

			#if ENABLE_FACEBOOK
			if(_ADIDS.useFacebookAsInterstitialNetwork) 
			{
				interstitialNetworks.Add(InterstitialNetwork.Facebook);
			}
			#endif

			return interstitialNetworks;
		}

		private List<VideoNetwork> InitVideoNetworks()
		{
			List<VideoNetwork> videoNetworks = new List<VideoNetwork> ();

			#if ENABLE_UNITY_ADS
			//#if UNITY_ADS
			if(_ADIDS.useUnityAdsAsBannerNetwork)
			{
				videoNetworks.Add(VideoNetwork.UnityAds);
			}
			#endif

			#if ENABLE_ADCOLONY
			if(_ADIDS.useAdColonyAsBannerNetwork)
			{
				videoNetworks.Add(VideoNetwork.ADColony);
			}
			#endif
			return videoNetworks;

		}

		private List<RewardedVideoNetwork> InitRewardedVideoNetworks()
		{
			List<RewardedVideoNetwork> rewardedVideoNetworks = new List<RewardedVideoNetwork> ();

			#if ENABLE_ADMOB
			if(_ADIDS.useAdmobAsRewardedVideoNetwork)
			{
				rewardedVideoNetworks.Add(RewardedVideoNetwork.Admob);
			}
			#endif

			#if ENABLE_UNITY_ADS
			//#if UNITY_ADS
			if(_ADIDS.useUnityAdsAsRewardedVideoNetwork)
			{
				rewardedVideoNetworks.Add(RewardedVideoNetwork.UnityAds);
			}
			#endif

			#if CHARTBOOST
			if(_ADIDS.useChartboostAsRewardedVideoNetwork)
			{
				rewardedVideoNetworks.Add(RewardedVideoNetwork.Chartboost);
			}
			#endif

			#if ENABLE_ADCOLONY
				rewardedVideoNetworks.Add(RewardedVideoNetwork.ADColony);
			#endif
			return rewardedVideoNetworks;
		}
		*/


		void Awake()
		{
			if(FindObjectOfType<AdsManager>() == null)
			{
				var o = new GameObject();
				o.SetActive(false);
			
				var adsManager = o.AddComponent<AdsManager>();

				adsManager.enabled = false;

				o.name = "_AdsManager";

				adsManager.adIds = this._ADIDS;
				adsManager.randomize = _ADIDS.RandomizeNetworks;

				adsManager.bannerNetworks = _ADIDS.bannerNetworks;
				adsManager.interstitialNetworks = _ADIDS.interstitialNetworks;
				adsManager.videoNetworks = _ADIDS.videoNetworks;
				adsManager.rewardedVideoNetworks = _ADIDS.rewardedVideoNetworks;


//				adsManager.bannerNetwork = _ADIDS.bannerNetwork;
//				adsManager.interstitialNetworks = InitInterstitialNetworks();
//				adsManager.videoNetworks = InitVideoNetworks ();
//				adsManager.rewardedVideoNetworks = InitRewardedVideoNetworks ();

				//adsManager.showOnlyFirstNetworkAtRun = _ADIDS.showOnlyFirstNetworkAtRun;
				adsManager.interstitialNetworkToShowAtRun = _ADIDS.interstitialNetworkToShowAtRun;
				adsManager.showInterstitialFirstRun = _ADIDS.showInterstitialFirstRun;
			
				adsManager.showBannerAtRun = _ADIDS.showBannerAtRun;

				#if ENABLE_ADMOB
				adsManager.AdmobBannerSize = _ADIDS.AdmobBannerSize;
				adsManager.AdmobBannerPosition = _ADIDS.AdmobBannerPosition;
				#endif

				#if ENABLE_FACEBOOK
				adsManager.FacebookBannerSize =  _ADIDS.FacebookBannerSize;
				adsManager.FacebookBannerPosition = _ADIDS.FacebookBannerPosition;
				#endif

				adsManager.enabled = true;

				o.SetActive(true);

				adsManager.DOAwake();
				adsManager.DOStart();
			}

			Destroy(gameObject);
		}
	}
}