namespace GS
{
    using System.IO;
	#if UNITY_EDITOR
    using UnityEditor;
	#endif
    using UnityEngine;
    public class FBSetup : ScriptableObject
    {
        //This is a dummy Score
        public int userCoinCount = 100;
        public string shareDialogTitle = "Amazing Example",
            shareDialogMsg = "This is a Superb Owesome Game! Check this Out.",
            inviteDialogTitle = "Amazing Example",
            inviteDialogMsg = "Let's Play this Great Fun Game!";
        public string fbShareURI = "http://u3d.as/aRQ",
                fbSharePicURI ="http://i.imgur.com/fPs7tnx.png";


        public string invMessage = "Let's Play this Fun game!",
        invTitle = "Super Awesome Example by Game Slyce",
        openGraphObjURL = "https://www.curioerp.com/gameslyce/plugins/fbpro/sampleog.html",
        achURL = "https://www.curioerp.com/gameslyce/plugins/fbpro/achievement.html",
        paymentObjectURL = "https://www.curioerp.com/plugins/fbpro/payments/100coins.php";

        int _inviteFriendsCount = 100;

        public int InviteFriendsCount
        {
            set { _inviteFriendsCount = Mathf.Clamp(value, 1, 5000); }
            get { return _inviteFriendsCount; }
        }

        const string assetDataPath = "Assets/FBPro/Resources/";
        const string assetName = "FBSetup";
        const string assetExt = ".asset";
        private static FBSetup instance;
        public static FBSetup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(assetName) as FBSetup;
                    if (instance == null)
                    {
                        instance = CreateInstance<FBSetup>();
#if UNITY_EDITOR
                        if (!Directory.Exists(assetDataPath))
                        {
                            Directory.CreateDirectory(assetDataPath);
                        }
                        string fullPath = assetDataPath + assetName + assetExt;
                        AssetDatabase.CreateAsset(instance, fullPath);
                        AssetDatabase.SaveAssets();
#endif
                    }
                }
                return instance;
            }
        }
        public static void DirtyEditor()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }
#if UNITY_EDITOR
        [MenuItem("Facebook/**FBManager**")]
        public static void Edit()
        {
            Selection.activeObject = Instance;
        }
#endif
    }
}