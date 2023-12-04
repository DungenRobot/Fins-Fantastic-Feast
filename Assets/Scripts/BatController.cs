using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour{
    private enum batState {
        SLEEPING,
        SLEEPING_REVERSED,
        WAITING,
        WAITING_REVERSED,
        DESCENDING,
        DESCENDING_REVERSED,
        ATTACKING,
        ATTACKING_REVERSED,
        ASCENDING,
        ASCENDING_REVERSED
    }; // used to determine what the bat should be doing
    private Vector3 startPos, targetPos1, targetPos2, endPos;//positions of the path
    private Transform selfPos;
    public float unitsPerSecond; //on average how fast the bat travels in units per second
    public float delay; //how many seconds from detection until attack
    private float timer;//used for waiting
    private float between = 0;//used for LERPing
    private float LPS1, LPS2, LPS3; //used for LERPing between points in the path traveled
    private batState batMode = batState.SLEEPING; //used for saving what state the bat is in
    void Start(){
        //defining positions
        selfPos = GetComponent<Transform>();
        startPos = selfPos.position;
        endPos = startPos;
        BoxCollider2D scaler = GetComponentInChildren<BoxCollider2D>();
        Transform hitPos = scaler.GetComponent<Transform>();
        endPos.x += (hitPos.position.x - startPos.x) * 2;
        targetPos1 = hitPos.position;
        targetPos2 = hitPos.position;
        targetPos1.x -= scaler.size.x / 2;
        targetPos2.x += scaler.size.x / 2;
        //getting path/speed info
        float dist1 = dist(startPos, targetPos1);
        float dist2 = dist(targetPos1,targetPos2);
        float dist3 = dist(targetPos2, endPos);
        LPS1 = unitsPerSecond / dist1;
        LPS2 = unitsPerSecond / dist2;
        LPS3 = unitsPerSecond/ dist3;
        //setting other things
        timer = delay;
    }

    // Update is called once per frame
    void Update(){
        switch (batMode) {
            case batState.SLEEPING: //skips entire update if bat is doing nothing
            case batState.SLEEPING_REVERSED:
                return;
            case batState.WAITING:
            case batState.WAITING_REVERSED:
                behavior_WAITING();
                break;
            case batState.DESCENDING:
                behavior_DESCENDING();
                break;
            case batState.ATTACKING:
            case batState.ATTACKING_REVERSED:
                behavior_ATTACKING();
                break;
            case batState.ASCENDING:
                behavior_ASCENDING();
                break;
            case batState.DESCENDING_REVERSED:
                behavior_DESCENDING_REVERSED();
                break;
            case batState.ASCENDING_REVERSED:
                behavior_ASCENDING_REVERSED();
                break;
        }
    }

    private void behavior_WAITING() {
        if (timer <= 0) {
            timer = delay;
            batMode = (batMode == batState.WAITING) ? batState.DESCENDING : batState.DESCENDING_REVERSED;
        } else {
            timer -= Time.deltaTime;
        }
    }

    private void behavior_DESCENDING() {
        between += LPS1 * Time.deltaTime;
        Vector3 nextPos;
        if (between < 1) {
            nextPos = new Vector3 {
                x = Mathf.Lerp(startPos.x, targetPos1.x, between),
                y = Mathf.Lerp(startPos.y, targetPos1.y, LerpSmooth(between)),
                z = startPos.z
            };
        } else {
            between = 0;
            nextPos = targetPos1;
            batMode = batState.ATTACKING;
        }
        selfPos.position = nextPos;
    }

    private void behavior_DESCENDING_REVERSED() {
        between += LPS3 * Time.deltaTime;
        Vector3 nextPos;
        if (between < 1) {
            nextPos = new Vector3 {
                x = Mathf.Lerp(endPos.x, targetPos2.x, between),
                y = Mathf.Lerp(endPos.y, targetPos2.y, LerpSmooth(between)),
                z = endPos.z
            };
        } else {
            between = 0;
            nextPos = startPos;
            batMode = batState.ATTACKING_REVERSED;
        }
        selfPos.position = nextPos;
    }

    private void behavior_ATTACKING() {
        between += LPS2 * Time.deltaTime;
        Vector3 nextPos;
        if (between < 1) {
            nextPos = new Vector3 {
                x = (batMode == batState.ATTACKING) ?
                    Mathf.Lerp(targetPos1.x, targetPos2.x, between):
                    Mathf.Lerp(targetPos2.x, targetPos1.x, between),
                y = targetPos1.y,
                z = targetPos1.z
            };
        } else {
            between = 0;
            nextPos = targetPos2;
            batMode = batState.ASCENDING;
        }
        selfPos.position = nextPos;
    }

    private void behavior_ASCENDING() {
        between += LPS3 * Time.deltaTime;
        Vector3 nextPos;
        if (between < 1) {
            nextPos = new Vector3 {
                x = Mathf.Lerp(targetPos2.x, endPos.x, between),
                y = Mathf.Lerp(targetPos2.y, endPos.y, LerpSmooth(between)),
                z = targetPos2.z
            };
        } else {
            between = 0;
            nextPos = endPos;
            batMode = batState.SLEEPING_REVERSED;
        }
        selfPos.position = nextPos;
    }

    private void behavior_ASCENDING_REVERSED() {
        between += LPS1 * Time.deltaTime;
        Vector3 nextPos;
        if (between < 0) {
            nextPos = new Vector3 {
                x = Mathf.Lerp(targetPos2.x, startPos.x, between),
                y = Mathf.Lerp(targetPos2.y, startPos.y, LerpSmooth(between)),
                z = targetPos2.z
            };
        } else {
            between = 0;
            nextPos = startPos;
            batMode = batState.SLEEPING;
        }
    }

    void PlayerDetected() {
        switch (batMode) {
            case batState.SLEEPING:
                batMode = batState.WAITING;
                break;
            case batState.SLEEPING_REVERSED:
                batMode = batState.WAITING_REVERSED;
                break;
        }
    }
    
    private float LerpSmooth(float value) {
        float rVal = Mathf.Clamp(value, 0, 1);
        switch (batMode) {
            case batState.DESCENDING:
            case batState.DESCENDING_REVERSED:
                return Mathf.Sin((Mathf.PI / 2) * value);
            case batState.ASCENDING:
            case batState.ASCENDING_REVERSED:
                return (1 - Mathf.Cos((Mathf.PI / 2) * value));
        }
        return value;
    }// used to make swooping path smoother

    private float dist(Vector3 v1, Vector3 v2) {//used my own because Vector3.distance wasn't giving correct results
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2) + Mathf.Pow(v1.z - v2.z, 2));
    }
}
