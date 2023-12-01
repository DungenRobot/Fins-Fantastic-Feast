using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebDisplay : MonoBehaviour
{
    private Transform selfPos;
    private float anchor; //place to stretch from
    private float spider; //where the spider is
    void Start(){
        selfPos = GetComponent<Transform>();
        anchor = selfPos.parent.position.y + (selfPos.parent.localScale.y / 2);
        spider = selfPos.parent.position.y;
    }
    private void Update() {
        spider = selfPos.parent.position.y;
        Vector3 nextPosition = new Vector3 {
            x = 0,
            y = -(spider-anchor)/2,
            z = selfPos.position.z
        };
        Vector3 nextScale = new Vector3{
            x = selfPos.localScale.x,
            y = spider-anchor,
            z = selfPos.localScale.z
        };
        selfPos.SetLocalPositionAndRotation(nextPosition, selfPos.rotation);
        selfPos.localScale = nextScale;
    }
}
