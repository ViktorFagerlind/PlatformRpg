using UnityEngine;
using System.Collections;


public enum TileShape
{
  Up,
  Down,
  Left,
  Right,
  UpLeft,
  UpRight,
  DownLeft,
  DownLeftStart,
  DownLeftEnd,
  DownRight,
  DownRightStart,
  DownRightEnd,
  Middle
};

public class TileSheet
{
  Tile[]      m_tiles;
  public int  m_width;
  public int  m_height;

  public double nofTiles
  {
    get { return m_width * m_height; }
  }

  public TileSheet (int w, int h)
  {
    m_width  = w;
    m_height = h;

    m_tiles = new Tile[w*h];
  }

  public void removeTile (int x, int y)
  {
    if (getTile (x, y) != null)
    {
      if (getTile (x, y).m_gameObject != null)
        GameObject.Destroy (getTile (x, y).m_gameObject);

      setTile (x, y, null);
    }
  }
  
  public Tile getTile (int index)
  {
    return m_tiles [index];
  }
      
  public Tile getTile (int x, int y)
  {
    return m_tiles [y * m_width + x];
  }
  
  public void setTile (int x, int y, Tile t)
  {
    m_tiles [y * m_width + x] = t;
  }

  public void setTile (Tile t)
  {
    setTile (t.m_pos.x, t.m_pos.y, t);
  }
}

public class Tile
{
  public GameObject m_gameObject = null;
  public TileShape  m_shape = TileShape.Middle;
  public VecInt2    m_pos;
  public int        m_meta = 0;

  public Tile (int x, int y)
  {
    m_pos = new VecInt2 (x, y);
  }

  public bool isWalkable ()
  {
    return m_shape == TileShape.Up || 
           m_shape == TileShape.DownLeft  || m_shape == TileShape.DownLeftStart  || m_shape == TileShape.DownLeftEnd ||
           m_shape == TileShape.DownRight || m_shape == TileShape.DownRightStart || m_shape == TileShape.DownRightEnd;
  }

}

public enum TileCriteria
{
  Yes,
  No,
  DontCare
};


public class TileChoise
{
  public TileCriteria m_upCriteria;
  public TileCriteria m_downCriteria;
  public TileCriteria m_leftCriteria;
  public TileCriteria m_rightCriteria;
  public TileCriteria m_upLeftCriteria;
  public TileCriteria m_downLeftCriteria;
  public TileCriteria m_upRightCriteria;
  public TileCriteria m_downRightCriteria;
  
  public TileShape m_tileShape;
  
  
  public TileChoise (TileShape    tileShape,
                     TileCriteria upCriteria        = TileCriteria.DontCare,
                     TileCriteria downCriteria      = TileCriteria.DontCare,
                     TileCriteria leftCriteria      = TileCriteria.DontCare,
                     TileCriteria rightCriteria     = TileCriteria.DontCare,
                     TileCriteria upLeftCriteria    = TileCriteria.DontCare,
                     TileCriteria downLeftCriteria  = TileCriteria.DontCare,
                     TileCriteria upRightCriteria   = TileCriteria.DontCare,
                     TileCriteria downRightCriteria = TileCriteria.DontCare)
  {
    m_upCriteria        = upCriteria;
    m_downCriteria      = downCriteria;
    m_leftCriteria      = leftCriteria;
    m_rightCriteria     = rightCriteria;
    m_upLeftCriteria    = upLeftCriteria;
    m_downLeftCriteria  = downLeftCriteria;
    m_upRightCriteria   = upRightCriteria;
    m_downRightCriteria = downRightCriteria;
    
    m_tileShape         = tileShape;
  }
  
  bool Test (bool exists, TileCriteria criteria)
  {
    return criteria == TileCriteria.DontCare || 
      (criteria == TileCriteria.Yes && exists) ||
        (criteria == TileCriteria.No && !exists);
  }
  
  public bool Test (bool upExists, bool downExists, bool leftExists, bool rightExists, 
                    bool upLeftExists, bool downLeftExists, bool upRightExists, bool downRightExists)
  {
    return  Test (upExists,         m_upCriteria)         &&
      Test (downExists,       m_downCriteria)       &&
        Test (leftExists,       m_leftCriteria)       &&
        Test (rightExists,      m_rightCriteria)      &&
        Test (upLeftExists,     m_upLeftCriteria)     &&
        Test (downLeftExists,   m_downLeftCriteria)   &&
        Test (upRightExists,    m_upRightCriteria)    &&
        Test (downRightExists,  m_downRightCriteria);
  }
  
}

