using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

  public float     m_damping;
  public Transform m_objectToFollow;

	// Update is called once per frame
	void Update () 
  {
    float x = Mathf.Lerp (transform.position.x, m_objectToFollow.position.x, m_damping * Time.smoothDeltaTime);
    float y = Mathf.Lerp (transform.position.y, m_objectToFollow.position.y, m_damping * Time.smoothDeltaTime);

    transform.position = new Vector3 (x, y, transform.position.z);
  }
}
