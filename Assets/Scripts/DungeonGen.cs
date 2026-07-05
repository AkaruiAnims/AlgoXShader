using UnityEngine;
using System.Collections.Generic;

public class DungeonGen : MonoBehaviour
{

    // We wnat to make a grid then subdivide with binary space partitioning
    [SerializeField] private int seed = 0; // seed for random number generation;

    [SerializeField] private int desiredRooms = 10; // maximum number of rooms to generate in the dungeon
    
    private System.Random rng; // for generating random numbers, we will use this instead of Unity's Random class to make it deterministic based on the seed

    [SerializeField] private int dungeonWidth = 50;
    [SerializeField] private int dungeonHeight = 50;

    [SerializeField] private int minRoomSize = 5; // 5x5
    
    [SerializeField] private bool drawOnStart = true; 

    [SerializeField] private bool drawDebug = true; 

    [SerializeField] private bool regenerateRooms = false;
    
    [SerializeField] private int roomCount = 0; 

    private List<DungeonRoom> rooms = new List<DungeonRoom>();



    void Start()
    {
        seed = seed == 0 ? Random.Range(1, int.MaxValue) : seed; // in case the user hasn't specified a seed yet
        rng = new System.Random(seed); // initialize the random number generator with the seed

        if (drawOnStart)
        {
            DivideSpaces();
        }
    }

    void Update()
    {
        if (regenerateRooms)
        {
            ResetDungeon();
            DivideSpaces();
            regenerateRooms = false;
        }
        
    }

    void ResetDungeon()
    {
        rooms.Clear();
    }

    void RemoveRoomFromList(DungeonRoom room)
    {
        rooms.Remove(room);
    }

    void PushRoomToList(DungeonRoom room)
    {
        rooms.Add(room);
    }

    void SpawnRoom(Vector2Int position, Vector2Int size)
    {
        PushRoomToList(new DungeonRoom(position.x, position.y, size.x, size.y)); // spawn the room in the dungeon
    }

    void DivideSpaces()
    {
        Debug.Log("Dividing spaces with seed: " + seed);
        bool isHeightPartition = false;
        
        SpawnRoom(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonHeight)); // spawn the initial room that fills the entire dungeon space

        while (rooms.Count < desiredRooms)
        {
            int currentRoomIndex = rng.Next(rooms.Count);
            int split;

            //Storing data
            DungeonRoom currentRoom = rooms[currentRoomIndex];
            DungeonRoom newRoom = new DungeonRoom(0,0,0,0);


            if (isHeightPartition)
            {
                split = rng.Next(minRoomSize, rooms[currentRoomIndex].GetHeight() - minRoomSize);
                if (split < minRoomSize || split > rooms[currentRoomIndex].GetHeight() - minRoomSize)
                {
                    Debug.LogWarning("Split value is out of bounds: " + split);
                    continue; // skip this iteration if the split value is out of bounds
                }

                // divide the space vertically
                newRoom.SetHeight( rooms[currentRoomIndex].GetHeight() - split ); // divide the height by a random number between 2 and 4
                newRoom.SetWidth( rooms[currentRoomIndex].GetWidth() ); // keep the width the same
                newRoom.SetPos( new Vector2Int( rooms[currentRoomIndex].GetPos().x, rooms[currentRoomIndex].GetPos().y + split ) ); 

                currentRoom.SetHeight( split ); // reduce the height of the current room to make space for the new room
            }
            else
            {
                split = rng.Next(minRoomSize, rooms[currentRoomIndex].GetWidth() - minRoomSize);
                if (split < minRoomSize || split > rooms[currentRoomIndex].GetWidth() - minRoomSize)
                {
                    Debug.LogWarning("Split value is out of bounds: " + split);
                    continue; // skip this iteration if the split value is out of bounds
                }

                // divide the space horizontally
                newRoom.SetWidth( rooms[currentRoomIndex].GetWidth() - split ); // divide the width by a random number between 2 and 4
                newRoom.SetHeight( rooms[currentRoomIndex].GetHeight() ); // keep the height the same
                newRoom.SetPos( new Vector2Int( rooms[currentRoomIndex].GetPos().x + split, rooms[currentRoomIndex].GetPos().y ) );

                currentRoom.SetWidth( split ); // reduce the width of the current room to make space for the new room
            }

            RemoveRoomFromList(rooms[currentRoomIndex]); 
            PushRoomToList(currentRoom); 
            PushRoomToList(newRoom);

            isHeightPartition = !isHeightPartition;
        }

        roomCount = rooms.Count;        
    }

    void OnDrawGizmos() // for visual debugging of the dungeon generation
    {
        //Draw xyz
        Debug.DrawLine(Vector3.zero, Vector3.right, Color.red);
        Debug.DrawLine(Vector3.zero, Vector3.up, Color.green);
        Debug.DrawLine(Vector3.zero, Vector3.forward, Color.blue);

        RectInt a = new RectInt(0, 0, dungeonWidth, dungeonHeight); 
        
        AlgorithmsUtils.DebugRectInt(a, Color.red);

        if (drawDebug)
        {
            foreach (DungeonRoom room in rooms)
            {
                room.OnDrawGizmos();
            }
        }
    }
}
