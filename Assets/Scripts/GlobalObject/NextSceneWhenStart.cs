using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneWhenStart : MonoBehaviour {

    public string sceneName;

	// Use this for initialization
	void Start ()
    {
        SceneManager.LoadScene(sceneName);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
