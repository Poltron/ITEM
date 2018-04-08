
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

#if ENABLE_FACEBOOK
using AudienceNetwork;

namespace AppAdvisory.Ads
{
	public class AAFacebook : AdBase, IBanner, IInterstitial
	{
		public string bannerId
		{
			get
			{
				return adIds.facebookBannerID;
			}
		}

		public string interstitialID
		{
			get
			{
				return adIds.facebookInterstitialID;
			}
		}
			
		public string Name()
		{
			return "AAFACEBOOK";
		}

		public void Init()
		{
			if(bannerId != null && string.IsNullOrEmpty(bannerId) == false)
				Debug.LogWarning("AAFacebook - Init bannerId = " + bannerId);

			if(interstitialID != null && string.IsNullOrEmpty(interstitialID) == false)
				Debug.LogWarning("AAFacebook - Init interstitialID = " + interstitialID);

			if (adIds.childrenPrivacy == ChildrenPrivacy.ChildDirected) 
			{
				Debug.LogWarning("AAFacebook - Init setChildIsDirected = true");
				//AdSettings.SetIsChildDirected (true);
			}
			else if (adIds.childrenPrivacy == ChildrenPrivacy.NotChildDirected) 
			{
				Debug.LogWarning("AAFacebook - Init setChildIsDirected = false");
				//AdSettings.SetIsChildDirected (false);
			}

			RequestBanner();
			CacheInterstitialStartup ();
		}
			
		AdView adView;
		AdSize FacebookBannerSize = AdSize.BANNER_HEIGHT_50;
		double bannerPositionY = 0;

		public void SetBannerSizeAndPosition(AdSize adSize, FacebookBannerPosition adPosition) {
			FacebookBannerSize = adSize;

			if (adPosition == FacebookBannerPosition.Top) 
			{
				bannerPositionY = 0;
			} else if (adPosition == FacebookBannerPosition.Bottom) 
			{
				double bannerHeight = 50;
				if (adSize == AdSize.BANNER_HEIGHT_50)
					bannerHeight = 50;
				else if (adSize == AdSize.BANNER_HEIGHT_90)
					bannerHeight = 90;
				else if (adSize == AdSize.RECTANGLE_HEIGHT_250)
					bannerHeight = 250;
				

				bannerPositionY = AudienceNetwork.Utility.AdUtility.height () - bannerHeight;
			}
		}

		private void RequestBanner()
		{
			if(!string.IsNullOrEmpty(bannerId))
			{
				Debug.LogWarning("AAFacebook - RequestBanner with bannerid = " + bannerId);	

				this.adView = new AdView(bannerId, FacebookBannerSize);
				this.adView.Register (this.gameObject);

				this.adView.AdViewDidLoad = (delegate() {
					Debug.Log ("Ad view loaded.");
				});
				this.adView.AdViewDidFailWithError = (delegate(string error) {
					Debug.Log ("Ad view failed to load with error: " + error);
				});
				this.adView.AdViewWillLogImpression = (delegate() {
					Debug.Log ("Ad view logged impression.");
				});
				this.adView.AdViewDidClick = (delegate() {
					Debug.Log ("Ad view clicked.");
				});
				this.adView.LoadAd ();

			}
			else
			{
				Debug.LogWarning("AAFacebook - RequestBanner ERROR ID IS NULL!!");	
			}
		}

		private void OnAdViewDidLoad () {
			print ("AAFacebook- OnAdViewDidLoad"); 
//			adView.Show (bannerPositionY);
			ShowBanner();
		}
			
		public void ShowBanner()
		{
			if(adView == null)
			{
				Debug.LogWarning("AAFacebook - ShowBanner bannerView == null ===> requestBanner");
				RequestBanner();

				adView.adViewDidLoad += OnAdViewDidLoad;
			}
			else
			{
				Debug.LogWarning("AAFacebook - ShowBanner bannerView != null ----> show");

				Debug.Log (adView);
				adView.Show(bannerPositionY);

				AdsManager.DOBannerShowed();
				 
				 
			}
		}
			
		public void HideBanner()
		{
			Debug.LogWarning("AAAdmob - HideBanner");

			DestroyBanner ();
		}

		public void DestroyBanner()
		{
			Debug.LogWarning("AAAdmob - DestroyBanner");

			if(adView != null)
			{
				adView.AdViewDidLoad -= OnAdViewDidLoad;
				adView.Dispose();
			}

			adView = null;

			 
			 
		}


		private InterstitialAd interstitialAd;
		private bool isLoaded = false;

		public bool IsReadyInterstitial() 
		{
			if(!isLoaded)
			{
				CacheInterstitial ();
			}
			return isLoaded;
		}
			
		public bool IsReadyInterstitialStartup() 
		{
			if(!isLoaded)
			{
				CacheInterstitialStartup ();
			}
			return isLoaded;
		}

		public void CacheInterstitial() 
		{
			this.interstitialAd = new InterstitialAd (interstitialID);;
			this.interstitialAd.Register (this.gameObject);

			this.interstitialAd.InterstitialAdDidLoad = (delegate() {
				Debug.Log ("Interstitial ad loaded.");
				this.isLoaded = true;
			});
			interstitialAd.InterstitialAdDidFailWithError = (delegate(string error) {
				Debug.Log ("Interstitial ad failed to load with error: " + error);
			});
			interstitialAd.InterstitialAdWillLogImpression = (delegate() {
				Debug.Log ("Interstitial ad logged impression.");
			});
			interstitialAd.InterstitialAdDidClick = (delegate() {
				Debug.Log ("Interstitial ad clicked.");
				NotifyInterstitialOpened();
			});
			interstitialAd.InterstitialAdDidClose = (delegate() {
				Debug.Log ("Interstitial ad closed.");
				DestroyInterstitial();
				CacheInterstitial ();
				NotifyInterstitialClosed();
			});
				
			this.interstitialAd.LoadAd ();
		}
			
		public void CacheInterstitialStartup() 
		{
			CacheInterstitial ();
		}

		private Action<bool> successShowInterstitial = null;

		public void ShowInterstitial(Action<bool> succes) 
		{
			if (this.isLoaded) {
				this.interstitialAd.Show ();
				this.isLoaded = false;

				if(succes != null)
					succes (true);
			} else {
				if(succes != null)
					succes (false);
			}
				
		}

		public void ShowInterstitialStartup(Action<bool> succes) 
		{
			ShowInterstitial (succes);
		}

		public void NotifyInterstitialOpened()
		{
			AdsManager.DOInterstitialOpened();
		}

		public void NotifyInterstitialClosed()
		{
			AdsManager.DOInterstitialClosed();
		}

		private void DestroyInterstitial()
		{
			if (this.interstitialAd != null) {
				this.interstitialAd.Dispose ();
			}
		}
	}
}

#endif