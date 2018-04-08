namespace GS
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(FBSetup))]
    public class FBSetupEditor : Editor
    {
        private FBSetup instance;
        public override void OnInspectorGUI()
        {
            instance = (FBSetup)target;
            EditorGUILayout.Space();
            CenterTitle("Settings For Facebook");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            instance.InviteFriendsCount = EditorGUILayout.IntField("Total Friends To Invite", instance.InviteFriendsCount);
            EditorGUILayout.Space();
            instance.shareDialogTitle = EditorGUILayout.TextField("Share Dialog Title", instance.shareDialogTitle);
            instance.shareDialogMsg = EditorGUILayout.TextField("Share Dialog Message", instance.shareDialogMsg);
            instance.fbShareURI = EditorGUILayout.TextField("URL For FB Share", instance.fbShareURI);
            instance.fbSharePicURI = EditorGUILayout.TextField("Pic URL For FB Share", instance.fbSharePicURI);

            EditorGUILayout.Space();
            instance.inviteDialogTitle = EditorGUILayout.TextField("Invite Dialog Title", instance.inviteDialogTitle);
            instance.inviteDialogMsg = EditorGUILayout.TextField("Invite Dialog Message", instance.shareDialogMsg);
            EditorGUILayout.Space();
            instance.userCoinCount = EditorGUILayout.IntField("Default Coins", instance.userCoinCount);
            EditorGUILayout.Space();
            instance.openGraphObjURL = EditorGUILayout.TextField("OG Object URL", instance.openGraphObjURL);
            instance.achURL = EditorGUILayout.TextField("Achievement OG URL", instance.achURL);
            EditorGUILayout.Space();
            instance.paymentObjectURL = EditorGUILayout.TextField("OG Object URL", instance.paymentObjectURL);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://goo.gl/Ovqwqb");
            }
            if (GUILayout.Button("Contact"))
            {
                EditorUtility.DisplayDialog("Contact Info",
                                    "Game Slyce: info.gameslyce@gmail.com", "OK");
                string mailSubject = System.Uri.EscapeDataString("Help needed Facebook Invite and LeaderboardMadeEasy");
                string mailURL = "mailto:mailto:info.gameslyce@gmail.com" + "?subject=" + mailSubject;
                Application.OpenURL(mailURL);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Version Details"))
            {
                EditorUtility.DisplayDialog("FBPro Version",
                                     "GameSlyce Facebook Integration Pro Plugin Version is 1.1", "OK");
            }
            EditorGUILayout.EndHorizontal();
            FBSetup.DirtyEditor();
        }

        public static void CenterTitle(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void DrawLine()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUI.indentLevel++;
        }
    }
}