using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteBehaviour : MonoBehaviour {

	public float BeatOffset;
	public int Pitch;
	public float Length;

	BeatMachine _beatMachine;
	float _beatWidth;
	RectTransform _rectTransform;
	float _originalWidth;
	Image _image;
	private bool _started = false;

	public delegate void NoteAction();

	public event NoteAction OnStart;
	public event NoteAction OnEnd;


	void Start () {
		_beatMachine = FindObjectOfType<BeatMachine> ();
		_image = GetComponent<Image> ();
		_rectTransform = GetComponent<RectTransform> ();
		_originalWidth = _rectTransform.rect.width;
		_beatWidth = _originalWidth / _beatMachine.beatsToShow;

		Update ();
	}

	void Update () {
		var left = (float)(BeatOffset - _beatMachine.currentBeatPosition) / _beatMachine.beatsToShow;
		var right = left + Length / _beatMachine.beatsToShow;

		_rectTransform.offsetMin = new Vector2 (left * _originalWidth, 0);
		_rectTransform.offsetMax = new Vector2 (-_originalWidth + right * _originalWidth, 0);

		_image.color = new Color(1, 1, 1, left < 0 ? 0.5f : 1f);

		if (BeatOffset < _beatMachine.currentBeatPosition && ! _started) {
			_started = true;
			OnStart ();
		}

		if (BeatOffset + Length < _beatMachine.currentBeatPosition) {
			OnEnd ();
			Destroy (gameObject);
		}
	}
}
