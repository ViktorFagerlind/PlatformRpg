using UnityEngine;
using System.Collections;

public class Misc : MonoBehaviour
{

  static public Transform GetChildByName (Transform from, string name) 
  {
    Transform[] ts = from.transform.GetComponentsInChildren <Transform> ();

    foreach (Transform t in ts) 
    {
      if (t.gameObject.name == name) 
        return t;
    }

    return null;
  }

  static public Transform ResetTransform (Transform t) 
  {
    t.localRotation   = Quaternion.identity;
    t.localPosition   = new Vector3 (0,0,0);
    t.localScale      = new Vector3 (1,1,1);

    return t;
  }

  static public void DeleteAllChildren (Transform parent) 
  {
    foreach (Transform child in parent) 
      GameObject.Destroy (child.gameObject);
  }

  static public Color CreateColor256 (int r, int g, int b)
  {
    return new Color ((float)r / 256f, (float)g / 256f, (float)b / 256f);
  }

}
