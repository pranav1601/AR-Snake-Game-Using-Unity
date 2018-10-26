using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FoodController : MonoBehaviour {
	private DetectedPlane detectedPlane;
	private GameObject foodInstance;
	private float foodAge;
	private readonly float maxAge = 10f;

	public GameObject[] foodModels;

	public void SetSelectedPlane(DetectedPlane selectedPlane)
	{
		detectedPlane = selectedPlane;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (detectedPlane == null)
		{
			return;
		}

		if (detectedPlane.TrackingState != TrackingState.Tracking)
		{
			return;
		}
		while (detectedPlane.SubsumedBy != null)
		{
			detectedPlane = detectedPlane.SubsumedBy;
		}
		if (foodInstance == null || foodInstance.activeSelf == false)
		{
			SpawnFoodInstance();
			return;
		}
		foodAge += Time.deltaTime;
		if (foodAge >= maxAge)
		{
			DestroyObject(foodInstance);
			foodInstance = null;
		}

		
	}

	void SpawnFoodInstance ()
	{
		GameObject foodItem = foodModels [Random.Range (0, foodModels.Length)];

		// Pick a location.  This is done by selecting a vertex at random and then
		// a random point between it and the center of the plane.
		List<Vector3> vertices = new List<Vector3> ();
		detectedPlane.GetBoundaryPolygon (vertices);
		Vector3 pt = vertices [Random.Range (0, vertices.Count)];
		float dist = Random.Range (0.05f, 1f);
		Vector3 position = Vector3.Lerp (pt, detectedPlane.CenterPose.position, dist);
		// Move the object above the plane.
		position.y += .05f;


		Anchor anchor = detectedPlane.CreateAnchor (new Pose (position, Quaternion.identity));

		foodInstance = Instantiate (foodItem, position, Quaternion.identity,
			anchor.transform);

		// Set the tag.
		foodInstance.tag = "food";

		foodInstance.transform.localScale = new Vector3 (.025f, .025f, .025f);
		foodInstance.transform.SetParent (anchor.transform);
		foodAge = 0;

		foodInstance.AddComponent<FoodMotion> ();
	}
}
