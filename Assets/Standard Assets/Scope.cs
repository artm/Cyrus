using UnityEngine;
using System.Collections;

public class Scope : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Transform orig = transform.GetChild(0);
		Vector3 pos = orig.position;
		Quaternion rot = orig.rotation;

		for (int i = 1 ; i<32; i++) {
			pos.x = i;
			Transform ch = GameObject.Instantiate(orig , pos, rot) as Transform;
			ch.parent = transform;
		}
	}

	void OnAudioSpectrum(float[] s)
	{
		for(int i=0; i<s.Length && i<transform.childCount; i++) {
			transform.GetChild(i).localScale = new Vector3(1, 1 + Mathf.Abs(s[i]), 1);
		}
	}
}
