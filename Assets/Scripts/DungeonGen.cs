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

    [SerializeField] private bool drawInSteps = false;
    
    [SerializeField] private int roomCount = 0; 

    private List<DungeonRoom> rooms = new List<DungeonRoom>();

    int maxRoomLoop = 1000; // maximum number of iterations to prevent infinite loops


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
            rng = new System.Random(seed);
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
        int split;
        bool canSplit = false;

        int currentRoomIndex = rng.Next(rooms.Count);
        int loopCount = 0;

        SpawnRoom(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonHeight)); // spawn the initial room that fills the entire dungeon space

        //Storing data
        DungeonRoom newRoom = new DungeonRoom(0,0,0,0);

        while (rooms.Count < desiredRooms)
        {
            loopCount++;
            if (loopCount > maxRoomLoop)
            {
                Debug.LogError("Maximum room generation iterations reached.");
                break;
            }

            DungeonRoom currentRoom = rooms[currentRoomIndex];
            split = 0;

            if (isHeightPartition && currentRoom.GetHeight() > minRoomSize * 2)
            {
                split = rng.Next(minRoomSize, currentRoom.GetHeight() - minRoomSize);
                if (split < minRoomSize || split > currentRoom.GetHeight() - minRoomSize)
                {
                    Debug.LogWarning("Split value is out of bounds: " + split);
                    continue; // skip this iteration if the split value is out of bounds
                }
                canSplit = true;
            }

            if (!isHeightPartition && currentRoom.GetWidth() > minRoomSize * 2)
            {
                split = rng.Next(minRoomSize, currentRoom.GetWidth() - minRoomSize);
                if (split < minRoomSize || split > currentRoom.GetWidth() - minRoomSize)
                {
                    Debug.LogWarning("Split value is out of bounds: " + split);
                    continue; // skip this iteration if the split value is out of bounds
                }
                canSplit = true;
            }

            if (canSplit)
            {
                Debug.Log(rooms.Count + " " + "Splitting room at index " + currentRoomIndex + " with split value: " + split + " isHeightPartition: " + isHeightPartition);
                if (isHeightPartition)
                {
                    newRoom = currentRoom.SplitVertically(split);
                }
                else
                {
                    newRoom = currentRoom.SplitHorizontally(split);
                }


                PushRoomToList(newRoom);
                canSplit = false; 
                isHeightPartition = !isHeightPartition;
                currentRoomIndex = rng.Next(rooms.Count);
            }
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
