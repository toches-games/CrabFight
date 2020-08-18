using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Velocidad con la que se movera tanto el jugador como la ia
    //tienen la misma velocidad porque siempre estarán de frente
    [Range(1, 5)]
    public float speed = 1f;

    //Radio del circulo donde caminarán y donde se crearan los collectables
    [Range(1, 5)]
    public int radius = 1;

    //Numero de lugares donde se podrán poner los collectables de manera aleatorea
    [Range(10, 20)]
    public int amountOfPoints = 10;

    //Los items que existirán en el juego
    public GameObject[] collectables;

    //Angulo del juego, es el encargado de realizar el movimiento del jugador y la ia
    [HideInInspector]
    public float angle = 0f;

    //Direccion en la que se gira el movimiento del juego
    [HideInInspector]
    public float direction = 0f;

    //Index del lugar donde se puso el ultimo collectable (el que se ve en pantalla)
    [HideInInspector]
    public int lastItemIndex = 0;

    //Index del lugar donde estaba el anterior collectable (ultimo que se recogió)
    [HideInInspector]
    public int beforeItemIndex = 0;

    //Referencia al player
    Transform player;

    //Referencia a la ia
    Transform ia;

    void Awake(){
        if(!instance){
            instance = this;
        }

        else{
            Destroy(this);
        }
    }

    void Start(){
        player = GameObject.Find("Player").transform;
        ia = GameObject.Find("IA").transform;

        //Se halla los grados a los que estarán separados los lugares donde se colcoarán los collectables
        //dependiendo de la cantidad que se quieran
        float degrees = 360 / amountOfPoints;

        for(int i=0; i<amountOfPoints; i++){
            //Se halla la posición del lugar
            Vector3 pos = new Vector3(Mathf.Cos(i * degrees * Mathf.Deg2Rad), 0.5f, Mathf.Sin(i * degrees * Mathf.Deg2Rad)) * radius;

            //Se muestra las posiciones de los lugares para verlos mejor
            Debug.DrawRay(transform.position, (pos - transform.position).normalized * (pos - transform.position).magnitude, Color.yellow, Mathf.Infinity);

            //Se crea un objeto vació
            Transform temp = new GameObject().transform;

            //Con la posición hallada antes
            temp.position = pos;

            //Y lo hacemos hijo del GameManager
            temp.SetParent(transform);
        }

        //Se crea el primer collectable del mapa al inicio
        InstantiateCollectable();
    }

    void Update(){
        //Va actualizando el angulo del mapa poco a poco para ir moviendo al jugador y la ia
        angle += direction * speed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //Crea un nuevo collectable en el mapa
    public void InstantiateCollectable(){
        //Antes de crear uno nuevo se guarda el index del actual
        beforeItemIndex = lastItemIndex;

        do{
            //Se elige un nuevo lugar aleatoreo donde poner el collectable
            lastItemIndex = Random.Range(0, transform.childCount);

            //Mientras que la distancia de este lugar no esté tan cerca de la posición del jugador ni de la ia
            //Ya que se podría justo en es posición y no se veria cuando los agarra
        }while(Vector3.Distance(transform.GetChild(lastItemIndex).position, player.position) < 1f || (Vector3.Distance(transform.GetChild(lastItemIndex).position, ia.position) < 1f));

        //Finalmente se crea el collectable aleatoreo en la mapa 
        Instantiate(collectables[Random.Range(0, collectables.Length)], transform.GetChild(lastItemIndex).position, Quaternion.identity);
    }
}
