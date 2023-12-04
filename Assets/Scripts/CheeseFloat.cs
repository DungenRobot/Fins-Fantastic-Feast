using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseFloat : MonoBehaviour{

    public float dist;//the distance away from the center that the cheese moves
    public float length; //the time in seconds it takes to complete a cycle
    private Transform selfPos;
    private float offset=0;//used internally to determine offset
    private float T = 0;//used to determine how much time has elapsed
    private float original;//the original position
    void Start(){
        selfPos = GetComponent<Transform>();
        original = selfPos.position.y;
    }

    // Update is called once per frame
    void Update(){
        T += Time.deltaTime;
        offset = Mathf.Sin(2*Mathf.PI*T/length) * dist;
        Vector3 nextPos = new Vector3 {
            x = selfPos.position.x,
            y = original+offset,
            z = selfPos.position.z
        };
        selfPos.position = nextPos;
    }
}
