using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int cheese = 0; //how much cheese the player has
    public float iTime; //how long the player is invulnerable in seconds
    public bool isInv = false;//used to determine whether or not player is invulnerable
    private float inv = 0; //used internally for invulnerability
    private SpriteRenderer display;
    private void Start() {
        cheese = 0;
        isInv = false;
        display = GetComponent<SpriteRenderer>();
    }
    void Update() {
        inv = (inv - Time.deltaTime > 0) ? inv - Time.deltaTime : 0;
        if (inv == 0) {
            isInv = false;
            display.color = Color.white;
            return;
        }
        display.color = Color.red;
    }
    void getCheese(int amt) { cheese+=amt; }
    void takeDamage(uint amt) {
        if (isInv) return; //invulnerability guard clause
        if (cheese < amt) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else {
            cheese--;
            inv = iTime;
            isInv = true;
        };
    }
}
