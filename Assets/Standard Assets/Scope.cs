using UnityEngine;
using System.Collections;

public class Scope : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	void OnAudioSpectrum(float[] s)
	{
		if (s.Length > transform.childCount) {
			Transform orig = transform.GetChild(0);
			Vector3 pos = orig.position;
			Quaternion rot = orig.rotation;
	
			for (int i = 1 ; i<s.Length; i++) {
				pos.x += 1.0f;
				Transform ch = GameObject.Instantiate(orig , pos, rot) as Transform;
				ch.parent = transform;
			}
		}


		for(int i=0; i<s.Length && i<transform.childCount; i++) {
			transform.GetChild(i).localScale = new Vector3(1, 1 + s[i], 1);
		}
	}
}
