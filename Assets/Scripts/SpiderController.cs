using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public int unitsPerSecond=2; // how fast the spider moves
    private bool isDescending = false; // to determine which direction the spider is moving
    private float lerpPerSecond; //used to determine where the spider should be
    private float between=0; //see above
    private float startPosition; //spider's maximum Y value
    private float endPosition; //spider's minimum Y value
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
        float frameMove = lerpPerSecond * Time.deltaTime;
        if (isDescending) {
            if (between < 1) {
                between = (between + frameMove < 1) ? between + frameMove : 1; //checks whether or not the spider has reached its maximum descent, and adjusts accordingly
            }
        } else {
            if (between > 0) {
                between = (between - frameMove > 0) ? between - frameMove : 0; //checks whether the spider is at max height while climbing, and adjusts accordingly
            }
        }
        Vector3 newPosition = new Vector3 {
            x = transform.position.x,
            y = Mathf.Lerp(startPosition, endPosition, between),
            z = transform.position.z
        };
        transform.SetPositionAndRotation(newPosition, transform.rotation);
	}

	void PlayerEnter() {
        isDescending = true;
    }

    void PlayerExit() {
        isDescending = false;
    }
}
