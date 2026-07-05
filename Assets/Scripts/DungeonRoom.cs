using UnityEngine;

public class DungeonRoom //should probably keep this private 
{
    private int width;
    private int height;

    private int xPos;
    private int yPos;

    public DungeonRoom(int xPos = 0, int yPos = 0, int width = 0, int height = 0)
    {
        // switched to int for simplicty and visual debugger
        this.width = width;
        this.height = height;
        this.xPos = xPos;
        this.yPos = yPos;
    }

    public Vector2Int GetPos()
    {
        return new Vector2Int(xPos, yPos);
    }

    public Vector2Int SetPos(Vector2Int pos)
    {
        this.xPos = pos.x;
        this.yPos = pos.y;
        return pos;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void SetWidth(int width)
    {
        this.width = width;
    }

    public void SetHeight(int height)
    {
        this.height = height;
    }

    public void OnDrawGizmos() // for visual debugging of the dungeon generation
    {
        //Draw xyz
        Debug.DrawLine(Vector3.zero, Vector3.right, Color.red);
        Debug.DrawLine(Vector3.zero, Vector3.up, Color.green);
        Debug.DrawLine(Vector3.zero, Vector3.forward, Color.blue);

        RectInt a = new RectInt(xPos, yPos, width, height); 
        
        AlgorithmsUtils.DebugRectInt(a, Color.yellow);
    }

}
