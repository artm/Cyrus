using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class VeeJoy : MonoBehaviour {

	public FloatDesc[] floats;
	bool showGUI = true;
	List<FloatController> floatControllers = new List<FloatController>();
	int current = 0;

	FloatController CurrentController {
		get {
			return (floatControllers.Count==0) ? null : floatControllers[current%floatControllers.Count];
		}
	}

	public class PropertyError : System.Exception {
		public PropertyError()
			: base()
		{}
	}

	[System.Serializable]
	public class FloatDesc {
		public Object target;
		public string property;
		public float min, max, speed;

		public bool canListen = true, listen = false;
		public int band = 1;
		public float add = 0, mul = 1;
	}

	public abstract class FloatController {
		public FloatDesc desc;

		public FloatController(FloatDesc d)
		{
			desc = d;
		}

		public void ApplyDelta(float delta)
		{
			if (Mathf.Abs(delta) < Mathf.Epsilon)
				return;
			Value = Mathf.Clamp( Value + delta * desc.speed, desc.min, desc.max );
		}

		public void ApplyDeltaAdd(float delta)
		{
			if (Mathf.Abs(delta) < Mathf.Epsilon)
				return;
			desc.add += delta * desc.speed;
		}

		public void ApplyDeltaMul(float delta)
		{
			if (Mathf.Abs(delta) < Mathf.Epsilon)
				return;
			desc.mul += delta * desc.speed;
		}

		public void Set(float v)
		{
			Value = Mathf.Clamp( v, desc.min, desc.max );
		}

		protected abstract float Value { get; set; }
		public abstract string Name { get; }
		public string Feedback { get { return Name + ":" + Value; } }

		public static void AddToList(FloatDesc desc, List<FloatController> floatControllers) {
			try {
				string[] tokens = desc.property.Split('.');
				if (tokens.Length == 1) {
					System.Type type = desc.target.GetType();
					if (type == typeof(Material)) {
						floatControllers.Add(new MaterialFloatController(desc));
					} else {
						FieldInfo fi = type.GetField(desc.property);
						if (fi == null || fi.FieldType != typeof(float))
							throw new PropertyError();
						floatControllers.Add(new FieldFloatController(desc,fi));
					}
				} else if (tokens.Length == 2) {
					// the property name is ComponentName.PropertyName
					Component c = (desc.target as GameObject).GetComponent(tokens[0]);
					if (c==null)
						throw new PropertyError();
					System.Type type = c.GetType();
					FieldInfo fi = type.GetField(tokens[1]);
					if (fi == null || fi.FieldType != typeof(float))
						throw new PropertyError();
					desc.target = c;
					floatControllers.Add(new FieldFloatController(desc,fi));
				}
			} catch (PropertyError) {
				Debug.LogError("Error reflecting on property " + desc.property + " of " + desc.target);
			}
		}
	}

	public class MaterialFloatController : FloatController {
		Material material;

		public MaterialFloatController(FloatDesc desc)
			: base(desc)
		{
			material = desc.target as Material;
		}

		protected override float Value {
			get { return material.GetFloat(desc.property); }
			set { material.SetFloat(desc.property, value); }
		}

		public override string Name {
			get { return material.name + "." + desc.property; }
		}
	}

	public class FieldFloatController : FloatController {
		FieldInfo field;
		public FieldFloatController(FloatDesc desc, FieldInfo fi)
			: base(desc)
		{
			field = fi;
		}

		protected override float Value {
			get { return (float)field.GetValue(desc.target); }
			set { field.SetValue(desc.target,value); }
		}

		public override string Name {
			get { return desc.property; }
		}
	}

	[System.Serializable]
	public class ControlGroup {
		public Object target;
		public FloatController[] properties;
	}


	void Start () {
		// at startup make a list of individual properties
		foreach(FloatDesc desc in floats) {
			FloatController.AddToList(desc, floatControllers);
		}
	}

	void Update () {
		if (floatControllers.Count == 0) return;

		if (Input.GetKeyDown("joystick button 6")) {
			if (--current < 0)
				current = floatControllers.Count - 1;
		} else if (Input.GetKeyDown("joystick button 7")) {
			current = (++current) % floatControllers.Count;
		}

		if (Input.GetKeyDown("tab")) {
			showGUI = !showGUI;
			Screen.showCursor = showGUI;
		}


		if (CurrentController.desc.listen) {
			CurrentController.ApplyDeltaAdd(Input.GetAxis("Vertical3") * Time.deltaTime);
			CurrentController.ApplyDeltaMul(Input.GetAxis("Horizontal3") * Time.deltaTime);
		} else {
			CurrentController.ApplyDelta(Input.GetAxis("Vertical3") * Time.deltaTime);
		}

		if (CurrentController.desc.canListen) {
			// allow control over:
			// band
			if (Input.GetKeyDown("joystick button 4")) {
				if (--CurrentController.desc.band < 0)
					CurrentController.desc.band = 15;
			} else if (Input.GetKeyDown("joystick button 5")) {
				CurrentController.desc.band = (++CurrentController.desc.band) % 16;
			}
			// toggle listen
			if (Input.GetKeyDown("joystick button 8")) {
				CurrentController.desc.listen = !CurrentController.desc.listen;
			}
		}
	}

	void OnGUI() {
		if (!showGUI) return;

		if (floatControllers.Count == 0) {
			GUI.Label(new Rect(10,10,100,20), "No controls defined");
			return;
		}

		GUILayout.BeginVertical();
		GUILayout.Label( CurrentController.Feedback );
		if (CurrentController.desc.canListen) {
			CurrentController.desc.listen = GUILayout.Toggle(CurrentController.desc.listen, "listen");
			GUILayout.Label("Band: " + CurrentController.desc.band);
			GUILayout.Label(string.Format("+[{0}] *[{1}]",
			                              CurrentController.desc.add,
			                              CurrentController.desc.mul));
		}
		GUILayout.EndVertical();
	}

	void OnAudioSpectrum(float[] s)
	{
		foreach(FloatController con in floatControllers) {
			if (con.desc.canListen && con.desc.listen) {
				con.Set( con.desc.add + con.desc.mul*s[con.desc.band % s.Length] );
			}
		}
	}
}
