using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {

	public float speed = 1;
	public float rotSpeed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate( 0, 0, speed * Time.deltaTime );
		transform.Rotate( Vector3.up, rotSpeed * Input.GetAxis("Horizontal") * Time.deltaTime );
		transform.Rotate( Vector3.right, - rotSpeed * Input.GetAxis("Vertical") * Time.deltaTime );
	}
}
