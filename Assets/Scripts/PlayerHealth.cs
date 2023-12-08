using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private AudioSource CollectSoundEffect;
    [SerializeField] private AudioSource HurtSoundEffect;
    public int cheese = 0; //how much cheese the player has
    public float iTime; //how long the player is invulnerable in seconds
    public bool isInv = false;//used to determine whether or not player is invulnerable

    public TMP_Text cheeseLabel;
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
        display.color = Color.gray;
    }
    void getCheese(int amt) 
    {
        cheese+=amt; 
        cheeseLabel.text = cheese.ToString();
        CollectSoundEffect.Play(); //Bridge puts Audio
    }

    void takeDamage(GameObject Source) {
        if (isInv) return; //invulnerability guard clause
        if (cheese == 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else {
            cheese--;
            inv = iTime;
            isInv = true;
        };
        cheeseLabel.text = cheese.ToString();
        HurtSoundEffect.Play();
    }
}
