using System;
using System.Collections;
using System.Collections.Generic;
using BeepBox;
using UnityEngine;

public class BeatMachine : MonoBehaviour
{
    public TextAsset beatsFile;

    private double _secPerBeat;
    private double _startDspTime;
    private double _currentTimeOffset;
    private double _currentBeatPosition;
    private BeepBoxFile _beats;

    void Start ()
    {
        _beats = JsonUtility.FromJson<BeepBoxFile>(beatsFile.text);

        _secPerBeat = 60d / _beats.beatsPerMinute;
        _startDspTime = (float)AudioSettings.dspTime;

        GetComponent<AudioSource>().Play();
    }
	
	void Update ()
    {
        _currentTimeOffset = AudioSettings.dspTime - _startDspTime;
        _currentBeatPosition = _currentTimeOffset / _secPerBeat;
    }
}