using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheeseCollection : MonoBehaviour
{
    private SpriteRenderer display;
    private Collider2D hit;
    public int amount = 1;
    
    // Start is called before the first frame update
    void Start(){
        display = GetComponent<SpriteRenderer>();
        hit = GetComponent<Collider2D>();
    }

	private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            collision.SendMessage("getCheese",amount);
            display.enabled = false;
            hit.enabled = false;
        }
	}
}
