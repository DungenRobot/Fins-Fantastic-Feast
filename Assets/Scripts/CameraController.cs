using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{

    public GameObject target;
    public float xOffset;
    public float yPos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 new_pos = transform.position;

        new_pos.y = yPos;

        if (transform.position.x < target.transform.position.x + xOffset)
        {
            new_pos.x = target.transform.position.x + xOffset;
        }

        transform.position = new_pos;
    }
}
