using UnityEngine;
using System.Collections;

public class SystemCommands : MonoBehaviour 
{
	// Update is called once per frame
	void Update () 
  {
    if (Input.GetKeyDown ("escape"))
        Application.Quit ();
    
    if (Input.GetKeyDown ("p"))
      Application.CaptureScreenshot ("Screenshot.png");
	}
}
