using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room : MonoBehaviour
{
    public List<GameObject> connections;
    public List<GameObject> nextRooms;
    public Transform playerStart;
    // parentRoom is the room that this room was spawned from.
    GameObject parentRoom;
    List<GameObject> childRooms = new List<GameObject>();
    // isStartRoom indicates if this is the room we started in.
    bool isStartRoom = false;
    public List<Door> doors;
    bool inRoom = false;

    void OnTriggerEnter(Collider other) {
        Debug.Log("entered room: " + gameObject.name);
        if(!isStartRoom) {
            StartCoroutine(generateNextRoomLayer());
        }

        Debug.Log("passed coroutine");

        // Upon entering, we enable all doors.
        foreach(Door door in doors) {
            door.gameObject.SetActive(true);
        }

        // We have entered the room.
        inRoom = true;
    }

    public bool isInRoom() {
        return inRoom;
    }

    IEnumerator generateNextRoomLayer() {
        yield return null;
        
        // Delete children of parent room that are not our current room.
        foreach(GameObject child in parentRoom.GetComponent<Room>().childRooms) {
            if(child != gameObject) {
                Destroy(child);
                yield return null;
            }
        }
        // Then delete the parent room.
        Destroy(parentRoom);
        yield return null;

        // Get all connections leaving the room.
        // Use the queue for breadth first creation.
        Queue<GameObject> allExits = new Queue<GameObject>();
        foreach(GameObject connection in connections) {
            allExits.Enqueue(connection);
        }

        while(allExits.Count!= 0) {
            GameObject exit = allExits.Dequeue();
            Vector3 exitPosition = exit.GetComponent<Connection>().connectionPoint.position;
            Vector3 exitNormal = exit.GetComponent<Connection>().exitPlane.transform.up;

            // We will track which room indexes we have already tried to build.
            List<int> alreadyAttemptedRoomIndexes = new List<int>();

            // Instantiate a random room.
            GameObject roomToBuild = chooseRoomToBuild(alreadyAttemptedRoomIndexes);
            GameObject room = Instantiate(roomToBuild, new Vector3(0,0,0), Quaternion.identity);
            // Iniitally, the room should not be active, so we don't bump the player.
            room.SetActive(false);

            yield return null;

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

            yield return null;

            // Set the parent room of the new room.
            room.GetComponent<Room>().setParentRoom(gameObject);
            // Add new room to the list of children.
            childRooms.Add(room);
            room.SetActive(true);

            yield return null;

            // We do not need to check for collisions because they are impossible if we only add one layer of rooms.
        }

        Debug.Log("finished coroutine");
        yield return null;
    }

    GameObject chooseRoomToBuild(List<int> alreadyAttemptedRoomIndexes) {
        // Construct the list of rooms we can build, omitting rooms we have already attempted.
        List<GameObject> buildableRooms = new List<GameObject>();
        for(int i=0; i<nextRooms.Count; i++) {
            if(!alreadyAttemptedRoomIndexes.Contains(i)) {
                buildableRooms.Add(nextRooms[i]);
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

    public void setParentRoom(GameObject room) {
        parentRoom = room;
    }

    public GameObject getParentRoom() {
        return parentRoom;
    }

    public void destroyParentRoom() {
        Destroy(parentRoom);
    }

    public void addChildRoom(GameObject room) {
        childRooms.Add(room);
    }

    public void setStartRoom(bool isStart) {
        isStartRoom = isStart;
    }

}
