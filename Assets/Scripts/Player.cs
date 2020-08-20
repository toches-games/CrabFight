using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public int initHealth = 100;

    //Vida que tendrá el jugador
    [HideInInspector]
    public int health;

    //Daño que hará el jugador a la ia
    public int damage = 5;

    //Velocidad con la que se moverá el jugador
    public float speed = 20f;

    Rigidbody rig;
    Animator anim;

    //Nos dice si presionó las teclas y las volvió a soltar
    //para comprar que solo se mueva cada vez que presione una tecla
    //y no cuando las deja presionadas
    bool keyDown;

    bool upCollectable;

    bool fail;

    [HideInInspector]
    public float inputDirection = 0;

    Vector3 initPosition;

    AudioSource source;

    [HideInInspector]
    public bool shield;

    [HideInInspector]
    public bool isDead;

    void Awake(){
        rig = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    void Start(){
        initPosition = transform.position;
        health = initHealth;

        StartCoroutine(Move());
    }

    IEnumerator Move(){
        while(true){
            //Si no se ha presionado la tecla, no se está jugando y presiona una tecla
            if(!keyDown && !GameManager.instance.isPlaying && Input.GetAxisRaw("Horizontal") != 0 && GameManager.instance.currentCollectable){
                //Y hacemos keyDown a true para que deje de comprobar por si se mantiene
                //las teclas presionadas
                keyDown = true;

                //Entonces juega el jugador
                Play();
            }

            //Si la ia no ha muerto entonces que se le quede mirando de frente
            if(GameManager.instance.ia && !GameManager.instance.ia.GetComponent<IA>().isDead){
                transform.LookAt(GameManager.instance.ia.position);
            }

            //Hasta que suelte las teclas
            if(Input.GetAxisRaw("Horizontal") == 0){
                //Se hace keyDown a false (queriendo decir que ya soltó las teclas y está listo para
                //comprobar ahora si, si el jugador logra reaccionar antes que la ia)
                keyDown = false;
            }

            //Si se está jugando se movera el jugador si presionó una tecla
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
                        Destroy(GameManager.instance.currentCollectable.gameObject, 1f);

                        yield return new WaitForSeconds(0.1f);
                    }
                }

                else{
                    initDistance = Vector3.Distance(initPosition, transform.position);
                }

                transform.RotateAround(Vector3.zero, Vector3.up, -inputDirection * speed * initDistance * Time.deltaTime);

                anim.SetBool("Walk", inputDirection != 0);

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

    void Play(){
        inputDirection = Input.GetAxisRaw("Horizontal");

        if(inputDirection != GameManager.instance.currentCollectable.position.x){
            fail = true;
        }

        GameManager.instance.isPlaying = true;
    }

    //Ataque del jugador
    void Attack(){
        anim.SetTrigger("Attack");

        GameManager.instance.PlayerAttackToIA();

        source.Play();
    }

    //Si el jugador choca con un collectable
    void OnTriggerEnter(Collider other){
        if(other.name != "IA" && GameManager.instance.currentCollectable){
            GameManager.instance.currentCollectable.GetComponent<Renderer>().enabled = false;
            GameManager.instance.currentCollectable.GetComponent<SphereCollider>().enabled = false;
        }
    }
}
