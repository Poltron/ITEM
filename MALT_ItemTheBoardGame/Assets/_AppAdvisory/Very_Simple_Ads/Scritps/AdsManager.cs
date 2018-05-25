
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


#pragma warning disable 0162 // code unreached.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obslolete
#pragma warning disable 0108 
#pragma warning disable 0649 //never used

//#define IAD
//#define ENABLE_ADMOB
//#define CHARTBOOST
//#define ENABLE_ADCOLONY
//#define ADCOLONY_INTERSTITIAL
//#define ADCOLONY_REWARDED_VIDEO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif


namespace AppAdvisory.Ads
{
	/// <summary>
	/// This is a static class that references a singleton object in order to provide a simple interface for easely configure Ads in the game
	///
	/// Class in charge to display ads in the game (banners, interstitials and rewarded videos) - please refere to the ADS_INTEGRATION_DOCUMENTATION.PDF
	/// </summary>
	public class AdsManager : AppAdvisory.Ads.Singleton<AdsManager>
	{
		protected AdsManager () {} // guarantee this will be always a singleton only - can't use the constructor!


		[HideInInspector]
		public bool showOnlyFirstNetworkAtRun = false;

		[HideInInspector]
		public InterstitialNetwork interstitialNetworkToShowAtRun = InterstitialNetwork.NULL;

		[HideInInspector]
		public bool showInterstitialFirstRun = true;

		[HideInInspector]
		public bool showBannerAtRun = true;


		#if ENABLE_ADMOB
		[HideInInspector]
		public GoogleMobileAds.Api.AdSize AdmobBannerSize = GoogleMobileAds.Api.AdSize.SmartBanner;
		[HideInInspector]
		public GoogleMobileAds.Api.AdPosition AdmobBannerPosition = GoogleMobileAds.Api.AdPosition.Bottom;
		#endif


		#if ENABLE_FACEBOOK
		[HideInInspector]
		public AudienceNetwork.AdSize FacebookBannerSize =  AudienceNetwork.AdSize.BANNER_HEIGHT_50;
		[HideInInspector]
		public FacebookBannerPosition FacebookBannerPosition = FacebookBannerPosition.Bottom;
		#endif

		public delegate void OnBannerShow();
		public static event OnBannerShow OnBannerShowed;
		public static void DOBannerShowed()
		{
			if(OnBannerShowed != null)
				OnBannerShowed();
		}

		public delegate void OnInterstitialOpen();
		public static event OnInterstitialOpen OnInterstitialOpened;
		public static void DOInterstitialOpened()
		{
			if(OnInterstitialOpened != null)
				OnInterstitialOpened();
		}

		public delegate void OnInterstitialClose();
		public static event OnInterstitialClose OnInterstitialClosed;
		public static void DOInterstitialClosed()
		{
			if(OnInterstitialClosed != null)
				OnInterstitialClosed();
		}


		public delegate void OnVideoInterstitialOpen();
		public static event OnVideoInterstitialOpen OnVideoInterstitialOpened;
		public static void DOVideoInterstitialOpened()
		{
			if(OnVideoInterstitialOpened != null)
				OnVideoInterstitialOpened();
		}

		public delegate void OnVideoInterstitialClose();
		public static event OnVideoInterstitialClose OnVideoInterstitialClosed;
		public static void DOVideoInterstitialClosed()
		{
			if(OnVideoInterstitialClosed != null)
				OnVideoInterstitialClosed();
		}

		[HideInInspector]
		public bool randomize = false;


		[SerializeField, HideInInspector] public List<BannerNetwork> bannerNetworks = new List<BannerNetwork> ();

		[SerializeField, HideInInspector] public BannerNetwork bannerNetwork;
		[SerializeField, HideInInspector] public List<InterstitialNetwork> interstitialNetworks = new List<InterstitialNetwork>();
		[SerializeField, HideInInspector] public List<VideoNetwork> videoNetworks = new List<VideoNetwork>();
		[SerializeField, HideInInspector] public List<RewardedVideoNetwork> rewardedVideoNetworks = new List<RewardedVideoNetwork>();

