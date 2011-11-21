using UnityEngine;
using System.Collections;

public class Duplicator : MonoBehaviour {

	public Transform target;
	public int nDuplis = 0;
	public Vector3 posStep = new Vector3(0,0,0);
	public float scaleStep = 0;

	// Use this for initialization
	void Start () {
		if (!target)
			return;
		Vector3 scaleStepVec = Vector3.one * scaleStep;

		for(int i = 1; i <= nDuplis; i++) {
			Transform dupli = (GameObject.Instantiate(target.gameObject,
			                                          target.position,
			                                          target.rotation) as GameObject).transform;
			dupli.parent = target.parent;
			dupli.localPosition = dupli.localPosition + i*posStep;
			dupli.localScale = dupli.localScale + i*scaleStepVec;
		}
	}
}
