using UnityEngine;
using System.Collections;

/*
 * Magick
 *
 * snap to grid in view space somehow.
 */
public class MirkPlacer : MonoBehaviour {

	public float minDistance = 0;

	Vector3 lastParentPos;
	float maxDistance;

	public Renderer fadeLayer = null;
	Material origMaterial;
	float fade = 1;

	void Start () {
		lastParentPos = transform.parent.position;
		maxDistance = transform.localPosition.z;

		if (fadeLayer) {
			origMaterial = fadeLayer.sharedMaterial;
		}
	}
	
	void Update () {
		float delta; // how much parent moved in the current view position
		delta = transform.parent.InverseTransformPoint(lastParentPos).z;

		lastParentPos = transform.parent.position;

		Vector3 tmp = transform.localPosition;
		tmp.z = minDistance + Mathf.Repeat( tmp.z - minDistance + delta, maxDistance - minDistance );
		transform.localPosition = tmp;

		if (fadeLayer) {
			float newFade = (tmp.z-minDistance) / (maxDistance-minDistance);
			if (newFade > fade)
				// this is a problem (happens too often?)
				fadeLayer.material = origMaterial;
			fade = newFade;
			fadeLayer.material.SetFloat("Fade", fade);

		}
	}
}