		IBanner currentBanner;
		List<IBanner> listBanners = new List<IBanner> ();
		//IBanner banner;
		List<IInterstitial> listInterstitials = new List<IInterstitial>();
		List<IVideoAds> listVideos = new List<IVideoAds>();
		List<IRewardedVideo> listRewardedVideos = new List<IRewardedVideo>();




		/// <summary>
		/// To store the time and know when we have to show an interstitial at game over if, and only if, basedTimeInterstitialAtGameOver = true
		/// </summary>
		float realTimeSinceStartup;

		public ADIDS m_adIds;
		[SerializeField] public ADIDS adIds
		{
			get
			{
				if(m_adIds == null)
				{
					Debug.LogWarning("ADIDS not in the scene!, please add it by clicking right on the hierarchy view -> app advisory > Very Simple Ads > Create Adids");

					GameObject gameObject = new GameObject("AdsInit");
					AdsInit a = gameObject.AddComponent<AdsInit>();
				}

				return m_adIds;
			}
			set
			{
				m_adIds = value;
			}
		}


		void Randomize()
		{
			if(randomize)
			{
				if(interstitialNetworks != null && interstitialNetworks.Count > 0)
					interstitialNetworks.Shuffle();

				if(videoNetworks != null && videoNetworks.Count > 0)
					videoNetworks.Shuffle();

				if(rewardedVideoNetworks != null && rewardedVideoNetworks.Count > 0)
					rewardedVideoNetworks.Shuffle();

				if (bannerNetworks != null && bannerNetworks.Count > 0)
					bannerNetworks.Shuffle ();


				if(listInterstitials != null && listInterstitials.Count > 0)
					listInterstitials.Shuffle();

				if(listVideos != null && listVideos.Count > 0)
					listVideos.Shuffle();

				if(listRewardedVideos != null && listRewardedVideos.Count > 0)
					listRewardedVideos.Shuffle();

				if (listBanners != null && listBanners.Count > 0)
					listBanners.Shuffle ();
			}
		}

		#if ENABLE_ADMOB
		void AddAdmob()
		{
		if(gameObject.GetComponent<AAAdmob>() == null)
		gameObject.AddComponent<AAAdmob>();

		GetComponent<AAAdmob>().SetBannerSizeAndPosition(AdmobBannerSize,AdmobBannerPosition);
		GetComponent<AAAdmob>().Init();
		}
		#endif

		#if ENABLE_UNITY_ADS
		//#if UNITY_ADS
		void AddUnityAds()
		{
		if(gameObject.GetComponent<AAUnityAds>() == null)
		gameObject.AddComponent<AAUnityAds>();
		}
		#endif

		#if ENABLE_ADCOLONY
		void AddADColony()
		{
		if(gameObject.GetComponent<AAADColony>() == null)
		gameObject.AddComponent<AAADColony>();

		GetComponent<AAADColony>().Init();
		}
		#endif

		#if CHARTBOOST
		void AddChartboost()
		{
		if(gameObject.GetComponent<AAChartboost>() == null)
		gameObject.AddComponent<AAChartboost>();

		GetComponent<AAChartboost>().Init();
		}
		#endif

		#if ENABLE_FACEBOOK
		void AddFacebook()
		{
		if (gameObject.GetComponent<AAFacebook> () == null)
		gameObject.AddComponent<AAFacebook> ();

		GetComponent<AAFacebook> ().SetBannerSizeAndPosition (FacebookBannerSize, FacebookBannerPosition);
		GetComponent<AAFacebook> ().Init ();
		}
		#endif

		IInterstitial FIRST_interstitialNetwork;

