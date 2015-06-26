using UnityEngine;
using System.Collections;

public class Noise
{
  int   m_octaves;
  float m_seed;

  public Noise (int octaves)
  {
    m_octaves = octaves;
    ReSeed ();
  }

  public void ReSeed ()
  {
    m_seed = Random.Range (0.0f, 100.0f);
  }

  public float BrownianRange (Vector2 v, Vector2 f, float min, float max)
  {
    return min + BrownianValue (v, f) * (max - min);
  }

  public bool BrownianBool (Vector2 v, Vector2 f, float p = 0.55f)
  {
    return BrownianValue (v, f) > p;
  }
  
  public bool BrownianLayeredBool (Vector2 v, Vector2[] fs, float[] ps)
  {
    for (int i=0; i<ps.Length; i++)
    {
      if (BrownianBool (v, fs[i], ps[i]))
        return true;
    }

    return false;
  }
  
  public float BrownianValue (Vector2 v, Vector2 f)
  {
    float result = 0.0f;
    float amp = 0.5f;
    float normalizeFactor = 0f;

    float x = v.x*f.x, y = v.y*f.y;
    
    for (int i = 0; i < m_octaves; i++)
    {
      result += amp * Mathf.PerlinNoise (m_seed + x, y);
      normalizeFactor += amp;
      x *= 2.0f;
      y *= 2.0f;
      amp *= 0.5f;
    }
    
    return result/normalizeFactor;
  }
}
