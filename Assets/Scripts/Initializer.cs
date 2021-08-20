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
        // Create start room.
        int startRoomIndex = Random.Range(0, startRooms.Count);
        GameObject start = Instantiate(startRooms[startRoomIndex], new Vector3(0,0,0), Quaternion.identity);
        start.GetComponent<Room>().setStartRoom(true);
        
        // Create player at starting point in start room.
        // Instantiate(player, start.GetComponent<Room>().playerStart.position, start.GetComponent<Room>().playerStart.rotation);

        // Get all connections leaving start room.
        List<GameObject> connections = start.GetComponent<Room>().connections;

        // Use the queue for breadth first creation.
        Queue<GameObject> allExits = new Queue<GameObject>();
        foreach(GameObject connection in connections) {
            allExits.Enqueue(connection);
        }

        while(allExits.Count != 0) {
            GameObject exit = allExits.Dequeue();
            Vector3 exitPosition = exit.GetComponent<Connection>().connectionPoint.position;
            Vector3 exitNormal = exit.GetComponent<Connection>().exitPlane.transform.up;

            // We will track which room indexes we have already tried to build.
            List<int> alreadyAttemptedRoomIndexes = new List<int>();

            // Instantiate a random room.
            GameObject roomToBuild = chooseRoomToBuild(alreadyAttemptedRoomIndexes);
            GameObject room = Instantiate(roomToBuild, new Vector3(0,0,0), Quaternion.identity);

            // Store the list of connection indexes we have tried to use as an entrance.
            List<int> alreadyAttemptedEntranceIndexes = new List<int>();
            
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
}