		public void DOAwake()
		{
			var a = FindObjectsOfType<AdsManager>();
			if(a!= null && a.Length > 1)
			{
				foreach(var ad in a)
				{
					if(ad != this)
					{
						Destroy(ad.gameObject);
					}
				}

				return;
			}


			#if ENABLE_ADMOB
			AddAdmob();
			//banner = GetComponent<AAAdmob>();
			#endif

			#if ENABLE_UNITY_ADS
			//#if UNITY_ADS
			AddUnityAds();
			#endif

			#if ENABLE_ADCOLONY
			AddADColony();
			#endif

			#if CHARTBOOST
			AddChartboost();
			#endif

			#if ENABLE_FACEBOOK
			AddFacebook();
			//banner = GetComponent<AAFacebook>();
			#endif

			if (bannerNetworks != null) 
			{
				if (listBanners == null)
					listBanners = new List<IBanner> ();

				foreach (BannerNetwork bannerNetworkType in bannerNetworks) 
				{
					#if ENABLE_ADMOB
					if(bannerNetworkType == BannerNetwork.Admob)
					{
					if(!listBanners.Contains(GetComponent<AAAdmob>()))
					{
					listBanners.Add(GetComponent<AAAdmob>());
					}
					}
					#endif

					#if ENABLE_FACEBOOK
					if(bannerNetworkType == BannerNetwork.Facebook) 
					{
					if(!listBanners.Contains(GetComponent<AAFacebook>()))
					{
					listBanners.Add(GetComponent<AAFacebook>());
					}
					}
					#endif
				}

			}

			if(interstitialNetworks != null)
			{
				if(listInterstitials == null)
					listInterstitials = new List<IInterstitial>();



				foreach(var m in interstitialNetworks)
				{
					#if ENABLE_ADMOB
					if(m == InterstitialNetwork.Admob)
					{
					if(!listInterstitials.Contains(GetComponent<AAAdmob>()))
					{
					listInterstitials.Add(GetComponent<AAAdmob>());
					if(m == interstitialNetworkToShowAtRun)
					FIRST_interstitialNetwork = GetComponent<AAAdmob>();
					}
					}
					#endif

					#if CHARTBOOST
					if(m == InterstitialNetwork.Chartboost)
					{
					if(!listInterstitials.Contains(GetComponent<AAChartboost>()))
					{
					listInterstitials.Add(GetComponent<AAChartboost>());
					if(m == interstitialNetworkToShowAtRun)
					FIRST_interstitialNetwork = GetComponent<AAChartboost>();
					}
					}
					#endif

					#if ENABLE_FACEBOOK
					if(m == InterstitialNetwork.Facebook)
					{
					if(!listInterstitials.Contains(GetComponent<AAFacebook>()))
					{
					listInterstitials.Add(GetComponent<AAFacebook>());
					if(m == interstitialNetworkToShowAtRun)
					FIRST_interstitialNetwork = GetComponent<AAFacebook>();
					}
					}
					#endif
				}


				//				if(listInterstitials != null && listInterstitials.Count > 0)
				//					FIRST_interstitialNetwork = listInterstitials[0];
			}

			if(videoNetworks != null)
			{
				if(listVideos == null)
					listVideos = new List<IVideoAds>();

				foreach(var m in videoNetworks)
				{

					#if ENABLE_ADCOLONY
					if(m == VideoNetwork.ADColony)
					{
					if(!listVideos.Contains(GetComponent<AAADColony>()))
					{
					listVideos.Add(GetComponent<AAADColony>());
					}
					}
					#endif

					#if ENABLE_UNITY_ADS
					//#if UNITY_ADS
					if(m == VideoNetwork.UnityAds)
					{
					if(!listVideos.Contains(GetComponent<AAUnityAds>()))
					{
					listVideos.Add(GetComponent<AAUnityAds>());
					}
					}
					#endif
				}
			}

			if(rewardedVideoNetworks != null)
			{
				if(listRewardedVideos == null)
					listRewardedVideos = new List<IRewardedVideo>();

				foreach(var m in rewardedVideoNetworks)
				{
					#if ENABLE_ADCOLONY
					if(m == RewardedVideoNetwork.ADColony)
					{
					if(!listRewardedVideos.Contains(GetComponent<AAADColony>()))
					{
					listRewardedVideos.Add(GetComponent<AAADColony>());
					}
					}
					#endif

					#if ENABLE_UNITY_ADS
					//#if UNITY_ADS
					if(m == RewardedVideoNetwork.UnityAds)
					{
					if(!listRewardedVideos.Contains(GetComponent<AAUnityAds>()))
					{
					listRewardedVideos.Add(GetComponent<AAUnityAds>());
					}
					}
					#endif

					#if CHARTBOOST
					if(m == RewardedVideoNetwork.Chartboost)
					{
					if(!listRewardedVideos.Contains(GetComponent<AAChartboost>()))
					{
					listRewardedVideos.Add(GetComponent<AAChartboost>());
					}
					}
					#endif

					#if ENABLE_ADMOB
					if(m == RewardedVideoNetwork.Admob)
					{
					if(!listRewardedVideos.Contains(GetComponent<AAAdmob>()))
					{
					listRewardedVideos.Add(GetComponent<AAAdmob>());
					}
					}
					#endif
				}
			}

			DontDestroyOnLoad(gameObject);
		}

