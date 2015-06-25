using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMap : MonoBehaviour
{
  public  int     m_width;
  public  int     m_height;
  public  Vector2 m_spacing;

  public  Sprite  m_upRoad;
  public  Sprite  m_upRoadEndsLeft;
  public  Sprite  m_upRoadEndsRight;

  public  Sprite  m_leftPfmDebug;
  public  Sprite  m_rightPfmDebug;

  public  Sprite  m_upTile;
  public  Sprite  m_downTile;
  public  Sprite  m_leftTile;
  public  Sprite  m_rightTile;
  public  Sprite  m_leftDownTile;
  public  Sprite  m_leftDownStartTile;
  public  Sprite  m_leftDownEndTile;
  public  Sprite  m_leftUpTile;
  public  Sprite  m_rightDownTile;
  public  Sprite  m_rightDownStartTile;
  public  Sprite  m_rightDownEndTile;
  public  Sprite  m_rightUpTile;
  public  Sprite  m_middleTile;

  public  Dictionary<TileShape, Sprite> accounts;

  TileSheet m_tileSheet;
  Noise     m_noise;
  
  TileChoise[] m_tileRules; 

  PlatformManager m_platformManager;

  Texture2D m_texture;

  void Awake ()
  {
    m_noise     = new Noise (2);
    m_tileSheet = new TileSheet (m_width, m_height);

    m_tileRules = new TileChoise[8];
    m_tileRules [0] = new TileChoise (TileShape.DownRight, TileCriteria.No, TileCriteria.Yes, TileCriteria.No, TileCriteria.Yes);
    m_tileRules [1] = new TileChoise (TileShape.DownLeft, TileCriteria.No, TileCriteria.Yes, TileCriteria.Yes, TileCriteria.No);
    m_tileRules [2] = new TileChoise (TileShape.UpRight, TileCriteria.Yes, TileCriteria.No, TileCriteria.No, TileCriteria.Yes);
    m_tileRules [3] = new TileChoise (TileShape.UpLeft, TileCriteria.Yes, TileCriteria.No, TileCriteria.Yes, TileCriteria.No);
    m_tileRules [4] = new TileChoise (TileShape.Down, TileCriteria.Yes, TileCriteria.No, TileCriteria.DontCare, TileCriteria.DontCare);
    m_tileRules [5] = new TileChoise (TileShape.Up, TileCriteria.No, TileCriteria.DontCare, TileCriteria.DontCare, TileCriteria.DontCare);
    m_tileRules [6] = new TileChoise (TileShape.Left, TileCriteria.DontCare, TileCriteria.DontCare, TileCriteria.No, TileCriteria.Yes);
    m_tileRules [7] = new TileChoise (TileShape.Right, TileCriteria.DontCare, TileCriteria.DontCare, TileCriteria.Yes, TileCriteria.No);


    int textureSize = 2048;
    m_texture = new Texture2D (textureSize, textureSize);
    for (int x = 0; x < textureSize; x++)
      for (int y = 0; y < textureSize; y++)
        m_texture.SetPixel (x, y,  new Color (1, 1, 1, 0)); //(float)x/(float)textureSize, (float)y/(float)textureSize, (float)(x-y)/(float)textureSize));
    m_texture.Apply ();


    SpriteRenderer renderer =  GetComponent<SpriteRenderer>();
    Sprite sprite = new Sprite ();
    sprite = Sprite.Create (m_texture,new Rect(0, 0, textureSize, textureSize), new Vector2 (0,0), 1f);
    renderer.sprite = sprite;
    renderer.sortingLayerName = "Level base";

    GenerateLevel ();

    m_platformManager = new PlatformManager (m_tileSheet, m_leftPfmDebug, m_rightPfmDebug, m_spacing);
  }

  void GenerateLevel ()
  {
    PerlinCreate ();
    
    fatten ();
    
    setTileShapes ();
    
    setTileSprites ();
  }
  
  TileShape findShape (int x, int y)
  {
    foreach (TileChoise tc in m_tileRules)
    {
      if (tc.Test (m_tileSheet.getTile(x,y+1)   != null,
                   m_tileSheet.getTile(x,y-1)   != null,
                   m_tileSheet.getTile(x-1,y)   != null,
                   m_tileSheet.getTile(x+1,y)   != null, 
                   m_tileSheet.getTile(x-1,y+1) != null,
                   m_tileSheet.getTile(x-1,y-1) != null,
                   m_tileSheet.getTile(x+1,y+1) != null,
                   m_tileSheet.getTile(x+1,y-1) != null))
      {
        return tc.m_tileShape;
      }

    }

    return TileShape.Middle;
  }

  void fatten ()
  {
    for (int y=1; y<m_height-1; y++)
    {
      for (int x=1; x<m_width-1; x++)
      {
        if (m_tileSheet.getTile (x, y) != null &&
            ((m_tileSheet.getTile (x, y + 1) == null && m_tileSheet.getTile (x, y - 1) == null) ||
         (m_tileSheet.getTile (x + 1, y) == null && m_tileSheet.getTile (x - 1, y) == null)))
        {
          m_tileSheet.setTile (new Tile (x, y-1));
          m_tileSheet.setTile (new Tile (x-1, y));
          m_tileSheet.setTile (new Tile (x-1, y-1));
        }
      }
    }
  }

  void setTileShapes ()
  {
    // Set the "normal" shapes
    for (int x=1; x<m_width-1; x++)
    {
      for (int y=1; y<m_height-1; y++)
      {
        Tile tile = m_tileSheet.getTile (x,y);
        
        if (tile != null)
          tile.m_shape = findShape (x, y);
      }
    }

    // Set the easier slopes...
    for (int x=1; x<m_width-1; x++)
    {
      for (int y=1; y<m_height-1; y++)
      {
        Tile tile        = m_tileSheet.getTile (x,y);
        Tile tileRight   = m_tileSheet.getTile (x+1,y);

        if (tile != null && tileRight != null &&
            tile.m_shape == TileShape.DownRight && tileRight.m_shape == TileShape.Up)
        {
          tile.m_shape = TileShape.DownRightStart;
          tileRight.m_shape = TileShape.DownRightEnd;
        }
        if (tile != null && tileRight != null &&
            tile.m_shape == TileShape.Up && tileRight.m_shape == TileShape.DownLeft)
        {
          tile.m_shape = TileShape.DownLeftEnd;
          tileRight.m_shape = TileShape.DownLeftStart;
        }
      }
    }
  }

  void setTileSprites ()
  {
    Dictionary<TileShape, Sprite> shapeToSprite = new Dictionary<TileShape, Sprite>();
    shapeToSprite[TileShape.Up]             = m_upTile;
    shapeToSprite[TileShape.Down]           = m_downTile;
    shapeToSprite[TileShape.Left]           = m_leftTile;
    shapeToSprite[TileShape.Right]          = m_rightTile;
    shapeToSprite[TileShape.UpLeft]         = m_leftUpTile;
    shapeToSprite[TileShape.DownLeft]       = m_leftDownTile;
    shapeToSprite[TileShape.DownLeftStart]  = m_leftDownStartTile;
    shapeToSprite[TileShape.DownLeftEnd]    = m_leftDownEndTile;
    shapeToSprite[TileShape.UpRight]        = m_rightUpTile;
    shapeToSprite[TileShape.DownRight]      = m_rightDownTile;
    shapeToSprite[TileShape.DownRightStart] = m_rightDownStartTile;
    shapeToSprite[TileShape.DownRightEnd]   = m_rightDownEndTile;
    shapeToSprite[TileShape.Middle]         = m_middleTile;

    for (int x=1; x<m_width-1; x++)
    {
      for (int y=1; y<m_height-1; y++)
      {
        Tile tile = m_tileSheet.getTile (x,y);

        if (tile != null)
        {
          Sprite sprite;

          tile.m_gameObject = new GameObject ("Tile (" + x + "," + y + ")");
          SpriteRenderer renderer = tile.m_gameObject.AddComponent<SpriteRenderer>();

          Tile tileLeft = m_tileSheet.getTile (x-1,y);
          Tile tileRight = m_tileSheet.getTile (x+1,y);
          if ((tile != null       && tile.m_shape == TileShape.Up) && 
              (tileLeft != null   && tileLeft.m_shape  == TileShape.Up) && 
              (tileRight != null  && tileRight.m_shape == TileShape.Up))
            sprite = m_upRoad;
          else if ((tile != null      && tile.m_shape == TileShape.Up) && 
                   (tileLeft != null  && tileLeft.m_shape  == TileShape.Up) && 
                   (tileRight == null || tileRight.m_shape != TileShape.Up))
            sprite = m_upRoadEndsRight;
          else if ((tile != null      && tile.m_shape == TileShape.Up) && 
                   (tileLeft == null  || tileLeft.m_shape != TileShape.Up) && 
                   (tileRight != null && tileRight.m_shape == TileShape.Up))
            sprite = m_upRoadEndsLeft;
          else
            sprite = shapeToSprite[tile.m_shape];

          renderer.sprite = sprite;
          renderer.sortingLayerName = "Level base";

          if (x < 16 && y < 16)
          {
            Color[] pixles = sprite.texture.GetPixels ();
            m_texture.SetPixels (x * (int)m_spacing.x, y * (int)m_spacing.y, (int)m_spacing.x, (int)m_spacing.y, pixles);
          }

          
          tile.m_gameObject.transform.parent = transform;
          tile.m_gameObject.transform.position = new Vector3 (x * m_spacing.x, y * m_spacing.y, 0f);
        }
      }
    }

    m_texture.Apply ();
  }

  void Clear ()
  {
    for (int x=0; x<m_width; x++)
      for (int y=0; y<m_height; y++)
        m_tileSheet.removeTile (x, y);
  }

  void PerlinCreate ()
  {
    const float freq_x = 0.04f;
    const float freq_relY = 5f;

    m_noise.ReSeed ();

    for (int x=0; x<m_width; x++)
    {
      for (int y=0; y<m_height; y++)
      {
        bool filled = m_noise.BrownianLayeredBool (
          new Vector2 ((float)x, (float)y),
          new Vector2[3] {new Vector2 (freq_x, freq_x * freq_relY),
          new Vector2 (0.03f, 0.03f),
        new Vector2 (0.01f, 0.01f)},
        new float[3]   {0.55f, 0.55f, 0.58f});

        filled |= y <= 3;

        if (filled)
          m_tileSheet.setTile (new Tile (x, y));
      }
    }
  }

  void Update ()
  {
    if (Input.GetKeyDown ("space"))
    {
      Clear ();
      GenerateLevel ();

      m_platformManager.CreateAll (m_tileSheet, m_leftPfmDebug, m_rightPfmDebug, m_spacing);
    }
  }

}
