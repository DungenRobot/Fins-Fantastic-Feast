using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public int unitsPerSecond=5;
    private bool isDescending = false;
    private float lerpPerSecond;
    private float between=0;
    private float startPosition;
    private float endPosition;
    void Start(){
        //determining the spider's movement
        startPosition = transform.position.y;
        BoxCollider2D container = GetComponentInChildren<BoxCollider2D>();
        CircleCollider2D body = GetComponent<CircleCollider2D>();
        float distToTravel = container.size.y - (body.radius * 2);
        endPosition = startPosition;
        endPosition -= distToTravel;
        lerpPerSecond = distToTravel / unitsPerSecond;
    }

	private void Update() {
        if (isDescending) between += lerpPerSecond;
        else between -= lerpPerSecond;
        Mathf.Clamp(between, 0, 1);
        
	}

	void PlayerEnter() {
        isDescending = true;
    }

    void PlayerExit() {
        isDescending = false;
    }
}
