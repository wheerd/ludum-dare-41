using System;
using System.Collections;
using System.Collections.Generic;
using BeepBox;
using UnityEngine;
using System.Linq;

public class BeatMachine : MonoBehaviour
{
	public TextAsset beatsFile;

	public GameObject notePrefab;

	public GameObject buildingPrefab;

	public int beatsToShow = 8;

	public float neededPercent = 0.5f;

	private double _secPerBeat;
	private double _startDspTime;
	private double _currentTimeOffset;
	public double currentBeatPosition;
	private BeepBoxFile _beats;

	private Dictionary<int,int> _pitches;

	internal Dictionary<int,int> pitchIndex;

	private SimpleNote[] _notes;

	private int _noteIndex = 0;

	void Start ()
	{
		_beats = JsonUtility.FromJson<BeepBoxFile> (beatsFile.text);

		_notes = GetNotesFromChannel (_beats.channels [1]);
		_pitches = GetPitches (_beats.channels [1]);

		pitchIndex = GetPitchIndex(_pitches);

		_secPerBeat = 60d / _beats.beatsPerMinute;
		_startDspTime = (float)AudioSettings.dspTime;

		GetComponent<AudioSource> ().Play ();

		SpawnBuildings ();
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
			.Select(g => new { g.Key, Count = GetNumberOfBuildingsForNotes(g) })
			.Where( g => g.Count > 0)
			.ToDictionary (g => g.Key, g => g.Count);
	}

	private SimpleNote GetNote (Note note, int offset)
	{
		var noteOffset = (note.StartTick + offset * _beats.ticksPerBeat * _beats.beatsPerBar) / (double)_beats.ticksPerBeat;

		return new SimpleNote {
			pitch = note.pitches [0],
			offset = noteOffset,
			length = (note.EndTick - note.StartTick) / (double)_beats.ticksPerBeat
		};
	}

	private int GetNoteWorth (int length)
	{
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

	void SpawnBuildings() {
		foreach (var kv in _pitches) {
			var pitch = pitchIndex [kv.Key];
			for (var i = 0; i < kv.Value; i++) {
				Debug.Log ("Creating with pitch " + kv.Key);
				var position = new Vector3 (UnityEngine.Random.Range (-5, 5), UnityEngine.Random.Range (-5, 5), 0);
				var building = Instantiate (buildingPrefab, position, Quaternion.identity);

				var behaviour = building.GetComponent<BuildingBehaviour> ();
				behaviour.SetBuildingType (pitch);
				behaviour.SetPoints (0);
				behaviour.SetMaxPoints (4);
			}
		}
	}


	void Update ()
	{
		_currentTimeOffset = AudioSettings.dspTime - _startDspTime;
		currentBeatPosition = _currentTimeOffset / _secPerBeat;

		while (_noteIndex < _notes.Length && _notes [_noteIndex].offset < currentBeatPosition + beatsToShow) {
			var note = Instantiate (notePrefab);
			var nc = note.GetComponent<NoteBehaviour> ();
			nc.BeatOffset = (float)_notes [_noteIndex].offset;
			nc.Pitch = _notes [_noteIndex].pitch;
			nc.Length = (float)_notes [_noteIndex].length;

			_noteIndex++;
		}
	}
}

class SimpleNote
{
	public int pitch;
	public double offset;
	public double length;
}