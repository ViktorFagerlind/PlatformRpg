using UnityEngine;
using System.Collections;

public class SetFps : MonoBehaviour
{

  public int m_frameRate = 60;

  // Use this for initialization
  void Start()
  {
    Application.targetFrameRate = m_frameRate;
  }

  // Update is called once per frame
  void Update()
  {
  }
}
