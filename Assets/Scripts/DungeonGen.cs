using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGen : MonoBehaviour
{

    // We wnat to make a grid then subdivide with binary space partitioning
    [SerializeField] private int seed = 0; // seed for random number generation;

    [SerializeField] private int desiredRooms = 10; // maximum number of rooms to generate in the dungeon
    
    private System.Random rng; // for generating random numbers, we will use this instead of Unity's Random class to make it deterministic based on the seed


    [SerializeField] private int dungeonWidth = 50;

    [SerializeField] private int dungeonHeight = 50;

[   SerializeField] private int maxRoomSize = 30; // 30x30

    [SerializeField] private int minRoomSize = 5; // 5x5
    
    [SerializeField] private bool drawOnStart = true; 

    [SerializeField] private bool drawDebug = true; 

    [SerializeField] float stepDelay = 0.1f;
    
    [SerializeField] private int roomCount = 0; 

    private List<DungeonRoom> completedRooms = new List<DungeonRoom>();

    private List<DungeonRoom> eligibleToSplit = new List<DungeonRoom>();

    private int maxRoomLoop = 1000; // maximum number of iterations to prevent infinite loops

    private bool completedGeneration = false; 


    void Start()
    {
        seed = seed == 0 ? Random.Range(1, int.MaxValue) : seed; // in case the user hasn't specified a seed yet
        rng = new System.Random(seed); // initialize the random number generator with the seed

        if (drawOnStart)StartCoroutine(DivideSpaces(stepDelay));
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.R)) //Random newGen
        {
            completedGeneration = false;
            seed = Random.Range(1, int.MaxValue);
            rng = new System.Random(seed);
            ResetDungeon();
            StartCoroutine(DivideSpaces(stepDelay));
        }

        if ( Input.GetKeyDown(KeyCode.Space)) //Replay currentGen
        {
            completedGeneration = false;
            rng = new System.Random(seed);
            ResetDungeon();
            StartCoroutine(DivideSpaces(stepDelay));
        }
        
    }


    void ResetDungeon()
    {
        completedRooms.Clear();
    }

    void RemoveRoomFromList(DungeonRoom room)
    {
        completedRooms.Remove(room);
    }

    void PushRoomToList(DungeonRoom room)
    {
        completedRooms.Add(room);
    }

    DungeonRoom SpawnRoom(Vector2Int position, Vector2Int size)
    {
        DungeonRoom newRoom = new DungeonRoom(position.x, position.y, size.x, size.y);
        
        if (newRoom.CheckSplitability(minRoomSize))
           eligibleToSplit.Add(newRoom); 
        else
            PushRoomToList(newRoom);

        return newRoom;
    }

    IEnumerator DivideSpaces(float delay = 0f) 
    {
        Debug.Log("Dividing spaces with seed: " + seed);
        bool isHeightPartition = false;
        int split;
        bool canSplit = false;

        int loopCount = 0;

        SpawnRoom(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonHeight)); // spawn the initial room that fills the entire dungeon space
        int currentRoomIndex = rng.Next(eligibleToSplit.Count);


        while (completedRooms.Count < desiredRooms)
        {
            loopCount++;
            if (loopCount > maxRoomLoop)
            {
                Debug.LogError("Maximum room generation iterations reached.");
                completedGeneration = true;
                eligibleToSplit.Clear();
                break;
            }

            DungeonRoom currentRoom = eligibleToSplit[currentRoomIndex];
            split = 0;

            if (isHeightPartition && currentRoom.CanSplitHorizontally(minRoomSize))
            {
                split = rng.Next(minRoomSize, currentRoom.GetHeight() - minRoomSize);
                if (split < minRoomSize || split > currentRoom.GetHeight() - minRoomSize)
                {
                    Debug.LogWarning("Split value is out of bounds: " + split);
                    continue; // skip this iteration if the split value is out of bounds
                }
                canSplit = true;
            }

            if (!isHeightPartition && currentRoom.CanSplitVertically(minRoomSize))
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
                Debug.Log(eligibleToSplit.Count + " " + "Splitting room at index " + currentRoomIndex + " with split value: " + split + " isHeightPartition: " + isHeightPartition);

                DungeonRoom newRoom; 
                if (isHeightPartition)
                {
                    newRoom = SpawnRoom(currentRoom.SplitHorizontally(split),
                                        new Vector2Int(currentRoom.GetWidth() - split, currentRoom.GetHeight())
                                        );
                }
                else
                {
                    newRoom = SpawnRoom(currentRoom.SplitVertically(split),
                                        new Vector2Int(currentRoom.GetWidth(), currentRoom.GetHeight() - split)
                                        );
                }


                PushRoomToList(newRoom);
                canSplit = false; 
                isHeightPartition = !isHeightPartition;
                currentRoomIndex = rng.Next(eligibleToSplit.Count);
                yield return new WaitForSeconds(delay);
            }
        }

        roomCount = completedRooms.Count;        
        completedGeneration = true;
        eligibleToSplit.Clear(); 
        yield return null;
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
            if (completedGeneration)
            {
                foreach (DungeonRoom room in completedRooms)
                {
                    room.OnDrawGizmos();
                }
            } else
            {
                foreach (DungeonRoom room in eligibleToSplit)
                {
                    room.OnDrawGizmos();
                }
            }
        }
    }
}
