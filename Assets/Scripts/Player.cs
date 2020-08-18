using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    //Vida que tendrá el jugador
    public int health = 100;

    //Daño que hará el jugador a la ia
    public int damage = 5;

    Rigidbody rig;
    Animator anim;
    
    //Referencia a la ia
    Transform ia;

    //Nos dice si presionó las teclas y las volvió a soltar
    //para comprar que solo se mueva cada vez que presione una tecla
    //y no cuando las deja presionadas
    bool keyDown;

    void Awake(){
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        ia = GameObject.Find("IA").transform;
    }

    void Update(){
        //Si no se están moviento y la ia no se ha destruido
        if(GameManager.instance.direction == 0 && ia){
            //Y no tiene la tecla presionada
            if(!keyDown){
                //Entonces cambia la dirección del juego a la dirección que seleccionó
                //con las teclas
                GameManager.instance.direction = Input.GetAxisRaw("Horizontal");

                //Y hacemos keyDown a true para que deje de comprobar por si se mantiene
                //las teclas presionadas
                keyDown = true;
            }
        }

        //Si la ia no ha muerto entonces que se le quede mirando de frente
        if(ia){
            transform.LookAt(ia.position);
        }

        //Hasta que suelte las teclas
        if(Input.GetAxisRaw("Horizontal") == 0){
            //Se hace keyDown a false (queriendo decir que ya soltó las teclas y está listo para
            //comprobar ahora si, si el jugador logra reaccionar antes que la ia)
            keyDown = false;
        }
    }

    void FixedUpdate(){
        //Se mueve el jugador a la dirección que el diga
        rig.MovePosition(new Vector3(Mathf.Cos(GameManager.instance.angle), 0f, Mathf.Sin(GameManager.instance.angle)) * GameManager.instance.radius);
    }

    //Ataque del jugador
    void Attack(){
        anim.SetTrigger("Attack");
        //Debug.DrawRay(transform.position, (ia.position - transform.position).normalized * GameManager.instance.radius * 2, Color.yellow);

        //Resta vida a la ia dependiendo del daño que haga
        ia.GetComponent<IA>().health -= damage;
        
        //Si la vida de la ia es cero se destruye
        if(ia.GetComponent<IA>().health <= 0){
            Destroy(ia.transform.gameObject);
        }
    }

    //Si el jugador choca con un collectable
    void OnTriggerEnter(Collider other){
        if(other.name != "IA"){
            //Detiene el movimiento del juego
            GameManager.instance.direction = 0f;

            //Destryude el collectable
            Destroy(other.gameObject);

            //Ataca a la ia
            Attack();

            //Y crea un nuevo collectable en el mapa
            GameManager.instance.InstantiateCollectable();
        }
    }
}
