using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> startRooms;
    public List<GameObject> rooms;

    void Start()
    {
        // List<GameObject> allRooms = new List<GameObject>();

        // Create start room.
        int startRoomIndex = Random.Range(0, startRooms.Count);
        GameObject start = Instantiate(startRooms[startRoomIndex], new Vector3(0,0,0), Quaternion.identity);
        start.GetComponent<Room>().setStartRoom(true);
        // allRooms.Add(start);
        
        // Create player at starting point in start room.
        Instantiate(player, start.GetComponent<Room>().playerStart.position, Quaternion.Euler(0, 180, 0));

        // Get all connections leaving start room.
        List<GameObject> connections = start.GetComponent<Room>().connections;

        // Use the queue for breadth first creation.
        Queue<GameObject> allExits = new Queue<GameObject>();
        foreach(GameObject connection in connections) {
            allExits.Enqueue(connection);
        }

        while(allExits.Count != 0) {
            // NewRoom:

            GameObject exit = allExits.Dequeue();
            Vector3 exitPosition = exit.GetComponent<Connection>().connectionPoint.position;
            Vector3 exitNormal = exit.GetComponent<Connection>().exitPlane.transform.up;

            // We will track which room indexes we have already tried to build.
            List<int> alreadyAttemptedRoomIndexes = new List<int>();

            // CreateRoom:

            // Instantiate a random room.
            GameObject roomToBuild = chooseRoomToBuild(alreadyAttemptedRoomIndexes);
            GameObject room = Instantiate(roomToBuild, new Vector3(0,0,0), Quaternion.identity);

            // Store the list of connection indexes we have tried to use as an entrance.
            List<int> alreadyAttemptedEntranceIndexes = new List<int>();

            // ChooseEntrance:
            
            // Ensure that room position is at origin before we move it into position.
            // It may have moved from origin if we have previously tried a different connection as entrance.
            room.transform.position = new Vector3(0,0,0);
            room.transform.rotation = Quaternion.identity;

            List<GameObject> roomConnections = room.GetComponent<Room>().connections;
            int entranceConnectionIndex = chooseEntranceIndexToUse(alreadyAttemptedEntranceIndexes, roomConnections);
            GameObject entranceConnection = roomConnections[entranceConnectionIndex];

            // Translate room into position.
            Vector3 entrancePosition = entranceConnection.GetComponent<Connection>().connectionPoint.position;
            room.transform.Translate(exitPosition - entrancePosition, Space.Self);

            // Rotate the room by rotating the angle between previous exit normal and new entrance normal.
            Vector3 newEntrancePosition = room.GetComponent<Room>().connections[entranceConnectionIndex].GetComponent<Connection>().connectionPoint.position;
            Vector3 newEntranceNormal = room.GetComponent<Room>().connections[entranceConnectionIndex].GetComponent<Connection>().entrancePlane.transform.up;
            float rotation = Vector3.SignedAngle(newEntranceNormal, exitNormal, Vector3.up);
            room.transform.RotateAround(newEntrancePosition, Vector3.up, rotation);

            // Set start room as parent of this room.
            room.GetComponent<Room>().setParentRoom(start);
            // Set this room as a child of the parent room.
            start.GetComponent<Room>().addChildRoom(room);
        }

        // Finally, disable all the bounding boxes we used for construction.
        // foreach(GameObject builtRoom in allRooms) {
        //     foreach(Renderer boundingBox in builtRoom.GetComponent<RoomConnections>().boundingBoxes) {
        //         boundingBox.enabled = false;
        //     }
        // }

        // Debug.Log(allRooms.Count + " rooms created");
    }

    GameObject chooseRoomToBuild(List<int> alreadyAttemptedRoomIndexes) {
        // Construct the list of rooms we can build, omitting rooms we have already attempted.
        List<GameObject> buildableRooms = new List<GameObject>();
        for(int i=0; i<rooms.Count; i++) {
            if(!alreadyAttemptedRoomIndexes.Contains(i)) {
                buildableRooms.Add(rooms[i]);
            }
        }

        int roomIndex = Random.Range(0, buildableRooms.Count);
        GameObject roomToBuild = buildableRooms[roomIndex];

        // Add the room index we will try to build to the list of attempted room indexes.
        alreadyAttemptedRoomIndexes.Add(roomIndex);

        return roomToBuild;
    }

    int chooseEntranceIndexToUse(List<int> alreadyAttemptedEntranceIndexes, List<GameObject> roomConnections) {
        // Add all indexes to the list of possible indexes.
        List<int> possibleIndexes = new List<int>();
        for(int i=0; i < roomConnections.Count; i++) {
            possibleIndexes.Add(i);
        }

        // Remove indexes that we have already tried.
        foreach(int alreadyAttemptedIndex in alreadyAttemptedEntranceIndexes) {
            possibleIndexes.Remove(alreadyAttemptedIndex);
        }

        // Choose a random one from the remaining options.
        int entranceConnectionIndex = possibleIndexes[Random.Range(0, possibleIndexes.Count)];

        alreadyAttemptedEntranceIndexes.Add(entranceConnectionIndex);

        return entranceConnectionIndex;
    }

    // void createDebugSphere(Vector3 pos) {
    //     if(debugMode) {
    //         GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //         sphere.transform.position = pos;
    //         sphere.transform.localScale = new Vector3(2,2,2);
    //     }
    // }

    // void createDebugCube(Vector3 pos) {
    //     if(debugMode) {
    //         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //         cube.transform.position = pos;
    //         cube.transform.localScale = new Vector3(2,2,2);
    //     }
    // }

    // void drawDebugRenderer(Renderer rend) {
    //     if(debugMode) {
    //         Instantiate(rend, rend.transform.position, rend.transform.rotation);
    //     }
    // }

    // BELOW WORKS WITH CRUDE NON-INTERSECTION ALGORITHM (NO INTELLIGENT RETRYING)

    // void Start()
    // {
    //     List<GameObject> allRooms = new List<GameObject>();

    //     // Create start room.
    //     int startRoomIndex = Random.Range(0, startRooms.Count);
    //     GameObject start = Instantiate(startRooms[startRoomIndex], new Vector3(0,0,0), Quaternion.identity);
    //     allRooms.Add(start);
        
    //     // Create player at starting point in start room.
    //     Instantiate(player, start.GetComponent<RoomConnections>().playerStart.position, Quaternion.Euler(0, 180, 0));

    //     // Get all connections leaving start room.
    //     List<GameObject> connections = start.GetComponent<RoomConnections>().connections;

    //     // Use the queue for breadth first creation.
    //     Queue<GameObject> allExits = new Queue<GameObject>();
    //     foreach(GameObject connection in connections) {
    //         allExits.Enqueue(connection);
    //     }

    //     int currentCreationAttempts = 0;
    //     while(currentRooms <= maxRooms) {
    //         NewRoom:

    //         Debug.Log("creating new room: " + currentRooms);

    //         // Get the exit of the room we built previously, if any exits exist in queue.
    //         if(allExits.Count == 0) {
    //             Debug.Log("no exits to dequeue. ending");
    //             break;
    //         }
    //         GameObject exit = allExits.Dequeue();
    //         Vector3 exitPosition = exit.GetComponent<Connection>().connectionPoint.position;
    //         Vector3 exitNormal = exit.GetComponent<Connection>().exitPlane.transform.up;

    //         CreateRoom:
    //         currentCreationAttempts++;

    //         // Get the next room we will build.
    //         int roomIndex = Random.Range(0, rooms.Count);
    //         GameObject roomToBuild = rooms[roomIndex];
    //         Debug.Log("attempting to build room, index: " + roomIndex);

    //         // Instantiate room.
    //         GameObject room = Instantiate(roomToBuild, new Vector3(0,0,0), Quaternion.identity);

    //         // Randomly choose the connection that we will use as an entrance to this room.
    //         List<GameObject> roomConnections = room.GetComponent<RoomConnections>().connections;
    //         int entranceConnectionIndex = Random.Range(0, roomConnections.Count);
    //         GameObject entranceConnection = roomConnections[entranceConnectionIndex];

    //         // Translate room into position.
    //         Vector3 entrancePosition = entranceConnection.GetComponent<Connection>().connectionPoint.position;
    //         room.transform.Translate(exitPosition - entrancePosition, Space.Self);

    //         // Rotate the room by rotating the angle between previous exit normal and new entrance normal.
    //         Vector3 newEntrancePosition = room.GetComponent<RoomConnections>().connections[entranceConnectionIndex].GetComponent<Connection>().connectionPoint.position;
    //         Vector3 newEntranceNormal = room.GetComponent<RoomConnections>().connections[entranceConnectionIndex].GetComponent<Connection>().entrancePlane.transform.up;
    //         float rotation = Vector3.SignedAngle(newEntranceNormal, exitNormal, Vector3.up);
    //         room.transform.RotateAround(newEntrancePosition, Vector3.up, rotation);

    //         // Check if new room intersects any previous rooms.
    //         foreach(GameObject prevRoom in allRooms) {

    //             // Check all bounding boxes for collisions;
    //             foreach(Renderer roomBoundingBox in room.GetComponent<RoomConnections>().boundingBoxes) {
    //                 foreach(Renderer prevRoomBoundingBox in prevRoom.GetComponent<RoomConnections>().boundingBoxes) {
    //                     Bounds roomBounds = roomBoundingBox.bounds;
    //                     Bounds prevRoomBounds = prevRoomBoundingBox.bounds;

    //                     if(roomBounds.Intersects(prevRoomBounds)) {
    //                         Destroy(room);
    //                         if(currentCreationAttempts <= maxRoomCreationAttempts) {
    //                             Debug.Log("room intersected. trying again.");
    //                             goto CreateRoom;
    //                         } else {
    //                             Debug.Log("max room creation attempts reached");
    //                             currentCreationAttempts = 0;
    //                             goto NewRoom;
    //                         }
    //                     }
    //                 }
    //             }
                
    //         }
            
    //         // Room was successfully created and does not intersect.
    //         currentRooms++;
    //         allRooms.Add(room);
    //         currentCreationAttempts = 0; // Reset.

    //         // Add all non-entrance connections into the exit queue.
    //         for(int i=0; i < roomConnections.Count; i++) {
    //             if(i != entranceConnectionIndex) {
    //                 allExits.Enqueue(roomConnections[i]);
    //             }
    //         }
    //     }

    //     // Finally, disable all the bounding boxes we used for construction.
    //     foreach(GameObject builtRoom in allRooms) {
    //         foreach(Renderer boundingBox in builtRoom.GetComponent<RoomConnections>().boundingBoxes) {
    //             boundingBox.enabled = false;
    //         }
    //     }

    //     Debug.Log(allRooms.Count + " rooms created");
    // }
}

