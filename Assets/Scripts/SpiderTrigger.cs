using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderVisionController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") SendMessageUpwards("PlayerEnter");
    }

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.tag == "Player") SendMessageUpwards("PlayerExit");
	}
}
