using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Platform
{
  public VecInt2 m_left;
  public VecInt2 m_right;

  public bool    m_wallLeft;
  public bool    m_wallRight;
  //  VecInt2 m_size;

  //Tile[] m_tiles;

  public Platform (Tile startTile)
  {
    m_right = startTile.m_pos.Clone ();
    m_left  = startTile.m_pos.Clone ();
  }

  public void addTile (Tile t)
  {
    if (t.m_pos.x < m_left.x)
      m_left.Set (t.m_pos);

    if (t.m_pos.x > m_right.x)
      m_right.Set (t.m_pos);
  }
}

public class PlatformManager
{
  enum Processing
  {
    NotDone = 0,
    Done = 1
  };

  List<Platform>  m_platforms;
  GameObject      m_baseObject;

  public PlatformManager (TileSheet tileSheet, Sprite leftDebugSprite, Sprite rightDebugSprite, Vector2 spacing)
  {
    CreateAll (tileSheet, leftDebugSprite, rightDebugSprite, spacing);
  }

  public void CreateAll (TileSheet tileSheet, Sprite leftDebugSprite, Sprite rightDebugSprite, Vector2 spacing)
  {
    if (m_baseObject != null)
      GameObject.Destroy (m_baseObject);

    m_platforms = new List<Platform> ();
    m_baseObject = new GameObject ("Platform debug");
    
    for (int i = 0; i < tileSheet.nofTiles; i++)
    {
      Tile t = tileSheet.getTile (i);
      if (t != null)
        t.m_meta = (int)Processing.NotDone;
    }
    
    findPlatforms (tileSheet);
    
    createDebugInfo (leftDebugSprite, rightDebugSprite, spacing);
  }

  void createDebugInfo (Sprite left, Sprite right, Vector2 spacing)
  {
    foreach (Platform p in m_platforms)
    {
      GameObject lo = new GameObject ("Left (" + p.m_left.x + "," + p.m_left.y + ")");
      SpriteRenderer renderer = lo.AddComponent<SpriteRenderer> ();
      renderer.sprite = left;
      lo.transform.position = new Vector3 ((p.m_left.x-0.3f) * spacing.x, p.m_left.y * spacing.y, 0f);
      renderer.sortingLayerName = "Debug";
      renderer.color = p.m_wallLeft ? Color.red : Color.white;
      lo.transform.parent = m_baseObject.transform;

      GameObject ro = new GameObject ("Right (" + p.m_right.x + "," + p.m_right.y + ")");
      renderer = ro.AddComponent<SpriteRenderer> ();
      renderer.sprite = right;
      ro.transform.position = new Vector3 ((p.m_right.x+0.3f) * spacing.x, p.m_right.y * spacing.y, 0f);
      renderer.sortingLayerName = "Debug";
      renderer.color = p.m_wallRight ? Color.red : Color.white;
      ro.transform.parent = m_baseObject.transform;
    }
  }

  void findPlatforms (TileSheet tileSheet)
  {
    for (int i = 0; i < tileSheet.nofTiles; i++)
    {
      Tile t = tileSheet.getTile (i);

      if (t != null && t.m_meta == (int)Processing.NotDone)
      {
        t.m_meta = (int)Processing.Done;

        if (t.isWalkable ())
          processPlatform (tileSheet, i);
      }
    }
  }
  
  void processPlatform (TileSheet tileSheet, int index)
  {
    Tile startTile = tileSheet.getTile (index);
    Platform p = new Platform (startTile);

    searchSideways (p, tileSheet, startTile, true);
    searchSideways (p, tileSheet, startTile, false);

    p.m_wallLeft  = tileSheet.getTile (p.m_left.x - 1, p.m_left.y) != null;
    p.m_wallRight = tileSheet.getTile (p.m_right.x + 1, p.m_right.y) != null;

    m_platforms.Add (p);
  }

  void searchSideways (Platform p, TileSheet tileSheet, Tile startTile, bool isRightDir)
  {
    bool finding = true;
    Tile currentTile = startTile;

    int addDirNumber = isRightDir ? 1 : -1;

    for (int x = startTile.m_pos.x+addDirNumber; 
             x < tileSheet.m_width && x > 0 && finding; 
             x += addDirNumber)
    {
      finding = false;
      
      for (int y = currentTile.m_pos.y - 1; y <= currentTile.m_pos.y + 1; y++)
      {
        Tile t = tileSheet.getTile (x, y);
        
        if (t != null)
        {
          t.m_meta = (int)Processing.Done;

          if (t.isWalkable ())
          {
            currentTile = t;
            p.addTile (t);

            finding = true;
            break;
          }
        }
      }
    }
  }
}
