using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class IA : MonoBehaviour
{
    public int initHealth = 100;

    //Vida que tendrá la ia
    [HideInInspector]
    public int health;

    //Daño que le hará al jugador
    public int damage = 5;

    public float speed = 20f;

    [Range(0, 1)]
    public float probability;
    
    Rigidbody rig;
    Animator anim;

    [HideInInspector]
    public float inputDirection = 0;

    bool upCollectable;
    bool fail;

    Vector3 initPosition;

    AudioSource source;

    [HideInInspector]
    public bool shield;

    void Awake(){
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    IEnumerator Start(){
        initPosition = transform.position;
        health = initHealth;

        StartCoroutine(Move());

        while(true){
            //Si la direccion actual del juego es 0 (no se están moviendo y por lo tanto el jugador
            //no ha presionado una tecla para jugar) y el player no ha sido destruido
            if(!GameManager.instance.isPlaying && GameManager.instance.currentCollectable){
                yield return new WaitForSeconds(0.4f);

                //Juega con su posición actualizada, aqui es cuando empieza a buscar el lado mas corto
                Play();
            }

            yield return null;
        }
    }

    //Juega al lado mas corto para agarrar el collectable de primero
    void Play(){
        //Si al momento de jugar el jugador ya presionó una tecla entonces no hace nada
        if(GameManager.instance.isPlaying){
            return;
        }

        GameManager.instance.isPlaying = true;

        if(Random.value <= probability){
            inputDirection = (GameManager.instance.currentCollectable.position - transform.position).normalized.x;
        }
        else{
            inputDirection = -(GameManager.instance.currentCollectable.position - transform.position).normalized.x;
            fail = true;
        }
    }

    IEnumerator Move(){
        while(true){
            //Si el jugador no ha sido destruido
            if(GameManager.instance.player){
                //Hacemos que la ia lo mire de frente
                transform.LookAt(GameManager.instance.player.position);
            }

            if(GameManager.instance.isPlaying && inputDirection != 0){
                float initDistance = 0f;

                if(fail){
                    initDistance = Vector3.Distance(new Vector3(
                        -GameManager.instance.currentCollectable.position.x,
                        GameManager.instance.currentCollectable.position.y,
                        GameManager.instance.currentCollectable.position.z
                    ), transform.position);

                    if(initDistance <= 0.01f){
                        inputDirection = -inputDirection;
                        upCollectable = true;
                        fail = false;
                    }
                }
                
                else if(!upCollectable && GameManager.instance.currentCollectable){
                    initDistance = Vector3.Distance(GameManager.instance.currentCollectable.position, transform.position);
                    
                    if(initDistance <= 0.01f){
                        inputDirection = -inputDirection;
                        upCollectable = true;

                        //Ataca al jugador
                        Attack();
                        
                        //Destruye el collectable
                        Destroy(GameManager.instance.currentCollectable.gameObject);

                        yield return new WaitForSeconds(0.1f);
                    }
                }

                else{
                    initDistance = Vector3.Distance(initPosition, transform.position);
                }

                transform.RotateAround(Vector3.zero, Vector3.up, inputDirection * speed * initDistance * Time.deltaTime);

                if(initDistance <= 0.001f && upCollectable){
                    upCollectable = false;
                    transform.position = initPosition;
                    GameManager.instance.isPlaying = false;
                    inputDirection = 0;
                }
            }

            yield return null;
        }
    }

    //Ataque de la ia
    void Attack(){
        anim.SetTrigger("Attack");

        GameManager.instance.IAAttackToPlayer();

        source.Play();
    }

    //Si agarra un collectable
    void OnTriggerEnter(Collider other){
        if(other.name != "Player"){
            GameManager.instance.currentCollectable.GetComponent<Renderer>().enabled = false;
        }
    }
}
