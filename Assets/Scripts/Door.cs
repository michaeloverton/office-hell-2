using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Room room;
    public Collider doorCollider;

    void OnTriggerEnter(Collider other) {
        // If we are touching the door's collider, but we are NOT in the room the door
        // is associated with, we are outside the room and entering into it.
        // In this case, disable the door so we don't get double doors.
        if(!room.isInRoom()) {
            gameObject.SetActive(false);
            return;
        }
        
        open();
    }

    void OnTriggerExit(Collider other) {
        close();
    }

    void open() {
        doorCollider.enabled = false;
        this.transform.Rotate(0, 90, 0, Space.Self);
    }

    public void close() {
        doorCollider.enabled = true;
        this.transform.Rotate(0, -90, 0, Space.Self);
    }
}
