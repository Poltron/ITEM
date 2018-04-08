/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obslolete
#pragma warning disable 0108 
#pragma warning disable 0162 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AppAdvisory;

namespace AppAdvisory.Ads
{
	public class ADIDS : ScriptableObject
	{
		public bool ANDROID_AMAZON = false;

		public bool DEBUG = false;

		public bool LoadNextSceneWhenAdLoaded = false;

		public bool RandomizeNetworks = true;

		public bool FIRST_TIME = true;
		public bool EnableAdmob = false;
		public bool EnableChartboost = false;
		public bool EnableAdcolony = false;
		public bool EnableFacebook = false;

		public bool showOnlyFirstNetworkAtRun = true;
		public InterstitialNetwork interstitialNetworkToShowAtRun = InterstitialNetwork.NULL;

		public bool showInterstitialFirstRun = false;
		public bool showBannerAtRun = true;

		//public BannerNetwork bannerNetwork = BannerNetwork.NULL;

		public bool isAdsFoldoutOpened = false;
		public bool isMediationNetworkFoldoutOpened = false;

		public bool IsAdmobSettingsOpened = false;
		public bool IsAdmobIOSSettingsOpened = false;
		public bool IsAdmobANDROIDSettingsOpened = false;
		public bool IsAdmobAMAZONSettingsOpened = false;
		public bool IsUnityAdsSettingsOpened = false;

		public bool IsChartboostSettingsOpened = false;
		public bool IsChartboostIOSSettingsOpened = false;
		public bool IsChartboostAndroidSettingsOpened = false;
		public bool IsChartboostAmazonSettingsOpened = false;

		public bool IsADColonySettingsOpened = false;
		public bool IsADColonyIOSSettingsOpened = false;
		public bool IsADColonyAndroidSettingsOpened = false;


		public bool rewardedVideoAlwaysReadyInSimulator = false;
		public bool rewardedVideoAlwaysSuccessInSimulator = false;
		public bool ShowIntertitialAtStart = true;

		public bool IsFacebookSettingsOpened = false;
		public bool IsFacebookIOSSettingsOpened = false;
		public bool IsFacebookANDROIDSettingsOpened = false;
		public bool IsFacebookAMAZONSettingsOpened = false;

		public bool IsBannerNetworksOpened = false;
		public bool IsInterstitialNetworksOpened = false;
		public bool IsVideoNetworksOpened = false;
		public bool IsRewardedVideoNetworksOpened = false;


		public bool EnableUnityAds = false;


		#region INTERSTITIAL NETWORKS

		public List<InterstitialNetwork> interstitialNetworks = new List<InterstitialNetwork> ();

		#if ENABLE_ADMOB
		public bool useAdmobAsInterstitialNetwork = false;
		#endif

		#if CHARTBOOST
		public bool useChartboostAsInterstitialNetwork = false;
		#endif

		#if ENABLE_FACEBOOK
		public bool useFacebookAsInterstitialNetwork = false;
		#endif

		#endregion

		#region BANNER NETWORKS

		public List<BannerNetwork> bannerNetworks = new List<BannerNetwork> ();

		#if ENABLE_ADMOB
		public bool useAdmobAsBannerNetwork = false;
		#endif

		#if ENABLE_FACEBOOK
		public bool useFacebookAsBannerNetwork = false;
		#endif

		#endregion

		#region VIDEO NETWORKS

		public List<VideoNetwork> videoNetworks = new List<VideoNetwork> ();


		#if ENABLE_UNITY_ADS
		//#if UNITY_ADS
		public bool useUnityAdsAsBannerNetwork = false;
		#endif

		#if ENABLE_ADCOLONY
		public bool useAdColonyAsBannerNetwork = false;
		#endif

		#endregion

		#region REWARDED VIDEO NETWORKS

		public List<RewardedVideoNetwork> rewardedVideoNetworks = new List<RewardedVideoNetwork> ();


		#if ENABLE_ADMOB
		public bool useAdmobAsRewardedVideoNetwork = false;
		#endif

		#if ENABLE_UNITY_ADS
		//#if UNITY_ADS
		public bool useUnityAdsAsRewardedVideoNetwork = false;
		#endif

		#if CHARTBOOST
		public bool useChartboostAsRewardedVideoNetwork = false;
		#endif

		#if ENABLE_ADCOLONY
		public bool useAdColonyAsRewardedVideoNetwork = false;
		#endif

		#endregion

		#if ENABLE_ADMOB || ENABLE_FACEBOOK
		public ChildrenPrivacy childrenPrivacy = ChildrenPrivacy.NONE;
		#endif

		#if ENABLE_ADMOB
		public string testDeviveId = "8fa42327347fc830609a54a833e611ed1cc716a7";
		public bool lookForGameAds = true;
		public GoogleMobileAds.Api.AdSize AdmobBannerSize = GoogleMobileAds.Api.AdSize.SmartBanner;
		public GoogleMobileAds.Api.AdPosition AdmobBannerPosition = GoogleMobileAds.Api.AdPosition.Bottom;
		#endif

