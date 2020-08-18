using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class IA : MonoBehaviour
{
    //Vida que tendrá la ia
    public int health = 100;

    //Daño que le hará al jugador
    public int damage = 5;
    
    Rigidbody rig;
    Animator anim;
    
    //Referencia al jugador
    Transform player;

    //Guarda el index del lugar mas cerca de la ia donde se coloca los collectables
    //Se usa para empezar a buscar siempre desde la posición de la ia la distancia mas corta
    //Hacia el lugar donde sale el nuevo collectable 
    [HideInInspector]
    public int minIndex = 0;

    void Awake(){
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        player = GameObject.Find("Player").transform;
    }

    IEnumerator Start(){
        while(true){
            //Si la direccion actual del juego es 0 (no se están moviendo y por lo tanto el jugador
            //no ha presionado una tecla para jugar) y el player no ha sido destruido
            if(GameManager.instance.direction == 0 && player){
                yield return new WaitForSeconds(0.6f);
                
                //Actualizamos el index del lugar mas cercano a la ia donde se crea un collectable
                //(es decir se obtiene la posición mas cerca a la ia del lugar donde se puede poner un collectable) 
                UpdateMinIndex();

                //Juega con su posición actualizada, aqui es cuando empieza a buscar el lado mas corto
                Play();
            }
            yield return null;
        }
    }

    //Juega al lado mas corto para agarrar el collectable de primero
    void Play(){
        //Si al momento de jugar el jugador ya presionó una tecla entonces no hace nada
        if(GameManager.instance.direction != 0){
            return;
        }

        //La cantidad de lugares que le hacen falta a la ia para alcanzar el lugar del collectable
        //que quiere agarrar, siempre será cero al inicio de cada jugada para que vaya aumentando
        //desde su posición actual
        int count = 0;

        //Recorre todos los lugares donde se puede crear un collectable desde
        //su posición actual (minIndex) hasta que haya recorrido toda la cantidad de lugares o
        //hasta que halla alcanzado a la posición del lugar donde se creó el collectable
        for(int currentIndex = minIndex + 1; count < GameManager.instance.amountOfPoints && currentIndex != GameManager.instance.lastItemIndex + 1; currentIndex++){
            //Si su ultima posición del lugar de collectables supera el limite (cuando queda por ejemplo a mas de la mitad)
            //es porque falta recorrer los otros lugares por debajo de la mitad asi que se coloca al inicio para que recorra
            //todo los luagres completos
            if(currentIndex >= GameManager.instance.transform.childCount){
                currentIndex = 0;
            }

            //Se aumenta en uno la cantidad de lugares que ha recorrido
            //(entre mas sea la cantidad mas lejos está el collectable)
            count++;

            /*Debug.DrawRay(
                GameManager.instance.transform.GetChild(currentIndex).position,
                (GameManager.instance.transform.GetChild(GameManager.instance.beforeItemIndex).position - GameManager.instance.transform.GetChild(currentIndex).position).normalized * (GameManager.instance.transform.GetChild(GameManager.instance.beforeItemIndex).position - GameManager.instance.transform.GetChild(currentIndex).position).magnitude,
                Color.red,
                3f
            );*/
        }

        //Si la cantidad de lugares que recorrió desde su posición es
        //menos o igual a la mitad entonces gira hacia la derecha
        if(count <= GameManager.instance.amountOfPoints/2){
            GameManager.instance.direction = 1;
        }

        //Si la derecha es mayor a la mitad quiere decir que la izquierda
        //sería el camino mas corto
        else{
            GameManager.instance.direction = -1;
        }
    }

    //Actualiza el index de los lugares donde se puede crear los collectables
    //al lugar mas cercano a la posición de la ia
    //(ya que cuando choca con el collectable se detiene de una vez y no alcanza a
    //llegar a su posición completa) por eso se busca la mas cercana
    void UpdateMinIndex(){
        //Hacemos que la distancia minima sea la mayor que pueda haber
        //que seria el diametro del cirtulo donde se moveran
        float minDistance = GameManager.instance.radius * 2;

        //Recorremos todos los lugares donde se puedan colocar los collectables
        for(int i=0; i<GameManager.instance.amountOfPoints; i++){
            //Y se va comparando la distancia que hay entre cada uno de ellos y la posición de la ia
            float tempDistance = Vector3.Distance(GameManager.instance.transform.GetChild(i).position, transform.position);

            if(tempDistance < minDistance){
                //Y se va guardando la menor distancia
                minDistance = tempDistance;
                //Para poder guardar el index del lugar donde se crea el collectable mas cercano a la ia
                //Para poder comparar desde ese lugar
                //ya que la comparación avanza es de lugar en lugar
                minIndex = i;
            }
        }
    }

    void Update(){
        //Si el jugador no ha sido destruido
        if(player){
            //Hacemos que la ia lo mire de frente
            transform.LookAt(player.position);
        }
    }

    void FixedUpdate(){
        //Hacemos que la posición de la ia sea la contraria a la del jugador
        //inviertiendo el angulo que se usa para desplazarse
        rig.MovePosition(new Vector3(-Mathf.Cos(GameManager.instance.angle), 0f, -Mathf.Sin(GameManager.instance.angle)) * GameManager.instance.radius);
    }

    //Ataque de la ia
    void Attack(){
        anim.SetTrigger("Attack");
        //Debug.DrawRay(transform.position, (player.position - transform.position).normalized * GameManager.instance.radius * 2, Color.blue);

        //Reduce la vida del jugador, quitandole el daño que haga
        player.GetComponent<Player>().health -= damage;
        
        //Si la vida del jugador llega a cero pierde
        if(player.GetComponent<Player>().health <= 0){
            Destroy(player.transform.gameObject);
        }
    }

    //Si agarra un collectable
    void OnTriggerEnter(Collider other){
        if(other.name != "Player"){
            //Detiene el movimiento
            GameManager.instance.direction = 0f;

            //Destruye el collectable
            Destroy(other.gameObject);

            //Ataca al jugador
            Attack();

            //Crea otro collectable en el mapa
            GameManager.instance.InstantiateCollectable();
        }
    }
}
