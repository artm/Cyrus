using UnityEngine;
using System.Collections;

public class SpecToProp : MonoBehaviour {

	[System.Serializable]
	public class Target {
		public int band;
		public Material target;
		public string property;
		public float _add = 0;
		public float _mul = 1;

		public void Set(float[] s) {
			target.SetFloat( property, _add + _mul * s[band] );
		}
	}

	public Target[] targets;

	void OnAudioSpectrum(float[] s)
	{
		foreach(Target target in targets) {
			target.Set(s);
		}
	}
}
