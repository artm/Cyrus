using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VeeJoy : MonoBehaviour {

	[System.Serializable]
	public class FloatDesc {
		public Object target;
		public string property;
		public float min, max, speed;
	}

	public abstract class FloatController {
		public float min, max, speed;
		public void ApplyDelta(float delta)
		{
			if (Mathf.Abs(delta) < Mathf.Epsilon)
				return;
			Value = Mathf.Clamp( Value + delta * speed, min, max );
		}
		protected abstract float Value { get; set; }
		public abstract string Name { get; }
		public string Feedback { get { return Name + ":" + Value; } }

		public static FloatController FromDesc(FloatDesc desc) {
			if (desc.target is Material) {
				if ((desc.target as Material).HasProperty(desc.property)) {
					return new MaterialFloatController(desc);
				}
			} else if (desc.target is GameObject && desc.property == ":ask") {

			}
			return null;
		}

	}

	public class MaterialFloatController : FloatController {
		Material material;
		string property;

		public MaterialFloatController(FloatDesc desc)
		{
			material = desc.target as Material;
			property = desc.property;
			min = desc.min;
			max = desc.max;
			speed = desc.speed;
		}

		protected override float Value {
			get { return material.GetFloat(property); }
			set { material.SetFloat(property, value); }
		}

		public override string Name {
			get { return material.name + "." + property; }
		}
	}

	[System.Serializable]
	public class ControlGroup {
		public Object target;
		public FloatController[] properties;
	}


	public FloatDesc[] floats;

	List<FloatController> floatControllers = new List<FloatController>();
	int current = 0;

	void Start () {
		// at startup make a list of individual properties
		foreach(FloatDesc desc in floats) {
			FloatController fc = FloatController.FromDesc(desc);
			if (fc != null) {
				floatControllers.Add(fc);
			}
		}
	}
	
	void Update () {
		if (floatControllers.Count == 0) return;

		if (Input.GetKeyDown("joystick button 6")) {
			current = (--current) % floatControllers.Count;
		} else if (Input.GetKeyDown("joystick button 7")) {
			current = (++current) % floatControllers.Count;
		}

		floatControllers[current].ApplyDelta(Input.GetAxis("Vertical3") * Time.deltaTime);


	}

	void OnGUI() {
		if (floatControllers.Count == 0) {
			GUI.Label(new Rect(10,10,100,20), "No controls defined");
			return;
		}


		GUILayout.Label( floatControllers[current].Feedback );
	}
}