		#if ENABLE_FACEBOOK
		public AudienceNetwork.AdSize FacebookBannerSize =  AudienceNetwork.AdSize.BANNER_HEIGHT_50;
		public FacebookBannerPosition FacebookBannerPosition = FacebookBannerPosition.Bottom;
		#endif


//		[SerializeField] public List<InterstitialNetwork> interstitialNetworks = new List<InterstitialNetwork>(new InterstitialNetwork[] {InterstitialNetwork.NULL});
//		[SerializeField] public List<VideoNetwork> videoNetworks = new List<VideoNetwork>(new VideoNetwork[] {VideoNetwork.NULL});
//		[SerializeField] public List<RewardedVideoNetwork> rewardedVideoNetworks = new List<RewardedVideoNetwork>( new RewardedVideoNetwork[] {RewardedVideoNetwork.NULL});

		#region CHARTBOOST
		public string ChartboostAppIdIOS = "5238852117ba479f20000006";
		public string ChartboostAppSignatureIOS = "acf88792a96bb162ae50e93bf3ef93739b3b740e";

		public string ChartboostAppIdAndroid = "58c58f8d04b016274d96c126";
		public string ChartboostAppSignatureAndroid = "9688ddff2be5b4fd8ad4657586df4086d234bece";

		public string ChartboostAppIdAmazon = "58c58fb5f6cd4539a7e1ba79";
		public string ChartboostAppSignatureAmazon = "6e5d86c5e49d2235c5ea82717f4cc05c71b2b668";

		public string ChartboostAppId
		{
			get
			{
				#if UNITY_IOS
				return ChartboostAppIdIOS;
				#endif

				#if UNITY_ANDROID
				if(isAmazon)
				return ChartboostAppIdAmazon;
				else
				return ChartboostAppIdAndroid;
				#endif

				#if !UNITY_IOS && !UNITY_ANDROID
				return "";
				#endif
			}
		}

		public string ChartboostAppSignature
		{
			get
			{
				#if UNITY_IOS
				return ChartboostAppSignatureIOS;
				#endif

				#if UNITY_ANDROID
				if(isAmazon)
				return ChartboostAppSignatureAmazon;
				else
				return ChartboostAppSignatureAndroid;
				#endif

				#if !UNITY_IOS && !UNITY_ANDROID
				return "";
				#endif
			}
		}
		#endregion

		#region ADMOB

		#if UNITY_ANDROID
		public bool isAmazon = false;
		#endif


		public string AdmobBannerIdIOS = "";//"ca-app-pub-4501064062171971/8068421245";

		public string AdmobInterstitialIdIOS = "";//"ca-app-pub-4501064062171971/9545154449";

		public string AdmobRewardedVideoIdIOS = "";//"ca-app-pub-4501064062171971/6591688042";

		public string AdmobBannerIdANDROID = "";//"ca-app-pub-4501064062171971/7928820445";

		public string AdmobInterstitialIdANDROID = "";//"ca-app-pub-4501064062171971/9405553649";

		public string AdmobRewardedVideoIdANDROID = "";//"ca-app-pub-4501064062171971/1882286844";

		public string AdmobBannerIdAMAZON = "";//"ca-app-pub-4501064062171971/6312486440";

		public string AdmobInterstitialIdAMAZON = "";//"ca-app-pub-4501064062171971/7789219647";

		public string AdmobRewardedVideoIdAMAZON = "";//"ca-app-pub-4501064062171971/9265952846";

		public string admobBannerID
		{
			get
			{
				#if UNITY_IOS
				return AdmobBannerIdIOS;
				#endif

				#if UNITY_ANDROID
				bool isAmazon = false;

				#if ANDROID_AMAZON
				isAmazon = true;
				#endif
				if(!isAmazon)
				return AdmobBannerIdANDROID;
				else
				return AdmobBannerIdAMAZON;
				#endif

				return "";
			}
		}

		public string admobInterstitialID
		{
			get
			{
				#if UNITY_IOS
				return AdmobInterstitialIdIOS;
				#endif

				#if UNITY_ANDROID
				bool isAmazon = false;

				#if ANDROID_AMAZON
				isAmazon = true;
				#endif

				if(!isAmazon)
				return AdmobInterstitialIdANDROID;
				else
				return AdmobInterstitialIdAMAZON;
				#endif

				return "";
			}
		}

		public string admobRewardedVideoID
		{
			get
			{
				#if UNITY_IOS
				return AdmobRewardedVideoIdIOS;
				#endif

				#if UNITY_ANDROID
				bool isAmazon = false;

				#if ANDROID_AMAZON
				isAmazon = true;
				#endif

				if(!isAmazon)
				return AdmobRewardedVideoIdANDROID;
				else
				return AdmobRewardedVideoIdAMAZON;
				#endif

				return "";
			}
		}

		#endregion

		#region UNITYADS
		#if ENABLE_UNITY_ADS
		//#if UNITY_ADS
		public bool activateRegularInterstitial_UNITY_ADS = false;
		public bool activateRewardedVideo_UNITY_ADS = false;
		public string rewardedVideoZoneUnityAds = "rewardedVideo";
		#endif
		#endregion

