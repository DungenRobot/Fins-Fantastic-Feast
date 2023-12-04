using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int cheese = 0;
    public float iTime; //how long the player is invulnerable in seconds
    private void Update() {
        
    }
    void getCheese(int amt) { cheese+=amt; }
    void takeDamage(uint amt) {
        if (cheese < amt) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else cheese--;
    }
}
