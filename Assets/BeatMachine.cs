﻿using System;
using System.Collections;
using System.Collections.Generic;
using BeepBox;
using UnityEngine;
using System.Linq;

public class BeatMachine : MonoBehaviour
{
	public TextAsset beatsFile;

	public GameObject noteBarPrefab;

	public GameObject buildingPrefab;

	public GameObject panel;

	public GameObject winText;

	public GameObject lostText;

	public int beatsToShow = 8;

	public float neededPercent = 0.5f;

	public float TimeToWait = 2f;

	private double _secPerBeat;
	private double _startDspTime;
	private double _currentTimeOffset;
	public double currentBeatPosition;
	private BeepBoxFile _beats;

	private Dictionary<int,int> _pitches;

	internal Dictionary<int,int> pitchIndex;

	private SimpleNote[] _notes;

	private int _noteIndex = 0;

	private NoteBarBehaviour[] _noteBars;

	float _panelHeight;

	int _totalBuildingCount;

	int _completedBuildingCount;

	float _songLengthInBeats;

	void Start ()
	{
		_beats = JsonUtility.FromJson<BeepBoxFile> (beatsFile.text);

		_notes = GetNotesFromChannel (_beats.channels [1]);
		_pitches = GetPitches (_beats.channels [1]);

		_totalBuildingCount = _pitches.Values.Sum ();

		pitchIndex = GetPitchIndex (_pitches);

		_secPerBeat = 60d / _beats.beatsPerMinute;
		_songLengthInBeats = _beats.loopBars * _beats.beatsPerBar;

		_noteBars = new NoteBarBehaviour[_pitches.Count];
		_panelHeight = 0;
		for (var i = 0; i < _pitches.Count; i++) {
			var obj = Instantiate (noteBarPrefab, panel.transform);

			var trans = obj.GetComponent<RectTransform> ();
			trans.anchoredPosition = new Vector2 (trans.anchoredPosition.x, -_panelHeight);

			_panelHeight += trans.rect.height;

			_noteBars [i] = obj.GetComponent<NoteBarBehaviour> ();
			_noteBars [i].SetBuildingType (i);
		}

		panel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, _panelHeight);

		SpawnBuildings ();

		_startDspTime = (float)AudioSettings.dspTime + TimeToWait;

		StartCoroutine (WaitAndPlaySong ());
	}

	private SimpleNote[] GetNotesFromChannel (Channel channel)
	{
		return channel.sequence.Take (_beats.loopBars).SelectMany ((s, i) => channel.patterns [s - 1].notes.Select (n => GetNote (n, i))).ToArray ();
	}

	private Dictionary<int,int> GetPitchIndex (IDictionary<int,int> pitches)
	{
		return pitches.Keys
			.OrderBy (p => p)
			.Select ((p, i) => new {p, i})
			.ToDictionary (x => x.p, x => x.i);
	}

	int GetNumberOfBuildingsForNotes (IEnumerable<Note> notes)
	{
		return (int)((notes.Sum (n => GetNoteWorth (n.Length)) * neededPercent) / 4);
	}

	private Dictionary<int,int> GetPitches (Channel channel)
	{
		return channel.sequence.Take (_beats.loopBars)
			.SelectMany (s => channel.patterns [s - 1].notes)
			.GroupBy (n => n.pitches [0])
			.Select (g => new { g.Key, Count = GetNumberOfBuildingsForNotes (g) })
			.Where (g => g.Count > 0)
			.ToDictionary (g => g.Key, g => g.Count);
	}

	private SimpleNote GetNote (Note note, int offset)
	{
		var noteOffset = (note.StartTick + offset * _beats.ticksPerBeat * _beats.beatsPerBar) / (double)_beats.ticksPerBeat;
		var length = note.EndTick - note.StartTick;

		return new SimpleNote {
			pitch = note.pitches [0],
			offset = noteOffset,
			length = length / (double)_beats.ticksPerBeat,
			worth = GetNoteWorth (length)
		};
	}

	private static int GetNoteWorth (int length)
	{
		return 1;
		switch (length) {
		case 1:
			return 4;
		case 2:
			return 3;
		case 3:
		case 4:
			return 2;
		default:
			return 1;
		}
	}

	void SpawnBuildings ()
	{
		var camera = Camera.main.GetComponent<Camera> ();
		var panelBottom = camera.ScreenToWorldPoint (new Vector3 (0, Screen.height / 2 - _panelHeight, 0)).y;
		var screenAspect = (float)Screen.width / (float)Screen.height;
		var cameraHeight = camera.orthographicSize * 2;
		var bounds = new Bounds (camera.transform.position, new Vector3 (cameraHeight * screenAspect, cameraHeight, 0));

		bounds.max = new Vector3 (bounds.max.x, bounds.max.y + panelBottom, 0);
		bounds.Expand (-0.5f);

		foreach (var kv in _pitches) {
			var pitch = pitchIndex [kv.Key];
			for (var i = 0; i < kv.Value; i++) {
				var position = new Vector3 (UnityEngine.Random.Range (bounds.min.x, bounds.max.x), UnityEngine.Random.Range (bounds.min.y, bounds.max.y), 0);
				var building = Instantiate (buildingPrefab, position, Quaternion.identity);

				var behaviour = building.GetComponent<BuildingBehaviour> ();
				behaviour.SetBuildingType (pitch);
				behaviour.SetPoints (0);
				behaviour.SetMaxPoints (4);

				behaviour.OnHit = _noteBars [pitch].Hit;
				behaviour.OnComplete += OnBuildingComplete;
			}
		}
	}

	void Update ()
	{
		_currentTimeOffset = AudioSettings.dspTime - _startDspTime;
		currentBeatPosition = _currentTimeOffset / _secPerBeat;

		while (_noteIndex < _notes.Length && _notes [_noteIndex].offset < currentBeatPosition + beatsToShow) {
			var note = _notes [_noteIndex];
			int index;
			if (pitchIndex.TryGetValue (note.pitch, out index)) {
				_noteBars [index].AddNote ((float)note.offset, (float)note.length, note.worth);
			}

			_noteIndex++;
		}

		if (currentBeatPosition > _songLengthInBeats) {
			lostText.SetActive (true);
			gameObject.SetActive (false);
		}
	}

	IEnumerator WaitAndPlaySong ()
	{
		yield return new WaitForSecondsRealtime (TimeToWait);
		_startDspTime = (float)AudioSettings.dspTime;

		GetComponent<AudioSource> ().Play ();
	}

	public void OnBuildingComplete() {
		if (++_completedBuildingCount == _totalBuildingCount) {
			winText.SetActive (true);
			gameObject.SetActive (false);
		}
	}
}

class SimpleNote
{
	public int pitch;
	public double offset;
	public double length;
	public int worth;
}