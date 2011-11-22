using System.Collections;
using UnityEngine;

/*
 * Some world constants aren't constant
 */
public class ConstantModulator : MonoBehaviour {
	[System.SerializableAttribute]
	public class Modulator {
		public Object target;
		public string paramName;
		public float _const;
		public Vector3 linear;
		public Vector3 pairwise;
		public Vector3 squared;
		public Vector2 clamp;

		public float Compute(Vector3 p) {
			float t = _const
				+ Vector3.Dot(linear, p)
					+ Vector3.Dot(pairwise, new Vector3(p.y*p.z, p.x*p.z, p.x*p.y))
					+ Vector3.Dot(squared, new Vector3(p.x*p.x, p.y*p.y, p.z*p.z));

			//return clamp.x + Mathf.PingPong(t - clamp.x, clamp.y - clamp.x);
			return Mathf.Clamp(t, clamp.x, clamp.y);
		}
	}

	public Modulator[] modulators;

	void Update () {
		Vector3 p = transform.position;

		foreach(Modulator m in modulators) {
			float t = m.Compute(p);
			if (m.target is Material) {
				Material mat = m.target as Material;
				mat.SetFloat( m.paramName, t );
			}
		}
	
	}
}
