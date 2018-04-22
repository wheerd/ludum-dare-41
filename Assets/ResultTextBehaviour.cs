using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTextBehaviour : MonoBehaviour {

	public Sprite[] SuccessSprites;

	public Sprite[] FailSprites;

	public bool IsSuccess;

	public float LifeTime = 1;

	public float Speed = 1;

	SpriteRenderer _renderer;

	void Start () {
		_renderer = GetComponent<SpriteRenderer> ();

		var sprites = IsSuccess ? SuccessSprites : FailSprites;
		_renderer.sprite = sprites [Random.Range (0, sprites.Length)];
	}

	void FixedUpdate () {
		var alpha = _renderer.color.a - Time.fixedDeltaTime / LifeTime;

		transform.position += new Vector3 (0, IsSuccess ? Speed : -Speed, 0) * Time.fixedDeltaTime;
		_renderer.color = new Color (1, 1, 1, alpha);

		if (alpha <= 0) {
			Destroy (gameObject);
		}
	}
}
