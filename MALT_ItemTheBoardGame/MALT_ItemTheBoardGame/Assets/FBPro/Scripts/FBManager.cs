using UnityEngine.SceneManagement;

namespace GS
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Facebook.Unity;
	using System.Linq;
	using System.IO;
	using Facebook.MiniJSON;

	// Main Class Responsible for Every Facebook Method and Work
	public class FBManager : MonoBehaviour
	{

		public Action FacebookConnect;
		public Action<String> NameLoaded;
		public Action<String> PicURLLoaded;


		#region Initialization and Declarations
		public GameObject loginBtnPanel, allOtherBtnsPanel;
		public Sprite[] stateSprites;
		[HideInInspector]
		public ToggleState tglStateSlctAll = ToggleState.Unchecked;

		public Text playerName, playerScore, countFriends, countInstalledFriends, countCoins;
		public Image playerDp, selectAllImg;

		// List of the invite and leaderboard list items
		List<ItemInvite> listInvites = new List<ItemInvite>();
		List<ItemLeader> listLeaderboard = new List<ItemLeader>();
		List<ItemGameScore> listGameScore = new List<ItemGameScore>();
		List<ItemGameScore> listUserFriends = new List<ItemGameScore>();

		// List containers that list Items - (Dynamically Increasing ListView <Custom>)
		public Transform inviteParent, leaderParent, allGamesScoresParent, userFriendsParent;

		//Prefabs that holds items that will be places in the containers.
		public ItemInvite itemInvitePref;
		public ItemLeader itemLeaderPref;
		public ItemGameScore gameScorePref, userFriendsPrefab;

		#if FACEBOOK_PUBLISH_PERISSION
		List<string> readPermission = new List<string>() { "public_profile", "user_friends", "user_games_activity" },
		publishPermission = new List<string>() { "publish_actions" };
		#else


		List<string> readPermission = new List<string> () {
			"public_profile",
			"user_friends"
			//			,
			//			"user_games_activity"
		};
		//		publishPermission = new List<string>() { "publish_actions" };
		#endif

		//strings that let you get JSON from the Facebook API calls.
		const string getLeaderboardString = "app/scores",

		getUserPicString = "me?fields=picture.height(256)",
		getNameString = "me?fields=first_name",
		getAllScoresString = "me/scores?fields=application,score",
		getFriendsInfoString = "me/friends";
		public static string appID;

		public Text logTxt;
		delegate void LoadPictureCallback(Texture2D texture, int index);

		public GameObject[] dialogs;
		public GameObject[] loaders;

		public InputField inpInvSearcher, inpLeadSearcher, inpSubmitScore, inpPostGraph;

		void Awake()
		{
			//SetFBItems(false);
			loginBtnPanel.SetActive (false);

			bool hasBeenLoggedOnce = PlayerPrefs.GetInt("FACEBOOK_LOGGED_ONCE", 0) == 1;

			#if UNITY_EDITOR
			hasBeenLoggedOnce = false;
			#endif
			//hasBeenLoggedOnce = false;

			if(hasBeenLoggedOnce) {
				loginBtnPanel.SetActive (false);
			} else {
				loginBtnPanel.SetActive (true);
			}


			InitFB ();

			return;
			if (FB.IsLoggedIn) {
				LoadPlayerName ();
				LoadPlayerPic ();
				SceneManager.LoadScene (1);
			}

		}

		public void SubmitScore()
		{
			ShowHideDialog(2, true);
			inpSubmitScore.text = "";
		}

		void ShowHideDialog(int dialogID, bool state)
		{
			dialogs[dialogID].SetActive(state);
		}

		void ShowHideLoader(int loaderId, bool state)
		{
			loaders[loaderId].SetActive(state);
		}
		//        public void print(string msg)
		//        {
		//            if (msg.Length > 3000)
		//            {
		//                print(msg);
		//                logTxt.text = "Huge Data, can't be Displayed in Log View! Please see Console!";
		//            }
		//            else
		//            {
		//                print(msg);
		//                logTxt.text = msg;
		//            }
		//        }


		public void DisplayInvitePanel(bool isShown) {
			ShowHideDialog (0, isShown);

		}

		#endregion

		#region Get and Post Current User Score- User's All games' Score
		public void PostScore()
		{
			#if FACEBOOK_PUBLISH_PERISSION
			/*
			If you don't have facebook publish permission already, Ask for it
			Note! this is not going to work if your publish_actions permission is not approved by facebook.
			Each time you'll post score, It'll prompt user to grant publish_actions unless your app is 
			approved by facebook for publish actions.
			*/
			if (!AccessToken.CurrentAccessToken.Permissions.Contains(publishPermission[0]))
			{
			// As A good Practice, You should tell your users that why you need publish permission so
			// show a dialog telling about it. or else simply go to facebook permission prompt.
			//sm.publish_permissionDialog.SetActive(true);
			GetPublishPermission();
			}
			else
			{
			PostOnlyIfPermitted();
			}
			#endif
		}

		void PostOnlyIfPermitted()
		{
			var scoreData = new Dictionary<string, string>() { { "score", inpSubmitScore.text } };
			FB.API("/me/scores", HttpMethod.POST, delegate (IGraphResult r)
				{
					print("Result: " + r.RawResult);
				}, scoreData);
		}
		public void GetPublishPermission()
		{
			#if FACEBOOK_PUBLISH_PERISSION

			FB.LogInWithPublishPermissions(publishPermission,
			delegate (ILoginResult loginResult)
			{
			if (AccessToken.CurrentAccessToken.Permissions.Contains(publishPermission[0]))
			{
			if (string.IsNullOrEmpty(loginResult.Error) && !loginResult.Cancelled)
			{
			PostOnlyIfPermitted();
			}
			}
			else
			{
			print("No publish_actions! permission");
			}

			});

			#endif

		}

		public void GetAppIDNScore()
		{
			if (string.IsNullOrEmpty(appID))
			{
				FB.API("app", HttpMethod.GET, delegate (IGraphResult result)
					{
						if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
						{
							appID = result.ResultDictionary["id"] as string;
							print("Current App Id is " + appID + "Now getting Score !");
							GetFBScoreInternal();
						}
						else
						{
							print("Failed to Get Current App ID! You can try again!");
						}
					});
			}
			else
			{
				GetFBScoreInternal();
			}

		}

		void GetFBScoreInternal()
		{
			FB.API(getAllScoresString, HttpMethod.GET, delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{
						List<object> data = result.ResultDictionary["data"] as List<object>;

						for (int i = 0; i < data.Count; i++)
						{
							Dictionary<string, object> appData = ((Dictionary<string, object>)data[i])["application"]
								as Dictionary<string, object>;
							string gameId = Convert.ToString(appData["id"]);

							if (gameId == appID)
							{
								string score = Convert.ToString(((Dictionary<string, object>)data[i])["score"]);
								playerScore.text = score;
								print(string.Format("Current Player Score is {0} For Current AppID {1}", score, appID));
								break;
							}
						}
					}
					else
					{
						print("Failed to Get Current App Score! You can try again!");
					}
				});
		}

		public void GetAllScores()
		{
			ShowHideDialog(3, true);
			ClearGameScores();
			FB.API(getAllScoresString, HttpMethod.GET, delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{
						List<object> data = result.ResultDictionary["data"] as List<object>;

						for (int i = 0; i < data.Count; i++)
						{

							string score = Convert.ToString(((Dictionary<string, object>)data[i])["score"]);
							Dictionary<string, object> appData = ((Dictionary<string, object>)data[i])["application"] as Dictionary<string, object>;

							string gameName = Convert.ToString(appData["name"]);

							ItemGameScore tempItem = Instantiate(gameScorePref, allGamesScoresParent, false) as ItemGameScore;
							tempItem.AssignValues(gameName, score);
							listGameScore.Add(tempItem);
						}
					}
					ShowHideLoader(2, false);
				});

		}

		#endregion

		#region Get Friends Info
		public void RequestFriendsData()
		{
			//ShowHideDialog(4, true);
			ClearUserFriendsData();
			FB.API(getFriendsInfoString, HttpMethod.GET, delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{
						List<object> data = result.ResultDictionary["data"] as List<object>;

						for (int i = 0; i < data.Count; i++)
						{
							string fName = Convert.ToString(((Dictionary<string, object>)data[i])["name"]);
							string fId = Convert.ToString(((Dictionary<string, object>)data[i])["id"]);

							ItemGameScore tempItem = Instantiate(userFriendsPrefab, userFriendsParent, false) as ItemGameScore;
							tempItem.AssignValues(fName, fId);
							listUserFriends.Add(tempItem);
						}
						countInstalledFriends.text = data.Count.ToString();
						IDictionary summary = result.ResultDictionary["summary"] as IDictionary;
						countFriends.text = summary["total_count"].ToString();

						print(string.Format("Friends Who Installed {0}, Total Friends", data.Count, countFriends.text));
					}
					else
					{
						print("Failed to Get Current App Score! You can try again!");
					}

					ShowHideLoader(3, false);
				});
		}
		#endregion

		#region Leaderboard
		//Method to load leaderboard
		public void LoadLeaderboard()
		{
			ShowHideDialog(1, true);
			ClearLeaderboard();
			FB.API(getLeaderboardString, HttpMethod.GET, CallBackLoadLeaderboard);
		}
		//callback of from Facebook API when the leaderboard data from the server is loaded.
		void CallBackLoadLeaderboard(IGraphResult result)
		{
			if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
			{
				//Dictionary<string, object> JSON = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

				List<object> data = result.ResultDictionary["data"] as List<object>;//JSON["data"] as List<object>;
				for (int i = 0; i < data.Count; i++)
				{
					string fScore;
					try
					{
						fScore = Convert.ToString(((Dictionary<string, object>)data[i])["score"]);
					}
					catch (Exception)
					{
						fScore = "0";
					}
					Dictionary<string, object> UserInfo = ((Dictionary<string, object>)data[i])["user"] as Dictionary<string, object>;
					string name = Convert.ToString(UserInfo["name"]);
					string id = Convert.ToString(UserInfo["id"]);
					CreateListItemLeaderboard(id, name, fScore, i + 1);
					LoadFriendsAvatar(i);
				}
			}

			ShowHideLoader(1, false);
			print(result.RawResult);
			inpLeadSearcher.text = "";
		}

		public void OnValueChangeLeaderSeacher()
		{
			string friendsName = inpLeadSearcher.text;

			print("Friend's Name is " + friendsName);
			if (!string.IsNullOrEmpty(friendsName))
			{
				foreach (var item in listLeaderboard)
				{
					string currName = item.txtName.text;

					if (friendsName.Length <= currName.Length &&
						string.Equals(friendsName, currName.Substring(0, friendsName.Length), StringComparison.OrdinalIgnoreCase))
					{
						item.SetObjectState(true);
					}
					else
					{
						item.SetObjectState(false);
					}
				}
			}
			else
			{
				print("No query It's blank !");
				foreach (var item in listLeaderboard)
				{
					item.SetObjectState(true);
				}
			}
		}
		// Method to load Friends Profile Pictures
		void LoadFriendsAvatar(int index)
		{
			FB.API(GetPictureURL(listLeaderboard[index].fId), HttpMethod.GET, result =>
				{
					if (result.Error != null)
					{
						print(result.Error);
						return;
					}
					listLeaderboard[index].picUrl = DeserializePictureURLString(result.RawResult);
					StartCoroutine(LoadFPicRoutine(listLeaderboard[index].picUrl, PicCallBackLeaderboard, index));
				});
		}

		//Method to all items to the leaderboard dynamically scrollable list
		void CreateListItemLeaderboard(string id, string fName, string fScore = "", int rank = 0)
		{
			ItemLeader tempItem = Instantiate(itemLeaderPref, leaderParent, false) as ItemLeader;

			tempItem.AssignValues(id, fName, fScore, rank.ToString());
			listLeaderboard.Add(tempItem);
		}

		private void PicCallBackLeaderboard(Texture2D texture, int index)
		{
			if (texture == null)
			{
				StartCoroutine(LoadFPicRoutine(listLeaderboard[index].picUrl, PicCallBackLeaderboard, index));
				return;
			}
			Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			listLeaderboard[index].imgPic.sprite = sp;
		}

		//Coroutine to load Picture from the specified URL
		IEnumerator LoadFPicRoutine(string url, LoadPictureCallback Callback, int index)
		{
			WWW www = new WWW(url);
			yield return www;
			Callback(www.texture, index);
		}
		#endregion

		#region Custom and Native Invite
		// Method that Proceeds with the Invitable Friends
		//Click Handler of Select All Buttons
		public void TglSelectAllClickHandler()
		{
			switch (tglStateSlctAll)
			{
			case ToggleState.Partial:
			case ToggleState.Unchecked:
				foreach (var item in listInvites)
				{
					item.tglBtn.isOn = true;
				}
				tglStateSlctAll = ToggleState.Checked;
				ChangeToggleState(ToggleState.Checked);
				break;
			case ToggleState.Checked:
				foreach (var item in listInvites)
				{
					item.tglBtn.isOn = false;
				}
				ChangeToggleState(ToggleState.Unchecked);
				break;
			}
		}
		//Method to change Toggle State On the Fly
		public void ChangeToggleState(ToggleState state)
		{
			switch (state)
			{
			case ToggleState.Unchecked:
				tglStateSlctAll = state;
				selectAllImg.sprite = stateSprites[0];
				break;
			case ToggleState.Partial:
				bool flagOn = false, flagOff = false;
				foreach (var item in listInvites)
				{
					if (item.tglBtn.isOn)
					{
						flagOn = true;
					}
					else
					{
						flagOff = true;
					}
				}
				if (flagOn && flagOff)
				{
					tglStateSlctAll = state;
					selectAllImg.sprite = stateSprites[1];
					//Debug.Log("Partial");
				}
				else if (flagOn && !flagOff)
				{
					ChangeToggleState(ToggleState.Checked);
					//Debug.Log("Checked");
				}
				else if (!flagOn && flagOff)
				{
					ChangeToggleState(ToggleState.Unchecked);
					//Debug.Log("Unchecked");
				}
				break;
			case ToggleState.Checked:
				tglStateSlctAll = state;
				selectAllImg.sprite = stateSprites[2];
				break;
			}
		}

		//ClickHandling Method that Sends Backend Facebofok Native App request (Invitable)Calls
		public void SendInvites()
		{
			List<string> lstToSend = new List<string>();
			foreach (var item in listInvites)
			{
				if (item.tglBtn.isOn)
				{
					lstToSend.Add(item.fId);
				}
			}
			int dialogCount = (int)Mathf.Ceil(lstToSend.Count / 50f);
			CallInvites(lstToSend, dialogCount);
		}
		//Helping method that will be recursive if you'll have to sent invites to more than 50 Friends.
		private void CallInvites(List<string> lstToSend, int dialogCount)
		{
			if (dialogCount > 0)
			{
				string[] invToSend = (lstToSend.Count >= 50) ? new string[50] : new string[lstToSend.Count];

				for (int i = 0; i < invToSend.Length; i++)
				{
					try
					{
						if (lstToSend[i] != null)
						{
							invToSend[i] = lstToSend[i];
						}
					}
					catch (Exception e)
					{
						Debug.Log(e.Message);
					}
				}
				lstToSend.RemoveRange(0, invToSend.Length);
				FB.AppRequest(
					FBSetup.Instance.inviteDialogMsg, invToSend, null, null, null, null, FBSetup.Instance.inviteDialogTitle,
					callback: delegate (IAppRequestResult result)
					{
						if (--dialogCount > 0)
						{
							CallInvites(lstToSend, dialogCount);
						}
					}
				);
			}
		}
		public void LoadInvitableFriends()
		{
			ShowHideDialog(0, true);
			selectAllImg.sprite = stateSprites[0];
			ClearInvite();
			string getInvitableFriendsString = "me/invitable_friends?limit=" + FBSetup.Instance.InviteFriendsCount;
			FB.API(getInvitableFriendsString, HttpMethod.GET, delegate (IGraphResult result) {
				//Deserializing JSON returned from server
				//Dictionary<string, object> JSON = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
				List<object> data = result.ResultDictionary["data"] as List<object>;//JSON["data"] as List<object>;
				//Loop to traverse and process all the items returned from the server.

				for (int i = 0; i < data.Count; i++)
				{
					string id = Convert.ToString(((Dictionary<string, object>)data[i])["id"]);
					string name = Convert.ToString(((Dictionary<string, object>)data[i])["name"]);
					Dictionary<string, object> picInfo = ((Dictionary<string, object>)data[i])["picture"] as Dictionary<string, object>;
					string url = DeserializePictureURLObject(picInfo);
					CreateListItemInvite(id, name, url);
					StartCoroutine(LoadFPicRoutine(url, PicCallBackInvitable, i));
				}
				print(result.RawResult);
				ShowHideLoader(0, false);
				inpInvSearcher.text = "";
			});
		}
		//Method to add item to the custom invitable dynamically scrollable list
		void CreateListItemInvite(string id, string fName, string url = "")
		{
			ItemInvite tempItem = Instantiate(itemInvitePref, inviteParent, false) as ItemInvite;
			tempItem.AssignValues(id, url, fName);
			listInvites.Add(tempItem);
		}
		//Callback of Invitable Friend API call
		void PicCallBackInvitable(Texture2D texture, int index)
		{
			if (texture == null)
			{
				StartCoroutine(LoadFPicRoutine(listInvites[index].picUrl, PicCallBackInvitable, index));
				return;
			}
			listInvites[index].imgPic.sprite = Sprite.Create(texture,
				new Rect(0, 0, texture.width, texture.height),
				new Vector2(0.5f, 0.5f)
			);
		}
		// Native Invite!
		public void NativeInviteFriendsFB()
		{
			FB.AppRequest(
				FBSetup.Instance.inviteDialogMsg, null, null, null, null, null, FBSetup.Instance.inviteDialogTitle,
				callback: delegate (IAppRequestResult result)
				{
					print(result.RawResult);
				});
		}

		public void OnValueChangeInvSearcher()
		{
			string friendsName = inpInvSearcher.text;

			print("Friend's Name is " + friendsName);
			if (friendsName != null && friendsName.Length != 0)
			{
				//print("There is a query");
				foreach (var item in listInvites)
				{
					string currName = item.txtName.text;

					//print("This Item Name is " + currName);
					if (friendsName.Length <= currName.Length &&
						string.Equals(friendsName, currName.Substring(0, friendsName.Length), StringComparison.OrdinalIgnoreCase))
					{
						item.SetObjectState(true);
					}
					else
					{
						item.SetObjectState(false);
					}
				}
			}
			else
			{
				print("No query It's blank !");
				foreach (var item in listInvites)
				{
					item.SetObjectState(true);
				}
			}
		}


		#endregion

		#region FB Init Login and Logout
		public void InitFB()
		{
			if (!FB.IsInitialized)
			{
				FB.Init(InitCallback, onHideUnity);
			}
			else
			{
				FB.ActivateApp();
				print("Initialized !");
				loginBtnPanel.SetActive(false);


			}
		}
		// Perform Unity Tasks When App is Connecting To Facebook 
		private void onHideUnity(bool isGameShown)
		{
			if (!isGameShown)
			{
				// Pause the game - we will need to hide
				Time.timeScale = 0;
			}
			else
			{
				// Resume the game - we're getting focus again
				Time.timeScale = 1;
			}
		}
		// Method that will Get called After Facebook Initialization Method Call!
		private void InitCallback()
		{
			if (FB.IsInitialized)
			{
				loginBtnPanel.SetActive(false);

				LoginFB();
				print("Initialized !");
			}
			else
			{
				print("Failed to Initialize the Facebook SDK!");
				//InitFB();//Try Again!
			}
		}

		void LoginFB()
		{
			if (FB.IsLoggedIn)
			{
				PlayerPrefs.SetInt("FACEBOOK_LOGGED_ONCE", 1);
				PlayerPrefs.Save();

				//SetFBItems(true);
				print("Logged In !");

				// TEST
				LoadPlayerName();
				LoadPlayerPic(false);
				//LoadInvitableFriends ();
				SceneManager.LoadScene (1);
			}
			else
			{
				loginBtnPanel.SetActive (true);
				//SetFBItems (true);
				//OnConnect ();
				//FB.ActivateApp();
				//FB.LogInWithReadPermissions(readPermission, LoginCallback);
			}
		}

		public void OnConnect() {
			FB.ActivateApp();
			FB.LogInWithReadPermissions(readPermission, LoginCallback);
		}

		public string pName;
		public string pUrlPic;
		//Callback method of login
		void LoginCallback(ILoginResult result)
		{
			if (FB.IsLoggedIn)
			{
				// AccessToken class will have session details
				var aToken = AccessToken.CurrentAccessToken;
				foreach (string perm in aToken.Permissions)
				{
					print(perm);
				}
				print("Logged In Successfully!");
				SetFBItems(true);

				LoadPlayerName();
				LoadPlayerPic(false);
				RequestFriendsData();
				//LoadInvitableFriends ();

				//				if (FacebookConnect != null)
				//					FacebookConnect ();

				SceneManager.LoadScene (1);

				//				Invoke("LoadPlayerName",0.1f);
				//				Invoke("LoadPlayerPic",1f);
			}
			else
			{
				print("User cancelled login");
			}
		}

		public void LoadPlayerPic()
		{
			LoadPlayerPic(false);
		}

		public void LogoutFB()
		{
			FB.LogOut();
			print("Logged Out !");
			SetFBItems(false);
			ClearOldData();
		}
		void ClearOldData()
		{
			ClearLeaderboard();
			ClearInvite();
			ClearGameScores();
			ClearUserFriendsData();
		}
		void ClearInvite()
		{
			listInvites.Clear();
			ShowHideLoader(0, true);
			for (int i = 0; i < inviteParent.childCount; i++)
			{
				Destroy(inviteParent.GetChild(i).gameObject);
			}
		}
		void ClearLeaderboard()
		{
			listLeaderboard.Clear();
			ShowHideLoader(1, true);
			for (int i = 0; i < leaderParent.childCount; i++)
			{
				Destroy(leaderParent.GetChild(i).gameObject);
			}
		}
		void ClearGameScores()
		{
			listGameScore.Clear();
			ShowHideLoader(2, true);
			for (int i = 0; i < allGamesScoresParent.childCount; i++)
			{
				Destroy(allGamesScoresParent.GetChild(i).gameObject);
			}
		}
		void ClearUserFriendsData()
		{
			listUserFriends.Clear();
			ShowHideLoader(3, true);
			for (int i = 0; i < userFriendsParent.childCount; i++)
			{
				Destroy(userFriendsParent.GetChild(i).gameObject);
			}
		}
		void SetFBItems(bool isLogin)
		{
			loginBtnPanel.SetActive(!isLogin);
		}

		#endregion

		#region User FB Name, Picture - Saving for Offline Access
		#if !UNITY_WEBGL
		string FILE_NAME = "userpic.jpg";
		string GetPath(string fileName)
		{
			return Path.Combine(Application.persistentDataPath, fileName);
		}
		#endif
		public void LoadPlayerPic(bool needToSave = false)
		{
			FB.API(getUserPicString, HttpMethod.GET,
				delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{
						IDictionary picData = result.ResultDictionary["picture"] as IDictionary;
						IDictionary data = picData["data"] as IDictionary;
						string picURL = data["url"] as string;
						pUrlPic = picURL;

						if(PicURLLoaded != null)
							PicURLLoaded(pUrlPic);

						StartCoroutine(GetProfilePicRoutine(picURL, needToSave));

					}
					print(result.RawResult);
				});
		}

		public void LoadDPifExists()
		{
			#if !UNITY_WEBGL
			if (File.Exists(GetPath(FILE_NAME)))
			{
				byte[] fileData = File.ReadAllBytes(GetPath(FILE_NAME));
				Texture2D tex = new Texture2D(2, 2);
				tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
				LoadOrSavePicture(tex, true);
				print("Dp Loaded From Local Storage! Address => " + GetPath(FILE_NAME));
				print("No Storage on WebGL!");
			}
			else
			{
				print("Nothing Stored Locally Yet!");
			}
			#else
			print("Nothing Stored Locally Yet!");
			#endif
		}
		void LoadOrSavePicture(Texture2D tex, bool needToSave)
		{
			return;

			playerDp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));



			#if !UNITY_WEBGL
			if (needToSave)
			{
				byte[] bytes = tex.EncodeToJPG();
				File.WriteAllBytes(GetPath(FILE_NAME), bytes);
			}
			#endif
		}

		private IEnumerator GetProfilePicRoutine(string url, bool needToSave = false)
		{
			print("GetProfilePicRoutine");
			WWW www = new WWW(url);
			yield return www;
			LoadOrSavePicture(www.texture, needToSave);
		}
		public void LoadPlayerName()
		{
			FB.API(getNameString, HttpMethod.GET,
				delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{

						//playerName.text = result.ResultDictionary["name"] as string;

						pName = result.ResultDictionary["first_name"] as string;

						if(NameLoaded != null)
							NameLoaded(pName);

						print("Name Loaded !");
					}
					else
					{
						print("Failed! Try Again!");
					}
				});
		}
		#endregion

		#region Screenshot

		public void TakeScreenshotNShare()
		{
			StartCoroutine(TakeScreenshot());
		}
		private IEnumerator TakeScreenshot()
		{
			yield return new WaitForEndOfFrame();

			var width = Screen.width;
			var height = Screen.height;
			var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
			// Read screen contents into the texture
			tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			tex.Apply();
			byte[] screenshot = tex.EncodeToPNG();

			print("Screenshot Taken! Now Started Posting to Facebook");
			var wwwForm = new WWWForm();
			wwwForm.AddBinaryData("image", screenshot, "Screenshot.png");

			FB.API("me/photos", HttpMethod.POST,
				delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{
						print("Post Successfully !" + result.RawResult);
					}
					else
					{
						print("Error Occured ! See Details =>" + result.RawResult);
					}

				}, wwwForm);
		}

		#endregion

		#region Get DeepLink

		public void GetDeepLink()
		{
			FB.GetAppLink(
				delegate (IAppLinkResult result)
				{
					if (!string.IsNullOrEmpty(result.Url))
					{
						var index = (new Uri(result.Url)).Query.IndexOf("request_ids");
						if (index != -1)
						{
							// ...have the user interact with the friend who sent the request,
							// perhaps by showing them the gift they were given, taking them
							// to their turn in the game with that friend, etc.
						}
					}
					print(result.RawResult);
				});

		}
		#endregion

		#region Share On Facebook
		public void ShareOnFB()
		{
			if (FB.IsLoggedIn)
			{
				FB.ShareLink(
					contentURL: new Uri(FBSetup.Instance.fbShareURI),
					contentTitle: FBSetup.Instance.shareDialogTitle,
					contentDescription: FBSetup.Instance.shareDialogMsg,
					photoURL: new Uri(FBSetup.Instance.fbSharePicURI),
					callback: delegate (IShareResult result)
					{
						if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
						{
							print("Story Posted Successfully!");
						}
						else
						{
							print("Error Occured!");
						}
					}
				);
			}
		}

		#endregion

		#region Canvas Payment

		public void BuyCoins()
		{
			FBManager fbM = FindObjectOfType<FBManager>();
			// Format payment URL
			string paymentURL = FBSetup.Instance.paymentObjectURL;

			// https://developers.facebook.com/docs/unity/reference/current/FB.Canvas.Pay
			FB.Canvas.Pay(paymentURL,
				"purchaseitem",
				1,
				null, null, null, null, null,
				(IPayResult result) =>
				{
					print("PayCallback");
					if (result.Error != null)
					{
						Debug.LogError(result.Error);
						return;
					}
					print(result.RawResult);

					object payIdObj;
					if (result.ResultDictionary.TryGetValue("payment_id", out payIdObj))
					{
						string payID = payIdObj.ToString();
						print("Payment complete");
						print("Payment id:" + payID);

						FBSetup.Instance.userCoinCount += 100;
						print("Purchase Complete");
						UpdateCoins();
					}
					else
						print("Payment error");
				});
		}
		public void UpdateCoins()
		{
			countCoins.text = FBSetup.Instance.userCoinCount.ToString();
			ShowHideDialog(5, true);
		}

		#endregion

		#region Achievements

		public void PostAchievement()
		{
			var data = new Dictionary<string, string>() { { "achievement", FBSetup.Instance.achURL } };
			FB.API("me/achievements",
				HttpMethod.POST,
				delegate (IGraphResult result)
				{
					if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
					{
						print("Success! ");
					}
					else
					{
						print(result.RawResult);
					}

					FB.ShareLink(
						contentURL: new Uri(FBSetup.Instance.achURL),
						callback: delegate (IShareResult shareRes)
						{
							if (string.IsNullOrEmpty(shareRes.Error) && !shareRes.Cancelled)
								print("Achievement Posted Successfully!");
							else
								print("Posting Unsuccessful!");
						}
					);

				},
				data);
		}
		#endregion

		#region Share Graph API

		public void ShareViaDialog()
		{
			FB.ShareLink(
				contentURL: new Uri(FBSetup.Instance.openGraphObjURL),
				callback: delegate (IShareResult shareRes)
				{
					if (string.IsNullOrEmpty(shareRes.Error) && !shareRes.Cancelled)
						print("Story Posted Successfully!");
					else
						print("Posting Unsuccessful!");
				}
			);
		}
		//this Code is deprecated and so can't be used in newer versions of Facebook API
		//public void OpenShareViaGraph()
		//{
		//    ShowHideDialog(6, true);
		//}
		//public void ShareViaGraph()
		//{
		//    var data = new Dictionary<string, string>() {
		//        { "gsfbtest", FBSetup.Instance.openGraphObjURL},
		//        {"fb:explicitly_shared" , "true" },
		//        {"message", inpPostGraph.text }
		//        };
		//    FB.API("me/gametestfeatures:test",
		//            HttpMethod.POST,
		//            delegate (IGraphResult result)
		//            {
		//                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
		//                {
		//                    print("Success! Posted");
		//                }
		//                else
		//                {
		//                    print("Error!");
		//                }
		//                print(result.RawResult);
		//            },
		//            data);
		//}

		#endregion

		#region Utils
		public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
		{
			string url = string.Format("/{0}/picture", facebookID);
			string query = width != null ? "&width=" + width.ToString() : "";
			query += height != null ? "&height=" + height.ToString() : "";
			query += type != null ? "&type=" + type : "";
			query += "&redirect=false";
			if (query != "") url += ("?g" + query);
			return url;
		}

		public static string DeserializePictureURLString(string response)
		{
			return DeserializePictureURLObject(Json.Deserialize(response));
		}
		public static string DeserializePictureURLObject(object pictureObj)
		{
			var picture = (Dictionary<string, object>)(((Dictionary<string, object>)pictureObj)["data"]);
			object urlH = null;
			if (picture.TryGetValue("url", out urlH))
			{
				return (string)urlH;
			}

			return null;
		}
		#endregion
	}
	public enum ToggleState
	{
		Unchecked,
		Partial,
		Checked
	};
}

