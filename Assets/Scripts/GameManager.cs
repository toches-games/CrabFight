using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Radio del circulo donde caminarán y donde se crearan los collectables
    [Range(1, 5)]
    public int radius = 1;

    //Numero de lugares donde se podrán poner los collectables de manera aleatorea
    [Range(10, 20)]
    public int amountOfPoints = 10;

    //Los items que existirán en el juego
    public GameObject[] collectables;

    //Index del lugar donde se puso el ultimo collectable (el que se ve en pantalla)
    [HideInInspector]
    public int lastItemIndex = 0;

    //Index del lugar donde estaba el anterior collectable (ultimo que se recogió)
    [HideInInspector]
    public int beforeItemIndex = 0;

    //Referencia al player
    [HideInInspector]
    public Transform player;

    //Referencia a la ia
    [HideInInspector]
    public Transform ia;

    [HideInInspector]
    public bool isPlaying;

    [HideInInspector]
    public Transform currentCollectable;

    void Awake(){
        if(!instance){
            instance = this;
        }

        else{
            Destroy(this);
        }
    }

    IEnumerator Start(){
        player = GameObject.Find("Player").transform;
        ia = GameObject.Find("IA").transform;

        while(true){
            if(!isPlaying && !currentCollectable){
                yield return new WaitForSeconds(0.5f);
                InstantiateCollectable();
            }

            yield return null;
        }
    }

    //Crea un nuevo collectable en el mapa
    public void InstantiateCollectable(){
        int randomX = 0;

        do{
            randomX = Random.Range(-1, 2);
        }while(randomX == 0);

        currentCollectable = Instantiate(collectables[Random.Range(0, collectables.Length)], new Vector3(randomX, 0.6f, 0f), Quaternion.identity).transform;
    }

    public void PlayerAttackToIA(){

    }

    public void IAAttackToPlayer(){

    }

}