		bool nextSceneIsLoaded = false;

		void LoadNextScene()
		{
			if(nextSceneIsLoaded)
				return;

			Debug.Log("@@@@@@ LOAD NEXT SCENE");


			nextSceneIsLoaded = true;
			#if UNITY_5_3_OR_NEWER
			//			SceneManager.LoadSceneAsync(1);
			SceneManager.LoadScene(1);
			#else
			Application.LoadLevel(1);
			#endif
		}

		IEnumerator checkInternetConnection(Action<bool> action)
		{
			WWW www = new WWW("http://google.com");

			yield return www;

			if (www.error != null) 
				action (false);
			else 
				action (true);
		} 

		bool waitForInternet = true;

		public void DOStart()
		{
			StopAllCoroutines();
			StartCoroutine(_DOStart());
		}

		IEnumerator _DOStart()
		{
			yield return new WaitForSeconds(0.3f);

			nextSceneIsLoaded = true;
			waitForInternet = true;

			if(adIds.LoadNextSceneWhenAdLoaded)
			{
				print("LoadNextSceneWhenAdLoaded = true");
				nextSceneIsLoaded = false;
			}
			else
			{
				print("LoadNextSceneWhenAdLoaded = false");
				nextSceneIsLoaded = true;
			}

			float timeSinceStartup = Time.realtimeSinceStartup;


			while(waitForInternet)
			{
				yield return new WaitForSeconds(1);

				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					waitForInternet = true;

					if((Time.realtimeSinceStartup - timeSinceStartup)  > 4)
					{
						print("@@@@@@ FORCE LoadNextScene() in while(waitForInternet)");
						waitForInternet = false;
						break;
					}
				} else {
					waitForInternet = false;
					break;
				}
				//
				//				StartCoroutine(checkInternetConnection((isConnected)=>{
				//
				//					if(waitForInternet)
				//					{
				//						if(isConnected)
				//						{
				//							Debug.LogWarning("@@@@@@ is connected!");
				//							waitForInternet = false;
				//						}
				//					}
				//					else if(!adIds.LoadNextSceneWhenAdLoaded)
				//					{
				//						nextSceneIsLoaded = true;
				//						LoadNextScene();
				//					} 
				//
				//
				//					if((Time.realtimeSinceStartup - timeSinceStartup)  > 4)
				//					{
				//						print("@@@@@@ FORCE LoadNextScene() in while(waitForInternet)");
				//						nextSceneIsLoaded = true;
				//						LoadNextScene();
				//					}
				//				}));
			}


			print("@@@@@@ OUT of while(waitForInternet)");


