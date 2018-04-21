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

	public int beatsToShow = 8;

    private double _secPerBeat;
    private double _startDspTime;
    private double _currentTimeOffset;
    public double currentBeatPosition;
    private BeepBoxFile _beats;

	private SimpleNote[] _notes;

	private int _noteIndex = 0;

    void Start ()
    {
        _beats = JsonUtility.FromJson<BeepBoxFile>(beatsFile.text);

		_notes = GetNotesFromChannel(_beats.channels [1]);

        _secPerBeat = 60d / _beats.beatsPerMinute;
        _startDspTime = (float)AudioSettings.dspTime;

        GetComponent<AudioSource>().Play();
    }

	private SimpleNote[] GetNotesFromChannel(Channel channel) {
		return channel.sequence.Take(_beats.loopBars).SelectMany ((s, i) => channel.patterns [s-1].notes.Select (n => GetNote (n, i))).ToArray ();
	}

	private SimpleNote GetNote(Note note, int offset) {
		var noteOffset = (note.StartTick + offset * _beats.ticksPerBeat * _beats.beatsPerBar) / (double)_beats.ticksPerBeat;

		return new SimpleNote {
			pitch = note.pitches [0],
			offset = noteOffset,
			length = (note.EndTick - note.StartTick) / (double)_beats.ticksPerBeat
		};
	}
	
	void Update ()
    {
        _currentTimeOffset = AudioSettings.dspTime - _startDspTime;
        currentBeatPosition = _currentTimeOffset / _secPerBeat;

		while (_noteIndex < _notes.Length && _notes [_noteIndex].offset < currentBeatPosition + beatsToShow) {
			var note = Instantiate (notePrefab);
			var nc = note.GetComponent<NoteBehaviour> ();
			nc.BeatOffset = (float)_notes [_noteIndex].offset;
			nc.Pitch = (float)_notes [_noteIndex].pitch;
			nc.Length = (float)_notes [_noteIndex].length;

			_noteIndex++;
		}
    }
}

class SimpleNote {
	public double pitch;
	public double offset;
	public double length;
}