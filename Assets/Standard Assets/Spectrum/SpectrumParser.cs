using UnityEngine;
using System;
using System.Collections;

public class SpectrumParser : MonoBehaviour {
	public float referenceAmplitude = 100f;
	public float sensitivity = 3f;


	void OnOscMessage(object[] msg)
	{
		string addr = msg[0] as string;
		if (addr == "/spectrum") {
			float[] spectrum = new float[(msg.Length-3)/2];
			for(int i = 0; i<spectrum.Length; i++) {
				float v = Mathf.Abs((float)msg[3+2*i]);
				spectrum[i] = Mathf.Max(0, sensitivity + Mathf.Log10(v / referenceAmplitude));
			}
			BroadcastMessage("OnAudioSpectrum", spectrum, SendMessageOptions.DontRequireReceiver);
		}
	}
}
