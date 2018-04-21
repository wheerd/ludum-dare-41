using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviour : MonoBehaviour {

	public Vector2 SpawnPosition;
	public Vector2 EndPosition;

	public float BeatOffset;
	public float Pitch;
	public float Length;

	BeatMachine _beatMachine;

	void Start () {
		_beatMachine = FindObjectOfType<BeatMachine> ();

		SpawnPosition = transform.position - 2 * Vector3.right + (Pitch - 48) * 0.5f * Vector3.up;
		EndPosition = SpawnPosition + 5 * Vector2.right;

		transform.localScale = new Vector3(2 * Length, 1, 1);
	}

	void Update () {
		var offset = (float)(_beatMachine.beatsToShow - (BeatOffset - _beatMachine.currentBeatPosition)) / _beatMachine.beatsToShow;


		transform.position = Vector2.Lerp (SpawnPosition, EndPosition, offset);

		Debug.Log (string.Format("{0} {1}", BeatOffset + Length, _beatMachine.currentBeatPosition));

		if (BeatOffset + Length < _beatMachine.currentBeatPosition) {
			Destroy (gameObject);
		}
	}
}
