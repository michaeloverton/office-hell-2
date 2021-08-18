using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringObjects : MonoBehaviour
{
    public List<GameObject> flickeringObjects;
    public int flickerProbability;

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject obj in flickeringObjects) {
            if(Random.Range(0, 100) < flickerProbability) {
                obj.SetActive(false);
            } else {
                obj.SetActive(true);
            }
        }
    }
}
