using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom //should probably keep this private 
{
    private int width;
    private int height;

    private int xPos;
    private int yPos;

    private bool canSplitVertically;
    private bool canSplitHorizontally;

    private List<DungeonRoom> neighboringRooms;

    public DungeonRoom(int xPos = 0, int yPos = 0, int width = 0, int height = 0)
    {
        // switched to int for simplicty and visual debugger
        this.width = width;
        this.height = height;
        this.xPos = xPos;
        this.yPos = yPos;
        neighboringRooms = new List<DungeonRoom>();
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

    public int GetDungeonRoomArea()
    {
        return width * height;
    }

    public bool CanSplitVertically(int minRoomSize)
    {
        CheckSplitability(minRoomSize);
        return canSplitVertically;
        
    }

    public bool CanSplitHorizontally(int minRoomSize)
    {
        CheckSplitability(minRoomSize);
        return canSplitHorizontally;
    }

    public bool CheckSplitability(int minRoomSize)
    {
        canSplitVertically = width > minRoomSize * 2;
        canSplitHorizontally = height > minRoomSize * 2;
        return canSplitVertically || canSplitHorizontally;
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

    public Vector2Int GetCenter()
    {
        return new Vector2Int(
            xPos + width / 2,
            yPos + height / 2
        );
    }
    
    /// <summary>
    /// Checks whether another room shares an edge with this room.
    /// </summary>
    public bool RoomsBorderEachOther(DungeonRoom otherRoom)
    {
        bool isLeft = otherRoom.GetWidth()+otherRoom.GetPos().x == GetPos().x;
        bool isRight = GetWidth()+GetPos().x == otherRoom.GetPos().x;

        bool isUp = otherRoom.GetHeight()+otherRoom.GetPos().y == GetPos().y;
        bool isDown = GetHeight()+GetPos().y == otherRoom.GetPos().y;

        if(isUp || isDown || isLeft || isRight)
        Debug.Log($"Room A: L={isLeft} R={isRight} B={isDown} T={isUp}");

        if(otherRoom.GetPos().x == GetPos().x  && (isUp || isDown)) 
            return true;

        if(otherRoom.GetPos().y == GetPos().y  && (isLeft || isRight)) 
            return true;

        return false;
    }

    /// <summary>
    /// Pushes other room to this room's neighbor list
    /// </summary>
    public void AddNeighbor(DungeonRoom room)
    {
        neighboringRooms.Add(room);  
        Debug.Log("Neighbor "+ neighboringRooms.Count +" added!");
        //Spawn Door here later
    }

    /// <summary>
    /// Gets the center of both rooms and returns the lenght between them 
    /// </summary>
    public float returnDistanceBetweenRooms(DungeonRoom otherRoom)
    {
        return Vector2.Distance(this.GetCenter(), otherRoom.GetCenter());
    }

    public Vector2Int SplitVertically(int split) // returns the remainder of the height
    {
        int tempWidth = width - split;
        width = split;
        return new Vector2Int(tempWidth, height);
    }

    public Vector2Int SplitHorizontally(int split)
    {
        int tempHeight = height - split;
        height = split;
        return new Vector2Int(width, tempHeight);
    }

    public void OnDrawGizmos() // for visual debugging of the dungeon generation
    {
        //Draw xyz
        Debug.DrawLine(Vector3.zero, Vector3.right, Color.red);
        Debug.DrawLine(Vector3.zero, Vector3.up, Color.green);
        Debug.DrawLine(Vector3.zero, Vector3.forward, Color.blue);

        RectInt a = new RectInt(xPos, yPos, width, height); 
        
        if (canSplitVertically || canSplitHorizontally)
            AlgorithmsUtils.DebugRectInt(a, Color.blue);
        else
            AlgorithmsUtils.DebugRectInt(a, Color.yellow);
        
        if (neighboringRooms.Count > 0)
            foreach (DungeonRoom room in neighboringRooms)
            {
                Vector2Int currentCenter = GetCenter();                
                Vector2Int neighborCenter = room.GetCenter();                

                Vector3 currentCenterFix = new Vector3(currentCenter.x, 0, currentCenter.y);
                Vector3 neighborCenterFix =  new Vector3(neighborCenter.x, 0, neighborCenter.y);

                Debug.DrawLine(currentCenterFix, neighborCenterFix, Color.green);   
            }

    }

}
