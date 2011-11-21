using UnityEngine;
using System.Collections;

public class DiscoLight : MonoBehaviour {

	public int offset = 0;
	public float Add = 0, Mul = 1;

	void OnAudioSpectrum(float[] s)
	{
		light.color = specColor(s, offset) * Mul + new Color(Add,Add,Add);
	}

	Color specColor(float[] s, int offs)
	{
		return new Color( s[offs % s.Length], s[(offs+1) % s.Length], s[(offs+2) % s.Length] );
	}
}
