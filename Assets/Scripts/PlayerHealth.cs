using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int cheese = 0;

    void getCheese(int amt) { cheese+=amt; }
    void takeDamage(uint amt) {
        if (cheese < amt) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else cheese--;
    }
}
