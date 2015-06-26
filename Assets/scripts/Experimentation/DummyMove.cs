using UnityEngine;
using System.Collections;

public class DummyMove : MonoBehaviour {


  public Vector2 m_speed;

	// Update is called once per frame
	void Update () 
  {
    Vector3 movement = new Vector3 (Input.GetAxis("Horizontal") * m_speed.x, Input.GetAxis("Vertical") * m_speed.y, 0f);

    transform.Translate (movement);
	}
}
