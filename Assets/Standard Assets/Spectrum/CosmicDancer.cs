using UnityEngine;
using System.Collections;

public class CosmicDancer : MonoBehaviour {

	public int offset = 1;
	public float scaleSpeed = 1;

	void OnAudioSpectrum(float[] s)
	{
		transform.localScale = new Vector3(1,1,1) + scaleSpeed * specVec(s, offset);
	}

	Vector3 specVec(float[] s, int offs)
	{
		return new Vector3( s[offs % s.Length], s[(offs+1) % s.Length], s[(offs+2) % s.Length] );
	}
}