			//			CacheAllInterstitial();
			//			yield return new WaitForSeconds(0.1f);
			//			CacheAllInterstitialStartup();
			//			yield return new WaitForSeconds(0.1f);
			//			CacheAllVideoAds();
			//			yield return new WaitForSeconds(0.1f);
			//			CacheAllRewardedVideo();
			//			yield return new WaitForSeconds(0.1f);

			if(this.showBannerAtRun)
				StartCoroutine(CoroutShowBanner());

			if (adIds.ShowIntertitialAtStart && !nextSceneIsLoaded) 
			{
				yield return new WaitForSeconds (0.3f);

				StartCoroutine (CoroutShowInterstitial ());
			}


			if(!adIds.ShowIntertitialAtStart && bannerNetwork == BannerNetwork.NULL)
			{
				yield return new WaitForSeconds(0.3f);

				LoadNextScene();
			}



			yield return new WaitForSeconds(1);

			if(!nextSceneIsLoaded)
			{
				print("@@@@@@ BACKUP -- 10 sec after we don't have any ads so ... we force LoadNextScene!");
				LoadNextScene();
			}
		}

		bool bannerIsShowed = false;
		bool interstitialStartIsShowed = false;


		//		IEnumerator CoroutShowBanner()
		//		{
		//			bannerIsShowed = true;
		//
		//			if(bannerNetwork != BannerNetwork.NULL)
		//			{
		//				#if ENABLE_ADMOB
		//				if(bannerNetwork == BannerNetwork.Admob)
		//				{
		//					bannerIsShowed = false;
		//
		//					print("@@@@@@ show banner");
		//					yield return new WaitForSeconds(0.1f);
		//					banner.ShowBanner();
		//					OnBannerShowed += AdsManager_OnBannerShowed;
		//				}
		//				#endif
		//
		//				#if ENABLE_FACEBOOK
		//				if(bannerNetwork == BannerNetwork.Facebook)
		//				{
		//					bannerIsShowed = false;
		//
		//					print("@@@@@@ show banner");
		//					yield return new WaitForSeconds(0.1f);
		//					banner.ShowBanner();
		//					OnBannerShowed += AdsManager_OnBannerShowed;
		//				}
		//				#endif
		//			}
		//
		//			yield return 0;
		//		}

		IEnumerator CoroutShowBanner()
		{
			bannerIsShowed = true;

			ShowBanner();
			OnBannerShowed += AdsManager_OnBannerShowed;
			yield return null;


			/*
			if(listBanners != null && listBanners.Count > 0)
			{
				bannerIsShowed = false;

				bool canContinue = false;

				while(!canContinue)
				{
					foreach(var it in listBanners)
					{
						Debug.Log("@@@@@@ canContinue = " + canContinue + " for listInterstitial = " + it.GetType().ToString());

						if(!canContinue)
						{
							if(it)
							{
								canContinue = true;
							}
							else
							{
								it.CacheInterstitial();
							}
						}
					}

					yield return new WaitForSeconds(0.3f);
				}
			}
			*/

		}


		IEnumerator CoroutShowInterstitial()
		{
			interstitialStartIsShowed = true;

			Debug.LogWarning("@@@@@@ adIds.ShowIntertitialAtStart && !nextSceneIsLoaded = true");

			if(listInterstitials != null && listInterstitials.Count > 0)
			{
				interstitialStartIsShowed = false;

				bool canContinue = false;

				while(!canContinue)
				{
					foreach(var it in listInterstitials)
					{
						//Debug.Log("@@@@@@ canContinue = " + canContinue + " for listInterstitial = " + it.GetType().ToString());

						if(!canContinue)
						{
							if(it.IsReadyInterstitialStartup())
							{
								canContinue = true;
							}
							else
							{
								it.CacheInterstitial();
							}
						}
					}

					yield return new WaitForSeconds(0.3f);
				}
			}

			yield return new WaitForSeconds(1f);

			this.ShowInterstitialStartup();

			OnInterstitialOpened += AdsManager_OnInterstitialOpened;
		}

