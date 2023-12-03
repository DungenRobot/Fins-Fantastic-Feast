using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{

    public int GoodScore = 1;
    public int PerfectScore = 16;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            int cheese = collider.gameObject.GetComponent<PlayerHealth>().cheese;

            if (cheese >= PerfectScore)
            {
                SceneManager.LoadScene("Good");
            }
            else if (cheese >= GoodScore)
            {
                SceneManager.LoadScene("Mid");
            } 
            else
            {
                SceneManager.LoadScene("Bad");
            }
        }
    }
}
