using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairMoveDown : MonoBehaviour
{
    public GameObject stairwell;
    public bool up; // If up, move up. Else move down.

    // On entering the up or down trigger, move the stairwell up or down.
    void OnTriggerEnter(Collider other) {
        if(up) {
            Debug.Log("moving up");
            stairwell.transform.Translate(0, 4, 0, Space.World);
        } else {
            Debug.Log("moving down");
            stairwell.transform.Translate(0, -4, 0, Space.World);
        }

        // After moving stairwell, destroy previously attached room.
        StartCoroutine(destroyChildRooms());
    }

    IEnumerator destroyChildRooms() {
        yield return null;
        stairwell.GetComponent<Room>().destroyChildRooms();
    }
}
