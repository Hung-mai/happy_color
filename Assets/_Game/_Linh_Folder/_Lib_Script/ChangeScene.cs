#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor {

    [MenuItem("Open Scene/Loading #1")]
    public static void OpenLoading()
    {
        OpenScene("Loading");
    }

    [MenuItem("Open Scene/Home #2")]
    public static void OpenHome()
    {
        OpenScene("Home");
    }
    
    [MenuItem("Open Scene/MainScene #3")]
    public static void OpenGame()
    {
        OpenScene("MainScene");
    }

    [MenuItem("Open Scene/BonusScene #4")]
    public static void OpenBonus()
    {
        OpenScene("BonusScene");
    }

    [MenuItem("Open Scene/LevelBonus #5")]
    public static void OpenLevelBonus()
    {
        OpenScene("LevelBonus");
    }

    private static void OpenScene (string sceneName) {
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
			EditorSceneManager.OpenScene ("Assets/_Game/Scenes/" + sceneName + ".unity");
		}
	}
}
#endif