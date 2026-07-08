using System;
using UnityEngine;

public class RoomDoor
{

    private int width;
    private int height;

    private int xPos;
    private int yPos;

    private DungeonRoom[] connectedRooms;

    public RoomDoor(DungeonRoom roomA, DungeonRoom roomB)
    {
        connectedRooms = new DungeonRoom[2];        
        connectedRooms[0] = roomA;
        connectedRooms[1] = roomB;
    }

    public DungeonRoom returnOtherRoom(DungeonRoom currentRoom)
    {
        int otherRoom = filterForOtherRoom(currentRoom);
        
        if (otherRoom > 1) return null; // For strange edge-cases where there is no other room ?   

        return connectedRooms[otherRoom];
    }

    private int filterForOtherRoom(DungeonRoom currentRoom)
    {
        for(int i =0; i < connectedRooms.Length; i++)
        {
            if (connectedRooms[i] == currentRoom)
            {
                int otherRoom = Math.Abs(i-1); // can only be 0 or 1 so just flip them

                if (connectedRooms[otherRoom] != null) return otherRoom;
            }
        }   

        Debug.LogError("There is no other room in array");
        return 3; // Impossible to have one door connect to more than 2 rooms or if only one room in array
    }

}