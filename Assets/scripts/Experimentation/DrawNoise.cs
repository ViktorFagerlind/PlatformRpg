using UnityEngine;
using System.Collections;

public class DrawNoise : MonoBehaviour
{
  private SpriteRenderer m_spriteRenderer;

  private int m_textureSize = 2*1024;

  private Noise m_noise;
  
  // -------------------------------------------------------------------------------------------
  
  void Awake ()
  {
    m_noise = new Noise (3);

    m_spriteRenderer = GetComponent<SpriteRenderer> ();
    
    Texture2D texture = new Texture2D (m_textureSize, m_textureSize);
    for (int x = 0; x < m_textureSize; x++)
      for (int y = 0; y < m_textureSize; y++)
          texture.SetPixel (x, y, Color.black);

    JustNoise (texture);

    //PlatformNoiseBuilder pnb = new PlatformNoiseBuilder ();
    //pnb.Create (texture, new Vector2 (m_textureSize, m_textureSize));

    texture.Apply ();  

    Rect spriteRect = new Rect (0, 0, m_textureSize, m_textureSize);
    m_spriteRenderer.sprite = Sprite.Create (texture, spriteRect, new Vector2 (0.5f, 0.5f));
  }
  
  // -------------------------------------------------------------------------------------------

  void Start ()
  {

  }
  
  // -------------------------------------------------------------------------------------------
  
  void JustNoise (Texture2D texture)
  {
    const int brickSize = 10;

    for (int x = 0; x < m_textureSize; x+=brickSize)
    {
      for (int y = 0; y < m_textureSize; y+=brickSize)
      {
        float p = m_noise.BrownianValue (new Vector2 ((float)x / (float)m_textureSize, 
                                                      (float)y / (float)m_textureSize),
                                         new Vector2 (7, 30));

        if (p > 0.46)
        {
          for (int bx = 0; bx < brickSize; bx++)
          {
            for (int by = 0; by < brickSize; by++)
            {
              Color c = texture.GetPixel (x+bx, y+by);
              c.g = 1.0f;
              texture.SetPixel (x+bx, y+by, c);
            }
          }
        }
      }
    }
  }

  // -------------------------------------------------------------------------------------------
  
}

public class PlatformNoiseBuilder
{
  private       Noise m_noise     = new Noise (3);

  private const float m_freq      = 0.008f;
  private const int   m_thickness = 20;

  
  public void Create (Texture2D texture, Vector2 size)
  {
    Vector2 startPoint = new Vector2 (); 
    Vector2 endPoint = new Vector2 (0, 0); 

    for (int x = 0; x < size.x; x++)
    {
      int start_y = (int)m_noise.BrownianRange (new Vector2 (x, 0), new Vector2 (m_freq, 0), 0f, 700f);

      for (int y = start_y; y < start_y + m_thickness; y++)
        texture.SetPixel (x, y, Color.white);
 
      Debug.Log ("x: " + x + ", y: " + start_y);
    }


    do
    {
      startPoint.x = endPoint.x + Random.Range (-80f, 80f);
      startPoint.y = m_noise.BrownianRange (new Vector2 (startPoint.x, 0), new Vector2 (m_freq, 0), -100f, 700f);

      CreatePlatform (texture, startPoint, out endPoint);
    } while (endPoint.x < size.x && endPoint.y < size.y);
  }

  // -------------------------------------------------------------------------------------------

  public void CreatePlatform (Texture2D texture, Vector2 start, out Vector2 end)
  {
    float length = Random.Range (100f, 200f);

    for (int x = (int)start.x; x < (int)(start.x + length); x++)
    {
      for (int y = (int)start.y; y < (int)start.y + m_thickness; y++)
      {
        Color c = texture.GetPixel (x, y);
        c.r += 0.5f;
        c.g += 0.3f;
        texture.SetPixel (x, y, c);
      }
    }

    end.x = start.x + length;
    end.y = start.y;
  }
  
  // -------------------------------------------------------------------------------------------
  
}