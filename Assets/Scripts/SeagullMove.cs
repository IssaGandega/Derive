using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SeagullMove : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -70 || transform.position.x > 70 || transform.position.z < -70 || transform.position.z > 70)
        {
            transform.position = new Vector3(Random.Range(-70, -50), transform.position.y, Random.Range(-70, 70));
            transform.Rotate(new Vector3(0, Random.Range(0, 180), 0));
        }
        gameObject.transform.position += transform.forward / 4;
    }
}
