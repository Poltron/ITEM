
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
using System;
using System.Collections;
using System.Collections.Generic;

#if ENABLE_ADCOLONY

namespace AppAdvisory.Ads
{
	public class AAADColony : AdBase, IVideoAds, IRewardedVideo
	{
		
		// AdColony values
		bool IsAdInitialized = false;
		bool IsAdAvailable
		{
			get
			{
				Debug.Log ("AAADColony - GET IsAdAvailable = " + _isAIsAdAvailable);

				return _isAIsAdAvailable;	
			}

			set
			{ 
				_isAIsAdAvailable = value;
				Debug.Log ("AAADColony - SET IsAdAvailable = " + value);
			}
		}

		bool _isAIsAdAvailable = false;

		AdColony.InterstitialAd Ad = null;

		float currencyPopupTimer = 0.0f;

		public string Name()
		{
			return "AAADColony";
		}

		public void Init()
		{
			ConfigureAds();
		}

		void ConfigureAds()
		{

			Debug.Log("**** Configure ADC SDK ****");

			AdColony.Ads.OnConfigurationCompleted -= OnConfigurationCompleted;
			AdColony.Ads.OnConfigurationCompleted += OnConfigurationCompleted;

			AdColony.AppOptions appOptions = new AdColony.AppOptions();
			appOptions.DisableLogging = false;
//			appOptions.UserId = "foo";
			appOptions.AdOrientation = AdColony.AdOrientationType.AdColonyOrientationAll;

			string[] zoneIDs = new string[] {adIds.ADCOLONY_InterstitialVideoZoneID, adIds.ADCOLONY_RewardedVideoZoneID};

			AdColony.Ads.Configure(adIds.ADCOLONY_appId, appOptions, zoneIDs);
		}


		#region ADColony Callbacks

		void OnConfigurationCompleted (List<AdColony.Zone> zones_)
		{

			if (zones_ == null || zones_.Count <= 0) {
				Debug.Log("zones_ == null || zones_.Count <= 0 => configure again!!!s");

				Invoke ("ConfigureAds", 1);
				// Show the configure asteroid again.
			}
			else {
				IsAdInitialized = true;

				Debug.Log("Successfully configured...");


				AdColony.Ads.OnRequestInterstitial -= OnRequestInterstitial;
				AdColony.Ads.OnRequestInterstitial += OnRequestInterstitial;

				AdColony.Ads.OnRequestInterstitialFailedWithZone -= OnRequestInterstitialFailedWithZone;
				AdColony.Ads.OnRequestInterstitialFailedWithZone += OnRequestInterstitialFailedWithZone;

				AdColony.Ads.OnOpened -= OnOpened;
				AdColony.Ads.OnOpened += OnOpened;

				AdColony.Ads.OnClosed -= OnClosed;
				AdColony.Ads.OnClosed += OnClosed;

				AdColony.Ads.OnExpiring -= OnExpiring;
				AdColony.Ads.OnExpiring += OnExpiring;

				AdColony.Ads.OnIAPOpportunity -= OnIAPOpportunity;
				AdColony.Ads.OnIAPOpportunity += OnIAPOpportunity;

				AdColony.Ads.OnRewardGranted -= OnRewardGranted;
				AdColony.Ads.OnRewardGranted += OnRewardGranted;

				AdColony.Ads.OnCustomMessageReceived -= OnCustomMessageReceived;
				AdColony.Ads.OnCustomMessageReceived += OnCustomMessageReceived;

				CacheVideoAds ();

				CacheRewardedVideo ();

				// Successfully configured... show the request ad asteroid.
			}
		}

		void OnRequestInterstitial (AdColony.InterstitialAd ad_)
		{
			Debug.Log("AdColony.Ads.OnRequestInterstitial called");
			Ad = ad_;
			IsAdAvailable = true;

			// Successfully requested ad... show the play ad asteroid.
		}

		void OnRequestInterstitialFailedWithZone (string zone)
		{
			Debug.Log("AdColony.Ads.OnRequestInterstitialFailed with zone : " + zone);
			IsAdAvailable = false;

			// Request Ad failed... show the request ad asteroid.
		}

		void OnOpened (AdColony.InterstitialAd ad_)
		{
			Debug.Log("AdColony.Ads.OnOpened called");
			IsAdAvailable = false;

			// Ad started playing... show the request ad asteroid for the next ad.
		}

