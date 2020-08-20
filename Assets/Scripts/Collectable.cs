using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    AudioSource source;

    public int value = 10;
    public string type;

    public GameObject particles;

    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other){
        GameObject temp = Instantiate(particles, transform.position, transform.rotation);
        Destroy(temp, 1f);

        if(other.name == "Player"){
            if(type == "Health"){
                StartCoroutine(GameManager.instance.SliderAnimation(
                    other.gameObject.GetComponent<Player>().health,
                    other.gameObject.GetComponent<Player>().health + value,
                    "player"
                ));

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
                StartCoroutine(GameManager.instance.SliderAnimation(
                    other.gameObject.GetComponent<IA>().health,
                    other.gameObject.GetComponent<IA>().health + value,
                    "ia"
                ));

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
