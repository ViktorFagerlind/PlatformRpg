using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Platform
{
  public VecInt2 m_left;
  public VecInt2 m_right;
  public VecInt2 m_pos;

  public bool    m_wallLeft;
  public bool    m_wallRight;
  public GameObject m_go;
  //  VecInt2 m_size;

  //Tile[] m_tiles;

  public Platform (Tile startTile)
  {
    m_right = startTile.m_pos.Clone ();
    m_left  = startTile.m_pos.Clone ();
    m_pos   = startTile.m_pos.Clone ();
  }

  public string name {get {return "Pfm (" + m_pos.x + "," + m_pos.y + ")";}}

  public void addTile (Tile t)
  {
    if (t.m_pos.x < m_left.x)
      m_left.Set (t.m_pos);

    if (t.m_pos.x > m_right.x)
      m_right.Set (t.m_pos);

    m_pos.Set ((m_left.x + m_right.x) / 2, (m_left.y + m_right.y) / 2);
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
  bool[,]         m_connections;

  public PlatformManager (TileSheet tileSheet, Sprite pfmSprite, Sprite leftDebugSprite, Sprite rightDebugSprite, Vector2 spacing)
  {
    CreateAll (tileSheet, pfmSprite, leftDebugSprite, rightDebugSprite, spacing);
  }

  public void CreateAll (TileSheet tileSheet, Sprite pfmSprite, Sprite leftDebugSprite, Sprite rightDebugSprite, Vector2 spacing)
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
      
    trimPlatforms ();
    
    setupConnections ();
    
    createDebugInfo (pfmSprite, leftDebugSprite, rightDebugSprite, spacing);
  }

  void setupConnections ()
  {
    m_connections = new bool[m_platforms.Count, m_platforms.Count];

    for (int i=0; i< m_platforms.Count; i++)
    {
      for (int j=i+1; j< m_platforms.Count; j++)
      {
        if (m_connections[i, j])
          continue;

        Platform pi = m_platforms[i];
        Platform pj = m_platforms[j];

        if (pi.m_left.x <= pj.m_right.x && pi.m_right.x >= pj.m_left.x)
        {
          m_connections[i,j] = m_connections[j,i] = true;
          Debug.Log (pi.name + " connected to " + pj.name);
        }
      }
    }
     
  }
  
  void trimPlatforms ()
  {
    m_platforms.RemoveAll (p => p.m_right.x - p.m_left.x < 3);
  }
  
  void createDebugInfo (Sprite pfm, Sprite left, Sprite right, Vector2 spacing)
  {
    // Draw left right indicators
    foreach (Platform p in m_platforms)
    {
      GameObject lo = new GameObject ("Left (" + p.m_left.x + "," + p.m_left.y + ")");
      SpriteRenderer renderer = lo.AddComponent<SpriteRenderer> ();
      renderer.sprite = left;
      lo.transform.position = new Vector3 ((p.m_left.x - 0.3f) * spacing.x, p.m_left.y * spacing.y, 0f);
      renderer.sortingLayerName = "Debug";
      renderer.color = p.m_wallLeft ? Color.red : Color.white;
      lo.transform.parent = m_baseObject.transform;

      GameObject ro = new GameObject ("Right (" + p.m_right.x + "," + p.m_right.y + ")");
      renderer = ro.AddComponent<SpriteRenderer> ();
      renderer.sprite = right;
      ro.transform.position = new Vector3 ((p.m_right.x + 0.3f) * spacing.x, p.m_right.y * spacing.y, 0f);
      renderer.sortingLayerName = "Debug";
      renderer.color = p.m_wallRight ? Color.red : Color.white;
      ro.transform.parent = m_baseObject.transform;

      
      p.m_go = new GameObject (p.name);
      renderer = p.m_go.AddComponent<SpriteRenderer> ();
      renderer.sprite = pfm;
      p.m_go.transform.position = new Vector3 (p.m_pos.x * spacing.x, p.m_pos.y * spacing.x, 0);
      renderer.sortingLayerName = "Debug";
      p.m_go.transform.parent = m_baseObject.transform;
    }

    // Draw connections
    for (int i=0; i<m_platforms.Count; i++)
    {
      for (int j=i+1; j< m_platforms.Count; j++)
      {
        if (!m_connections[i, j])
          continue;
        
        Platform pi = m_platforms[i];
        Platform pj = m_platforms[j];

        GameObject o = new GameObject (pi.name + " to " + pj.name);
        o.transform.position = new Vector3 (pi.m_pos.x * spacing.x, pi.m_pos.y * spacing.x, 0);
        o.transform.parent = m_baseObject.transform;
        LineRenderer renderer = o.AddComponent<LineRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Additive"));
        renderer.SetColors (Color.blue, Color.red);
        renderer.SetWidth (5f, 5f);
        renderer.SetVertexCount (2);
        renderer.SetPosition(0, new Vector3 (pi.m_pos.x * spacing.x, pi.m_pos.y * spacing.x, -1));
        renderer.SetPosition(1, new Vector3 (pj.m_pos.x * spacing.x, pj.m_pos.y * spacing.x, -1));
      }
    }
    
    Debug.Log ("Nof platforms: " + m_platforms.Count);
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

    p.m_wallLeft = tileSheet.getTile (p.m_left.x - 1, p.m_left.y) != null;
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
