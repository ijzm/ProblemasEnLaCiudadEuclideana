
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Util : MonoBehaviour {
	public static void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
}