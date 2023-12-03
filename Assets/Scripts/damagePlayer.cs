using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagePlayer : MonoBehaviour
{
    private Collider2D hit;
    public uint amount = 1;

    // Start is called before the first frame update
    void Start(){
        hit = GetComponent<Collider2D>();
    }

	private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            collision.SendMessage("takeDamage",amount);
            SendMessageUpwards("damageDealt",null,SendMessageOptions.DontRequireReceiver);
        }
	}
}
