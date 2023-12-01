using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{

    public GameObject target;
    public float x_offset;
    public float y_pos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 new_pos = transform.position;

        new_pos.y = y_pos;

        if (transform.position.x < target.transform.position.x + x_offset)
        {
            new_pos.x = target.transform.position.x + x_offset;
        }

        transform.position = new_pos;
    }
}
