using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCounter : MonoBehaviour
{
    public int startWeirdRoomsAfterRoom;
    public int weirdProbabilityIncrement;
    private int roomCount = 0;
    private int weirdRoomProbability = 0;

    public void incrementRoomCount() {
        roomCount++;

        // If rooms can become weird now, increment the weird room probability.
        if(roomCount > startWeirdRoomsAfterRoom && weirdRoomProbability < 100) {
            weirdRoomProbability += weirdProbabilityIncrement;
        }

        Debug.Log("room count: " + roomCount);
        Debug.Log("weird room probability: " + weirdRoomProbability);
    }

    public int getRoomCount() {
        return roomCount;
    }

    public bool nextRoomIsWeird() {
        return Random.Range(0, 100) < weirdRoomProbability;
    }
}
