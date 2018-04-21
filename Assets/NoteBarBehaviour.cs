using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteBarBehaviour : MonoBehaviour {

	public Sprite[] BuildingSprites;

	public GameObject Building;

	public GameObject Bar;

	public GameObject NotePrefab;

	private int _buildingType = 3;
	private BeatMachine _beatMachine;
	Image _image;

	public int CurrentValue { get; private set; }

	// Use this for initialization
	void Start () {
		_beatMachine = FindObjectOfType<BeatMachine> ();

		_image = Building.GetComponent<Image> ();

		_image.color = new Color (1, 1, 1, 0.5f);

		UpdateBuildingSprite ();
	}

	public void SetBuildingType(int type) {
		_buildingType = type;
		UpdateBuildingSprite ();
	}

	void UpdateBuildingSprite ()
	{
		Building.GetComponent<Image> ().sprite = BuildingSprites [_buildingType];
	}

	public void AddNote(float offset, float length, int worth) {
		var obj = Instantiate (NotePrefab, Bar.transform);
		var note = obj.GetComponent<NoteBehaviour> ();

		note.Length = length;
		note.BeatOffset = offset;
		note.Worth = worth;

		note.OnStart += Activate;
		note.OnEnd += Deactivate;
	}

	void Activate (int value)
	{
		CurrentValue = value;
		_image.color = new Color (1, 1, 1, 1f);	
	}

	public void Deactivate ()
	{
		CurrentValue = 0;
		_image.color = new Color (1, 1, 1, 0.5f);	
	}

	public int Hit ()
	{
		var value = CurrentValue;
		Deactivate ();
		return value;
	}
}
