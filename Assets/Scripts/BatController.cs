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
    public GameObject target;
    private float timer;//used for waiting
    private float between = 0;//used for LERPing
    private float LPS1, LPS2, LPS3; //used for LERPing between points in the path traveled
    private batState batMode = batState.SLEEPING; //used for saving what state the bat is in
    public GameObject Sprite;
    private Animator anim;
    private Transform spritePos;
    [SerializeField] private AudioSource SwooshSoundEffect;

    void Start(){
        //getting components of Sprite Object
        anim = Sprite.GetComponent<Animator>();
        spritePos = Sprite.GetComponent<Transform>();
        //defining positions
        selfPos = GetComponent<Transform>();
        startPos = selfPos.position;
        endPos = startPos;
        BoxCollider2D scaler = target.GetComponent<BoxCollider2D>();
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
        LPS3 = unitsPerSecond / dist3;
        //setting other things
        timer = delay;
    }

    // Update is called once per frame
    void Update(){
        switch (batMode) {
            case batState.SLEEPING: //skips entire update if bat is doing nothing
            case batState.SLEEPING_REVERSED:
                anim.SetBool("Moving", false);
                break;
            case batState.WAITING:
            case batState.WAITING_REVERSED:
                anim.SetBool("Moving", false);
                behavior_WAITING();
                break;
            case batState.DESCENDING:
                anim.SetBool("Moving", true);
                behavior_DESCENDING();
                break;
            case batState.ATTACKING:
            case batState.ATTACKING_REVERSED:
                anim.SetBool("Moving", true);
                behavior_ATTACKING();
                break;
            case batState.ASCENDING:
                anim.SetBool("Moving", true);
                behavior_ASCENDING();
                break;
            case batState.DESCENDING_REVERSED:
                anim.SetBool("Moving", true);
                behavior_DESCENDING_REVERSED();
                break;
            case batState.ASCENDING_REVERSED:
                anim.SetBool("Moving", true);
                behavior_ASCENDING_REVERSED();
                break;
        }
    }

    private void behavior_WAITING() {
        if (timer <= 0) {
            timer = delay;
            batMode = (batMode == batState.WAITING) ? batState.DESCENDING : batState.DESCENDING_REVERSED;
            //Bridge
            SwooshSoundEffect.Play();
        } else {
            between = 0;
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
        updateRotation(nextPos);
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
        updateRotation(nextPos);
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
            nextPos = (batMode == batState.ATTACKING) ? targetPos2 : targetPos1;
            batMode = (batMode == batState.ATTACKING) ? batState.ASCENDING : batState.ASCENDING_REVERSED;
        }
        updateRotation(nextPos);
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
            updateRotation(nextPos);
            selfPos.position = nextPos;
        } else {
            goToSleep();
        }
    }

    private void behavior_ASCENDING_REVERSED() {
        between += LPS1 * Time.deltaTime;
        Vector3 nextPos;
        if (between < 1) {
            nextPos = new Vector3 {
                x = Mathf.Lerp(targetPos1.x, startPos.x, between),
                y = Mathf.Lerp(targetPos1.y, startPos.y, LerpSmooth(between)),
                z = targetPos1.z
            };
            updateRotation(nextPos);
            selfPos.position = nextPos;
        } else {
            goToSleep();
        }
    }

    private void goToSleep() {
        Transform p = target.GetComponent<Transform>();
        p.localPosition = new Vector3 {
            x = -p.localPosition.x,
            y = p.localPosition.y,
            z = p.localPosition.z
        };
        between = 0;
        if (batMode == batState.ASCENDING) {
            selfPos.position = endPos;
            batMode = batState.SLEEPING_REVERSED;
        } else {
            selfPos.position = startPos;
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

    private void updateRotation(Vector3 nextPos) {
        float modifier = -Mathf.PI / 4;
        float xDif = nextPos.x - selfPos.position.x;
        float yDif = nextPos.y - selfPos.position.y;
        float angle = (Mathf.Atan2(yDif, xDif) + modifier) * Mathf.Rad2Deg;
        spritePos.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
