using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    AudioSource source;

    public int value = 10;
    public string type;

    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.name == "Player"){
            if(type == "Health"){
                other.gameObject.GetComponent<Player>().health += value;
                if(other.gameObject.GetComponent<Player>().health > other.gameObject.GetComponent<Player>().initHealth){
                    other.gameObject.GetComponent<Player>().health = other.gameObject.GetComponent<Player>().initHealth;
                }
            }

            else if(type == "Shield"){
                other.gameObject.GetComponent<Player>().shield = true;
            }
        }

        else{
            if(type == "Health"){
                other.gameObject.GetComponent<IA>().health += value;
                if(other.gameObject.GetComponent<IA>().health > other.gameObject.GetComponent<IA>().initHealth){
                    other.gameObject.GetComponent<IA>().health = other.gameObject.GetComponent<IA>().initHealth;
                }
            }

            else if(type == "Shield"){
                other.gameObject.GetComponent<IA>().shield = true;
            }
        }

        source.Play();
    }
}
