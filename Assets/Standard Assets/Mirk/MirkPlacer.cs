using UnityEngine;
using System.Collections;

/*
 * Magick
 *
 * snap to grid in view space somehow.
 */
public class MirkPlacer : MonoBehaviour {

	public float minDistance = 0;
	public Renderer fadeLayer = null;

	public float noiseScale = 1;
	float oldScale = 1;

	Material origMaterial;
	float fade = 1;
	Vector3 lastParentPos;
	float maxDistance;

	void Start () {
		lastParentPos = transform.parent.position;
		maxDistance = transform.localPosition.z;

		if (fadeLayer) {
			origMaterial = fadeLayer.sharedMaterial;
			oldScale = noiseScale = origMaterial.GetFloat("NoiseScale");
		}
	}
	
	void Update () {
		if (oldScale != noiseScale) {
			origMaterial.SetFloat("NoiseScale", noiseScale);

			// we want to pin layer 1 in noise space
			Vector3 l1 = transform.GetChild(1).position;
			Vector3 offset = l1 * oldScale / noiseScale - l1;
			transform.parent.position += offset;
			lastParentPos += offset;
			oldScale = noiseScale;
		}
		
		// how much parent moved in the current view direction
		float delta = transform.parent.InverseTransformPoint(lastParentPos).z;

		lastParentPos = transform.parent.position;

		Vector3 tmp = transform.localPosition;
		tmp.z = minDistance + Mathf.Repeat( tmp.z - minDistance + delta, maxDistance - minDistance );
		transform.localPosition = tmp;

		if (fadeLayer) {
			float newFade = (tmp.z-minDistance) / (maxDistance-minDistance);
			fadeLayer.material.CopyPropertiesFromMaterial(origMaterial);
			fade = newFade;
			fadeLayer.material.SetFloat("Fade", fade);
		}
	}
}
