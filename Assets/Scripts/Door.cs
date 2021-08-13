using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Room room;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        // If we are touching the door's collider, but we are NOT in the room the door
        // is associated with, we are outside the room and entering into it.
        // In this case, disable the door so we don't get double doors.
        if(!room.isInRoom()) {
            Debug.Log("disabling door!");
            gameObject.SetActive(false);
            return;
        }
        
        open();
    }

    void OnTriggerExit(Collider other) {
        close();
    }

    void open() {
        this.transform.Rotate(0, 90, 0, Space.Self);
    }

    public void close() {
        this.transform.Rotate(0, -90, 0, Space.Self);
    }
}
