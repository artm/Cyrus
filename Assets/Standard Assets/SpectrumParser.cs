using UnityEngine;
using System;
using System.Collections;

public class SpectrumParser : MonoBehaviour {
	public float mul = 1;
	public bool log = true;
	public float logP0 = 100f;
	public float logOffs = 3f;


	void OnOscMessage(object[] msg)
	{
		string addr = msg[0] as string;
		if (addr == "/spectrum") {
			float[] spectrum = new float[(msg.Length-3)/2];
			for(int i = 0; i<spectrum.Length; i++) {
				float v = Mathf.Abs((float)msg[3+2*i]);
				spectrum[i] = mul * (log ? Mathf.Max(0, logOffs + Mathf.Log10(v / logP0)) : v);
			}
			BroadcastMessage("OnAudioSpectrum", spectrum, SendMessageOptions.DontRequireReceiver);
		}
	}
}