		void AdsManager_OnBannerShowed ()
		{
			print("@@@@@@ AdsManager_OnBannerShowed");

			OnBannerShowed -= AdsManager_OnBannerShowed;

			bannerIsShowed = true;

			if(!nextSceneIsLoaded)
			{
				StopCoroutine("CoroutLoadNextScene");
				StartCoroutine("CoroutLoadNextScene");
			}
		}

		void AdsManager_OnInterstitialOpened ()
		{
			print("@@@@@@ AdsManager_OnInterstitialOpened");

			OnInterstitialOpened -= AdsManager_OnInterstitialOpened;

			interstitialStartIsShowed = true;

			if(!nextSceneIsLoaded)
			{
				StopCoroutine("CoroutLoadNextScene");
				StartCoroutine("CoroutLoadNextScene");
			}
		}

		IEnumerator CoroutLoadNextScene() 
		{
			while(!bannerIsShowed && !interstitialStartIsShowed)
			{
				yield return new WaitForSeconds(1f);
			}

			LoadNextScene();
		}



		public void SetNoAdsPuschased()
		{

			PlayerPrefs.SetInt("AA NO ADS", 1);
			PlayerPrefs.Save();
			HideBanner();
			DestroyBanner();
		}

		public bool isNoAds
		{
			get
			{
				return PlayerPrefs.GetInt("AA NO ADS", 0) == 1;
			}
		}

		public void ShowBanner()
		{
			if(isNoAds)
				return;

			#if UNITY_EDITOR
			Debug.LogWarning("There are no Banners on the Unity Editor ! Please build your project on a mobile device !");
			#endif

			if(listBanners != null && listBanners.Count > 0)
			{
				if(randomize)
					listBanners.Shuffle();

				List<IBanner> listTemp = new List<IBanner>();

				foreach(var it in listBanners)
				{
					listTemp.Add(it);
				}

				if(randomize)
				{
					listTemp.Shuffle();
				}

				_ShowBanner (listTemp);
			}

			//			if (banner == null) {
			//				Debug.LogWarning ("There is no Banner set, please select a Banner network");
			//				return;
			//			}
			//
			//			banner.ShowBanner();

		}

		public void DestroyBanner()
		{
			#if UNITY_EDITOR
			Debug.LogWarning("There are no Banners on the Unity Editor ! Please build your project on a mobile device !");
			#endif

			//			if (banner == null) {
			//				Debug.LogWarning ("There is no Banner set, please select a Banner network");
			//				return;
			//			}
			//
			//			banner.DestroyBanner();

			if (currentBanner == null)
				return;

			currentBanner.DestroyBanner ();
			currentBanner = null;
		}

		public void HideBanner()
		{
			#if UNITY_EDITOR
			Debug.LogWarning("There are no Banners on the Unity Editor ! Please build your project on a mobile device !");
			#endif



			//			if (banner == null) {
			//				Debug.LogWarning ("There is no Banner set, please select a Banner network");
			//				return;
			//			}
			//
			//			banner.HideBanner();

			if (currentBanner == null)
				return;

			currentBanner.HideBanner ();

		}

		public void ShowInterstitialStartup()
		{
			if(isNoAds)
				return;

			if(!this.showInterstitialFirstRun)
			{
				bool isFirstRun = PlayerPrefs.GetInt("IS_IT_FIRST_RUN", 0) == 0;

				PlayerPrefs.SetInt("IS_IT_FIRST_RUN", 1);
				PlayerPrefs.Save();

				if(isFirstRun)
					return;
			}

			PlayerPrefs.SetInt("IS_IT_FIRST_RUN", 1);
			PlayerPrefs.Save();

			Randomize();

			if(listInterstitials != null && listInterstitials.Count > 0)
			{

				if(this.showOnlyFirstNetworkAtRun)
				{
					//					listTemp.Add(FIRST_interstitialNetwork);
					FIRST_interstitialNetwork.ShowInterstitialStartup(null);

					return;
				}
				else
				{
					List<IInterstitial> listTemp = new List<IInterstitial>();

					foreach(var it in listInterstitials)
					{
						listTemp.Add(it);
					}

					listTemp.Shuffle();

					_ShowInterstitialStartup(listTemp);

				}

			}
		}