		void OnClosed (AdColony.InterstitialAd ad_)
		{
			Debug.Log("AdColony.Ads.OnClosed called, expired: " + ad_.Expired);
			IsAdAvailable = false;
		}

		void OnExpiring (AdColony.InterstitialAd ad_)
		{
			Debug.Log("AdColony.Ads.OnExpiring called");
			Ad = null;
			IsAdAvailable = false;

			// Current ad expired... show the request ad asteroid.
		}

		void OnIAPOpportunity (AdColony.InterstitialAd ad_, string iapProductId_, AdColony.AdsIAPEngagementType engagement_)
		{
			Debug.Log("AdColony.Ads.OnIAPOpportunity called");
		}

		void OnRewardGranted (string zoneId, bool success, string name, int amount)
		{
			Debug.Log(string.Format("AdColony.Ads.OnRewardGranted called\n\tzoneId: {0}\n\tsuccess: {1}\n\tname: {2}\n\tamount: {3}", zoneId, success, name, amount));

			if(successShowInterstitial != null)
			{
				successShowInterstitial(success);
			}

			successShowInterstitial = null;
		}

		void OnCustomMessageReceived (string type, string message)
		{
			Debug.Log(string.Format("AdColony.Ads.OnCustomMessageReceived called\n\ttype: {0}\n\tmessage: {1}", type, message));
		}

		#endregion

		#region VSADS interface methods

		public bool IsReadyVideoAds()
		{
			return IsAdAvailable;
		}

		public void CacheVideoAds()
		{
			if (IsAdInitialized) {
				Debug.Log ("**** Cache ADColony Video Ads ****");

				AdColony.AdOptions adOptions = new AdColony.AdOptions ();
				adOptions.ShowPrePopup = true;
				adOptions.ShowPostPopup = true;

				AdColony.Ads.RequestInterstitialAd (adIds.ADCOLONY_InterstitialVideoZoneID, adOptions);
			} else {
				Debug.Log ("**** Adcolony trying to CACHE Video Ads and Adcolony not yet initialized ****");

			}
		}

		public void ShowVideoAds()
		{
			if (IsAdInitialized) {
	
			if (Ad != null) {
				Debug.Log ("**** Play ADColony video ads ****");

				AdColony.Ads.ShowAd (Ad);
				NotifyVideoInterstitialOpened ();
			} else {
				Debug.Log ("**** Cant' Play ADColony video ads ==> Ad == null ****");
			}
			} else {
				Debug.Log ("**** Adcolony trying to SHOW Video Ads and Adcolony not yet initialized ****");

			}
		}

		public void NotifyInterstitialOpened()
		{
			AdsManager.DOInterstitialOpened();
		}

		public void NotifyInterstitialClosed()
		{
			AdsManager.DOInterstitialClosed();
		}

		public void NotifyVideoInterstitialClosed()
		{
			AdsManager.DOVideoInterstitialClosed();
		}

		public void NotifyVideoInterstitialOpened()
		{
			AdsManager.DOVideoInterstitialOpened();
		}

		public bool IsReadyRewardedVideo()
		{
			return IsAdAvailable;
		}

		public void CacheRewardedVideo()
		{
			if (IsAdInitialized) {
				
			Debug.Log("**** Cache ADColony REWARDED Video Ads ****");

			AdColony.AdOptions adOptions = new AdColony.AdOptions();
			adOptions.ShowPrePopup = true;
			adOptions.ShowPostPopup = true;

			AdColony.Ads.RequestInterstitialAd(adIds.ADCOLONY_RewardedVideoZoneID, adOptions);
			} else {
				Debug.Log ("**** Adcolony trying to CACHE Rewarded Video Ads and Adcolony not yet initialized ****");

			}
		}

		Action<bool> successShowInterstitial = null;

		public void ShowRewardedVideo(Action<bool> success)
		{
			if (IsAdInitialized) {

			successShowInterstitial = success;

			if (Ad != null) {
				Debug.Log ("**** Play ADColony REWARDED video ****");

				AdColony.Ads.ShowAd (Ad);
			} else {
				Debug.Log ("**** Can't Play ADColony REWARDED video ==> Ad == null ****");

			}
			} else {
				Debug.Log ("**** Adcolony trying to SHOW Rewarded Video Ads and Adcolony not yet initialized ****");

			}
		}

		#endregion

	}
}

#endif