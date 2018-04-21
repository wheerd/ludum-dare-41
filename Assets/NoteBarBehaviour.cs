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

	public void AddNote(float offset, float length) {
		var obj = Instantiate (NotePrefab, Bar.transform);
		var note = obj.GetComponent<NoteBehaviour> ();

		note.Length = length;
		note.BeatOffset = offset;

		note.OnStart += Activate;
		note.OnEnd += Deactivate;
	}

	void Activate ()
	{
		_image.color = new Color (1, 1, 1, 1f);	
	}

	void Deactivate ()
	{
		_image.color = new Color (1, 1, 1, 0.5f);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
