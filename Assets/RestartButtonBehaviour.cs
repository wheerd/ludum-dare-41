using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButtonBehaviour : MonoBehaviour {
	void Start()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	void OnClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
