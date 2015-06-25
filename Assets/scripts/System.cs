using UnityEngine;
using System.Collections;

public class VecInt2
{
  public int x;
  public int y;

  public VecInt2 (int x, int y)
  {
    Set (x, y);
  }

  public void Set (int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  public void Set (VecInt2 pos)
  {
    Set (pos.x, pos.y);
  }

  public VecInt2 Clone ()
  {
    return new VecInt2 (x, y);
  }
}