		void _ShowInterstitialStartup(List<IInterstitial> listTemp)
		{
			if(isNoAds)
				return;

			var i = listTemp[0];
			listTemp.RemoveAt(0);

			print("@@@@@@@@@@@@@@@@ trying ShowInterstitialStartup for : " + i.GetType().ToString());

			i.ShowInterstitialStartup((bool success) => {
				if(success)
				{
					print("@@@@@@@@@@@@@@@@ success ShowInterstitialStartup for : " + i.GetType().ToString());
				}
				else
				{
					print("@@@@@@@@@@@@@@@@ fail ShowInterstitialStartup for : " + i.GetType().ToString());

					if(listTemp != null && listTemp.Count > 0)
					{
						_ShowInterstitialStartup(listTemp);
					}
				}
			});
		}

		public void ShowInterstitial()
		{
			if(isNoAds)
				return;

			#if UNITY_EDITOR
			Debug.LogWarning("There are no Interstitials on the Unity Editor ! Please build your project on a mobile device !");
			#endif

			if(listInterstitials != null && listInterstitials.Count > 0)
			{
				if(randomize)
					listInterstitials.Shuffle();

				List<IInterstitial> listTemp = new List<IInterstitial>();

				foreach(var it in listInterstitials)
				{
					listTemp.Add(it);
				}

				if(randomize)
				{
					listTemp.Shuffle();
				}

				_ShowInterstitial(listTemp);
			}
		}



		void _ShowInterstitial(List<IInterstitial> listTemp)
		{
			if(isNoAds)
				return;

			print("###### _ShowInterstitial : " + listTemp.GetType().ToString());

			var i = listTemp[0];
			listTemp.RemoveAt(0);

			print("trying ShowInterstitial for : " + i.GetType().ToString());

			i.ShowInterstitial((bool success) => {
				if(success)
				{
					print("success ShowInterstitial for : " + i.GetType().ToString());
				}
				else
				{
					print("fail ShowInterstitial for : " + i.GetType().ToString());

					if(listTemp != null && listTemp.Count > 0)
					{
						_ShowInterstitial(listTemp);
					}
				}
			});
		}

		void _ShowBanner(List<IBanner> listTemp) 
		{
			if (isNoAds)
				return;


			print("###### _ShowBanner : " + listTemp.GetType().ToString());

			var i = listTemp[0];
			listTemp.RemoveAt(0);

			print("trying ShowBanner for : " + i.GetType().ToString());

			i.ShowBanner ();
			currentBanner = i;

			// il faudrait savoir si on a réussi à afficher la banière.
			// Si non essayer d'afficher une autre banière

		}

		void CacheAllInterstitial()
		{
			if(isNoAds)
				return;

			if(listInterstitials != null && listInterstitials.Count > 0)
			{
				foreach(var itt in listInterstitials)
				{
					itt.CacheInterstitial();
				}
			}
		}

		void CacheAllInterstitialStartup()
		{
			if(isNoAds)
				return;

			if(listInterstitials != null && listInterstitials.Count > 0)
			{
				foreach(var itt in listInterstitials)
				{
					itt.CacheInterstitialStartup();
				}
			}
		}

		public bool IsReadyInterstitial()
		{
			if(isNoAds)
				return false;

			bool isReady = false;

			if(listInterstitials != null && listInterstitials.Count > 0)
			{
				foreach(var itt in listInterstitials)
				{
					if(itt.IsReadyInterstitial())
					{
						isReady = true;
					}
					//					else
					//					{
					//						itt.CacheInterstitial();
					//					}
				}
			}

			return isReady;
		}

