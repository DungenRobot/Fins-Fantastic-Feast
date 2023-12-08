using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatStomp : MonoBehaviour {
    private SpriteRenderer display;
    private Transform selfPosBase;
    private Collider2D collider;
    public float damageAmount;
    private void Start() {
        display = GetComponent<SpriteRenderer>();
        selfPosBase = GetComponent<Transform>();
        collider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag != "Player") return;
        Vector2 colPos = new Vector2 {
            x = collision.GetComponent<Transform>().position.x,
            y = collision.GetComponent<Transform>().position.y
        };
        Vector2 selfPos = new Vector2 {
            x = selfPosBase.position.x,
            y = selfPosBase.position.y
        };
        colPos -= selfPos;
        if (colPos.y >= 0 && Mathf.Abs(colPos.x) < 0.8) {
            display.enabled = false;
            collider.enabled = false;
            collision.SendMessage("bounce",gameObject);
        } else {
            collision.SendMessage("takeDamage", gameObject);
        }
    }
}
