using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour{
    
    private Transform selfPos;
    private Vector3[] path;
    private Vector3 moveFrom;
    private Vector3 moveTo;
    public float unitsPerSecond = 3;
    private float lerpPerSecond;
    private float between=0;
    private int nextIndex=2;
    private SpriteRenderer display;
    // Start is called before the first frame update
    void Start(){
        display = GetComponent<SpriteRenderer>();
        selfPos = GetComponent<Transform>();
        //path initialization
        path = new Vector3[selfPos.childCount + 1];
        path[0] = selfPos.position;
        for (int i = 0; i < path.Length-1; i++) path[i+1]=selfPos.GetChild(i).position;
        //position initialization
        moveTo = path[0];
        lerpReset(path[1]);
    }

    // Update is called once per frame
    void Update(){
        between += lerpPerSecond * Time.deltaTime;
        if (between >= 1) {
            //transitioning from moving to one point to the next
            selfPos.position = moveTo;
            if (nextIndex >= path.Length) {
                lerpReset(path[0]);
                nextIndex = 1;
            } else lerpReset(path[nextIndex++]);
        } else {
            //moving between points as normal
            Vector3 newPosition = new Vector3 {
                x = Mathf.Lerp(moveFrom.x, moveTo.x, between),
                y = Mathf.Lerp(moveFrom.y, moveTo.y, between),
                z = moveFrom.z
            };
            selfPos.SetPositionAndRotation(newPosition, selfPos.rotation);
        }
    }

    void lerpReset(Vector3 nextPos) {
        //sets next position to move towards
        display.flipX ^= (nextPos.x != selfPos.position.x);//<-- DO NOT REPLICATE. I AM A TRAINED DUMBASS WHO KNOWS THAT THIS IS A BAD IDEA
        moveFrom = moveTo;
        moveTo = nextPos;
        between = 0;
        float distance = Mathf.Sqrt( Mathf.Pow((moveFrom.x - moveTo.x), 2) + Mathf.Pow((moveFrom.y - moveTo.y), 2));
        lerpPerSecond = unitsPerSecond/distance;
    }
}
