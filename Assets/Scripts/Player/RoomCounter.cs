using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCounter : MonoBehaviour
{
    private int roomCount = 0;

    public void incrementRoomCount() {
        roomCount++;
    }

    public int getRoomCount() {
        return roomCount;
    }
}
