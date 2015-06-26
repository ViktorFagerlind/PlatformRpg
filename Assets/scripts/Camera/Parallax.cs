using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
  [System.Serializable]
  public class Layer 
  {
    public float     m_scale;
    public Transform m_object;
  }

  public Layer[] m_layers;                   // Array of all the layers to be parallaxed.
  public float parallaxScale;              // The proportion of the camera's movement to move the backgrounds by.
  public float parallaxReductionFactor;   // How much less each successive layer should parallax.

  public float smoothing;                 // How smooth the parallax effect should be.
  
  private Transform m_cam;               // Shorter reference to the main camera's transform.
  private Vector3   m_camPreviousPosition;          // The postion of the camera in the previous frame.
  
  
  void Awake ()
  {
    // Setting up the reference shortcut.
    m_cam = Camera.main.transform;
  }
  
  
  void Start ()
  {
    // The 'previous frame' had the current frame's camera position.
    m_camPreviousPosition = m_cam.position;
  }

  void Update ()
  {
    float distanceMoved = m_camPreviousPosition.x - m_cam.position.x;
    
    // For each successive background...
    foreach (Layer l in m_layers)
    {
      // The camera has already moved all objects "distanceMoved" in the eyes of the beholder...
      float layerDistanceToMove = -distanceMoved + distanceMoved * l.m_scale;

      float backgroundTargetPosX = l.m_object.position.x + layerDistanceToMove;

      // Create a target position which is the background's current position but with it's target x position.
      Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, l.m_object.position.y, l.m_object.position.z);

      l.m_object.position = backgroundTargetPos;
      
      // Lerp the background's position between itself and it's target position.
      //backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
    }
    
    // Set the previousCamPos to the camera's position at the end of this frame.
    m_camPreviousPosition = m_cam.position;
  }
}