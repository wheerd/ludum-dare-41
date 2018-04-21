using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviour : MonoBehaviour {

	public Vector2 SpawnPosition;
	public Vector2 EndPosition;

	public float BeatOffset;
	public int Pitch;
	public float Length;

	BeatMachine _beatMachine;

	void Start () {
		_beatMachine = FindObjectOfType<BeatMachine> ();

		int index;
		if (_beatMachine.pitchIndex.TryGetValue (Pitch, out index)) {
			SpawnPosition = transform.position + 3 * Vector3.right + index * 0.5f * Vector3.up;
			EndPosition = SpawnPosition - 6 * Vector2.right;

			transform.localScale = new Vector3 (2 * Length, 1, 1);
		} else {
			Destroy (gameObject);
		}
	}

	void Update () {
		var offset = (float)(_beatMachine.beatsToShow - (BeatOffset - _beatMachine.currentBeatPosition)) / _beatMachine.beatsToShow;

		transform.position = Vector2.Lerp (SpawnPosition, EndPosition, offset);

		if (BeatOffset + Length < _beatMachine.currentBeatPosition) {
			Destroy (gameObject);
		}
	}
}
