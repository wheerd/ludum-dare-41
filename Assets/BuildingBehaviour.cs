using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour {

	public Sprite[] BuildingSprites;

	public Sprite[] PointSprites;

	public Sprite[] PointBackgrounds;

	public GameObject buildingObject;

	public GameObject pointObject;

	public GameObject backgroundObject;

	private int _buildingType;

	private int _points = 3;

	private int _maxPoints = 4;

	public bool IsComplete { get { return _maxPoints == _points; } }

	void Start () {
		SetBuildingType(Random.Range (0, BuildingSprites.Length));
		UpdatePointSprite ();
		UpdateMaxPointSprite ();
	}

	public void SetBuildingType(int type) {
		_buildingType = type;
		UpdateBuildingSprite ();
	}

	void UpdateBuildingSprite ()
	{
		buildingObject.GetComponent<SpriteRenderer> ().sprite = BuildingSprites [_buildingType];
	}

	public void SetMaxPoints(int points) {
		if (points < _points) {
			SetPoints (points);
		}

		_maxPoints = points;
		UpdateMaxPointSprite ();
	}

	void UpdateMaxPointSprite ()
	{
		backgroundObject.GetComponent<SpriteRenderer> ().sprite = PointBackgrounds [_maxPoints];
	}

	public void SetPoints(int points) {
		_points = points;
		UpdatePointSprite ();
	}

	void UpdatePointSprite ()
	{
		pointObject.GetComponent<SpriteRenderer> ().sprite = PointSprites [_points];
		buildingObject.GetComponent<SpriteRenderer> ().color = new Color(1, 1, 1, IsComplete ? 1f : 0.5f);
	}

	void OnMouseDown() {
		SetPoints ((_points + 1) % (_maxPoints + 1));
	}
}
