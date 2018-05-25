
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;



public class SampleScript : MonoBehaviour 
{
	public int NumberOfPlayToShowInterstitial = 5;

	public float probaToShowVideoInterstitial = 30;

	void ShowAds()
	{

		if(PlayerPrefs.GetInt("COUNT_ADS",0) > NumberOfPlayToShowInterstitial)
		{

			if(UnityEngine.Random.Range(0,100) <= probaToShowVideoInterstitial)
			{
				if(AppAdvisory.Ads.AdsManager.instance.IsReadyVideoAds())
				{
					ShowVideoAds();
					return;
				}
			}

			if(AppAdvisory.Ads.AdsManager.instance.IsReadyInterstitial())
			{
				ShowInterstitialAds();
				return;
			}

			if(AppAdvisory.Ads.AdsManager.instance.IsReadyVideoAds())
			{
				ShowVideoAds();
				return;
			}
		}
		else
		{
			PlayerPrefs.SetInt("COUNT_ADS", PlayerPrefs.GetInt("COUNT_ADS",0) + 1);
			PlayerPrefs.Save();
		}
	}

	void ShowVideoAds()
	{
		PlayerPrefs.SetInt("COUNT_ADS",0);
		PlayerPrefs.Save();

		#if APPADVISORY_ADS
		AppAdvisory.Ads.AdsManager.instance.ShowVideoAds();
		#endif
	}

	void ShowInterstitialAds()
	{
		PlayerPrefs.SetInt("COUNT_ADS",0);
		PlayerPrefs.Save();

		#if APPADVISORY_ADS
		AppAdvisory.Ads.AdsManager.instance.ShowInterstitial();
		#endif
	}
}