		public void ShowVideoAds()
		{
			if(isNoAds)
				return;

			#if UNITY_EDITOR
			Debug.LogWarning("There are no Video Ads on the Unity Editor ! Please build your project on a mobile device !");
			#endif

			if(listVideos != null && listVideos.Count > 0)
			{
				Randomize();

				List<IVideoAds> listTemp = new List<IVideoAds>();

				foreach(var it in listVideos)
				{
					listTemp.Add(it);
				}

				if(randomize)
				{
					listTemp.Shuffle();
				}

				int rand = UnityEngine.Random.Range(0,listTemp.Count);
				var i = listTemp[rand];

				if(!i.IsReadyVideoAds())
				{
					i.CacheVideoAds();

					listTemp.RemoveAt(rand);

					foreach(var itt in listTemp)
					{
						if(itt.IsReadyVideoAds())
						{
							itt.ShowVideoAds();
							return;
						}
						else
						{
							itt.CacheVideoAds();
						}
					}
				}
				else
				{
					i.ShowVideoAds();
					return;
				}
			}
		}

		void CacheAllVideoAds()
		{
			if(isNoAds)
				return;

			if(listVideos != null && listVideos.Count > 0)
			{
				foreach(var itt in listVideos)
				{

					itt.CacheVideoAds();
				}
			}
		}

		public bool IsReadyVideoAds()
		{
			if(isNoAds)
				return false;

			bool isReady = false;

			if(listVideos != null && listVideos.Count > 0)
			{
				foreach(var itt in listVideos)
				{
					if(itt.IsReadyVideoAds())
					{
						isReady = true;
					}
					//					else
					//					{
					//						itt.CacheVideoAds();
					//					}
				}
			}

			return isReady;
		}

		public void ShowRewardedVideo(Action<bool> success)
		{
			#if UNITY_EDITOR
			Debug.LogWarning("There are no Rewarded Videos on the Unity Editor ! Please build your project on a mobile device !");
			#endif

			Debug.Log (listRewardedVideos.Count);
			if(listRewardedVideos != null && listRewardedVideos.Count > 0)
			{
				Randomize();


				List<IRewardedVideo> listTemp = new List<IRewardedVideo>();

				foreach(var it in listRewardedVideos)
				{
					listTemp.Add(it);
				}

				listTemp.Shuffle();

				int rand = UnityEngine.Random.Range(0,listTemp.Count);
				var i = listTemp[rand];

				if(!i.IsReadyRewardedVideo())
				{

					print("AdsManager - rewarded video not ready for " + i.GetType().ToString() + " ==> let's cache it");

					i.CacheRewardedVideo();

					listTemp.RemoveAt(rand);

					foreach(var itt in listTemp)
					{
						if(itt.IsReadyRewardedVideo())
						{
							print("AdsManager - (2nd chance) rewarded video is ready for " + itt.GetType().ToString() + " ==> let's show it");


							itt.ShowRewardedVideo(success);
							return;
						}
						else
						{
							print("AdsManager - (2nd chance) rewarded video not ready for " + itt.GetType().ToString() + " ==> let's cache it");


							itt.CacheRewardedVideo();
						}
					}
				}
				else
				{
					print("AdsManager - rewarded video is ready for " + i.GetType().ToString() + " ==> let's show it");

					i.ShowRewardedVideo(success);
				}
			}

			if(success != null)
			{
				success(false);
			}
		}

		void CacheAllRewardedVideo()
		{
			if(listRewardedVideos != null && listRewardedVideos.Count > 0)
			{
				foreach(var itt in listRewardedVideos)
				{
					itt.CacheRewardedVideo();
				}
			}
		}

		public bool IsReadyRewardedVideo()
		{
			bool isReady = false;

			if(listRewardedVideos != null && listRewardedVideos.Count > 0)
			{
				foreach(var itt in listRewardedVideos)
				{
					if(itt.IsReadyRewardedVideo())
					{
						isReady = true;
					}
					else
					{
						itt.CacheRewardedVideo();
					}
				}
			}

			return isReady;
		}
	}
}