		#region ADCOLONY

		#if ENABLE_ADCOLONY
		public bool activateRegularInterstitial_ADCOLONY = false;
		public bool activateRewardedVideo_ADCOLONY = false;
		//		 Arbitrary version number
		public string version = "1.1";



		public string AdColonyAppID_iOS = "app4901e09bc6a84e2e90";

		public string ADCOLONY_appId
		{
			get
			{
		#if UNITY_IOS
		return AdColonyAppID_iOS;
		#endif

		#if UNITY_ANDROID
		return AdColonyAppID_ANDROID;
		#endif

				return "";

			}
		}




		public string AdColonyInterstitialVideoZoneKEY_iOS = "VideoZone1";
		public string AdColonyInterstitialVideoZoneID_iOS = "vzd728fbab1a2948498e";

		public string AdColonyInterstitialVideoZoneKEY_ANDROID = "VideoZone1";
		public string AdColonyInterstitialVideoZoneID_ANDROID = "vz96292e0ce9c44b728f";


		public string ADCOLONY_InterstitialVideoZoneKEY
		{
			get
			{

		#if UNITY_IOS
		return AdColonyInterstitialVideoZoneKEY_iOS;
		#endif

		#if UNITY_ANDROID
		return AdColonyInterstitialVideoZoneKEY_ANDROID;
		#endif

				return "";
			}
		}





		public string ADCOLONY_InterstitialVideoZoneID
		{
			get
			{


		#if UNITY_IOS
		return AdColonyInterstitialVideoZoneID_iOS;
		#endif

		#if UNITY_ANDROID
		return AdColonyInterstitialVideoZoneID_ANDROID;
		#endif
				return "";

			}
		}


		public string AdColonyAppID_ANDROID = "appb7de26187820418c97";


		public string AdColonyRewardedVideoZoneKEY_iOS = "V4VCZone1";

		public string AdColonyRewardedVideoZoneID_iOS = "vzbfea02bb641f4a6a95";





		public string AdColonyRewardedVideoZoneKEY_ANDROID = "V4VCZone1";

		public string AdColonyRewardedVideoZoneID_ANDROID = "v4vc74c3fdbf7da34df2a4";

		public string ADCOLONY_RewardedVideoZoneID
		{
			get
			{

		#if UNITY_IOS
		return AdColonyRewardedVideoZoneID_iOS;
		#endif

		#if UNITY_ANDROID
		return AdColonyRewardedVideoZoneID_ANDROID;
		#endif

				return "";

			}
		}

		public string ADCOLONY_RewardedVideoZoneKEY
		{
			get
			{
		#if UNITY_IOS
		return AdColonyRewardedVideoZoneKEY_iOS;
		#endif

		#if UNITY_ANDROID
		return AdColonyRewardedVideoZoneKEY_ANDROID;
		#endif

				return "";
			}
		}
		#endif


		#endregion

		#region FACEBOOK

		public string FacebookBannerIdIOS = "338722809862911_338723276529531";

		public string FacebookInterstitialIdIOS = "338722809862911_338723339862858";

		public string FacebookBannerIdANDROID = "338722809862911_338723476529511";

		public string FacebookInterstitialIdANDROID = "338722809862911_338723549862837";

		public string FacebookBannerIdAMAZON = "338722809862911_338723893196136";

		public string FacebookInterstitialIdAMAZON = "338722809862911_338723969862795";

		public string facebookBannerID
		{
			get
			{
				#if UNITY_IOS
				return FacebookBannerIdIOS;
				#endif

				#if UNITY_ANDROID
				bool isAmazon = false;

				#if ANDROID_AMAZON
				isAmazon = true;
				#endif
				if(!isAmazon)
				return FacebookBannerIdANDROID;
				else
				return FacebookBannerIdAMAZON;
				#endif

				return "";
			}
		}

		public string facebookInterstitialID
		{
			get
			{
				#if UNITY_IOS
				return FacebookInterstitialIdIOS;
				#endif

				#if UNITY_ANDROID
				bool isAmazon = false;

				#if ANDROID_AMAZON
				isAmazon = true;
				#endif

				if(!isAmazon)
				return FacebookInterstitialIdANDROID;
				else
				return FacebookInterstitialIdAMAZON;
				#endif

				return "";
			}
		}

		#endregion
		#region EDITOR
		public static readonly string PATH = "Assets/_AppAdvisory/Very_Simple_Ads/";
		public static readonly string NAME = "Ads_Settings";

		private static string PathToAsset 
		{
			get 
			{
				return PATH + NAME + ".asset";
			}
		}

		#if UNITY_EDITOR

		[MenuItem("Assets/Create/AppAdvisory/AdSettings")]
		public static void CreateAdSettings()
		{
			ADIDS asset = ScriptableObject.CreateInstance<ADIDS>();

			AssetDatabase.CreateAsset(asset, PathToAsset);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
		}

		#endif

		#endregion


	}
}