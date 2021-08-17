using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicleRotator : MonoBehaviour
{
    public List<GameObject> cubicles;
    private List<List<float>> cubicleRotationAmounts = new List<List<float>>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject cube in cubicles) {
            List<float> rotationAmounts = new List<float>();
            bool positiveX = Random.Range(0f,1f) > 0.5f;
            rotationAmounts.Add(Random.Range(0f,1f) * (positiveX ? 1 : -1)); // x rotation
            bool positiveZ = Random.Range(0f,1f) > 0.5f;
            rotationAmounts.Add(Random.Range(0f,1f) * (positiveZ ? 1 : -1)); // z rotation
            cubicleRotationAmounts.Add(rotationAmounts);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < cubicles.Count; i++) {
            GameObject cubicle = cubicles[i];
            List<float> rotationAmounts = cubicleRotationAmounts[i];
            cubicle.transform.RotateAround(cubicle.transform.position + new Vector3(1,0,0), Vector3.left, rotationAmounts[0]);
            cubicle.transform.RotateAround(cubicle.transform.position + new Vector3(0, 0, 1), Vector3.forward, rotationAmounts[1]);
        }
    }
}
