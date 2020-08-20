using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Los items que existirán en el juego
    public GameObject[] collectables;

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

    public GameObject[] ias;

    int iaCount = 0;

    AudioSource source;

    Vector3 velocity;

    [HideInInspector]
    public bool PVCOM;

    [HideInInspector]
    public bool PVP;

    public Slider playerSlider;
    public Slider iaSlider;

    float velocityF;

    //Colores de los collectables
    public Color colorLife;
    public Color colorDeffense;
    public Color colorAttack;

    void Awake(){
        if(!instance){
            instance = this;
        }

        else{
            Destroy(this);
        }
    }

    IEnumerator Start(){
        source = GetComponent<AudioSource>();
        
        player = GameObject.Find("Player").transform;
        
        while(!PVCOM && !PVP){
            yield return null;
        }

        if(PVCOM){
            ia = GameObject.Find("IA 1").transform;
        }

        else{
            GameObject.Find("IA 1").name = "Player 2";
            ia = GameObject.Find("Player 2").transform;
        }

        InitUI();

        while(true){
            //segundos para crear el primer item
            yield return new WaitForSeconds(3f);

            //Si no se juega y no hay un collectable creado y existen player e ia
            //entonces crea un collectable
            if(!isPlaying && !currentCollectable && player && !player.GetComponent<Player>().isDead && ia && !ia.GetComponent<IA>().isDead){
                yield return new WaitForSeconds(0.1f);
                InstantiateCollectable();
            }

            //gana ia
            if(!player){
                Debug.Log("Gana ia");
                StopAllCoroutines();
            }

            //gana jugador
            else if(ia && ia.GetComponent<IA>().isDead){
                if(iaCount >= ias.Length){
                    Debug.Log("Gana player");
                    StopAllCoroutines();
                }
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

        currentCollectable = Instantiate(collectables[Random.Range(0, collectables.Length)], new Vector3(randomX, -0.1f, 0f), Quaternion.identity).transform;
        StartCoroutine(CollectableAnimation());
    }

    IEnumerator CollectableAnimation(){
        source.Play();
        Vector3 targetPosition = new Vector3(currentCollectable.position.x, 0.8f, 0f);

        while(Vector3.Distance(currentCollectable.position, targetPosition) > 0.001f){
            currentCollectable.position = Vector3.SmoothDamp(currentCollectable.position, targetPosition, ref velocity, 0.1f);
            yield return null;
        }

        targetPosition = new Vector3(currentCollectable.position.x, 0.6f, 0f);

        while(Vector3.Distance(currentCollectable.position, targetPosition) > 0.001f){
            currentCollectable.position = Vector3.SmoothDamp(currentCollectable.position, targetPosition, ref velocity, Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator SliderAnimation(float init, float target, string quien){
        while(Mathf.Abs(target - init) > 0.001f){
            init = Mathf.SmoothDamp(init, target, ref velocityF, 0.2f);

            if(quien == "ia"){
                iaSlider.value = init;
            }

            else{
                playerSlider.value = init;
            }

            yield return null;
        }
    }

    public void PlayerAttackToIA(){
        if(ia.GetComponent<IA>().shield){
            ia.GetComponent<IA>().shield = false;
        }
        
        else{
            StartCoroutine(SliderAnimation(ia.GetComponent<IA>().health, ia.GetComponent<IA>().health - player.GetComponent<Player>().damage, "ia"));
            ia.GetComponent<IA>().health -= player.GetComponent<Player>().damage;

            if(ia.GetComponent<IA>().health <= 0){
                ia.GetComponent<IA>().isDead = true;
                StartCoroutine(DeathAnimation(ia));
            }
        }

    }

    public void IAAttackToPlayer(){
        if(player.GetComponent<Player>().shield){
            player.GetComponent<Player>().shield = false;
        }

        else{
            StartCoroutine(SliderAnimation(player.GetComponent<Player>().health, player.GetComponent<Player>().health - ia.GetComponent<IA>().damage, "player"));
            player.GetComponent<Player>().health -= ia.GetComponent<IA>().damage;

            if(player.GetComponent<Player>().health <= 0){
                player.GetComponent<Player>().isDead = true;
                StartCoroutine(DeathAnimation(player));
            }
        }
    }

    IEnumerator DeathAnimation(Transform temp){
        Vector3 targetPosition = new Vector3(temp.position.x, -0.8f, temp.position.z);

        if(temp && temp.GetComponent<IA>()){
            iaCount++;
        }

        while(temp && Vector3.Distance(temp.position, targetPosition) > 0.001f){
            temp.position = Vector3.SmoothDamp(temp.position, targetPosition, ref velocity, 0.5f);
            yield return null;
        }

        if(temp && temp.GetComponent<IA>()){
            Destroy(ia.gameObject);
        }
        
        if(iaCount < ias.Length && player && !player.GetComponent<Player>().isDead){
            ia = Instantiate(ias[iaCount], new Vector3(0, 0.6f, 1), Quaternion.Euler(0, 180, 0)).transform;
            InitUI();
        }

        else if(temp && temp.GetComponent<Player>()){
            Destroy(temp.gameObject);
        }
    }

    public void InitPVP(){
        PVP = true;
    }

    public void InitPVCOM(){
        PVCOM = true;
    }

    void InitUI(){
        iaSlider.maxValue = ia.GetComponent<IA>().initHealth;
        iaSlider.value = ia.GetComponent<IA>().initHealth;

        Image iaSliderColor = iaSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        iaSliderColor.color = ia.GetChild(0).GetChild(1).GetComponent<Renderer>().materials[0].color;
        
        if(iaCount == 0){
            playerSlider.maxValue = player.GetComponent<Player>().initHealth;
            playerSlider.value = player.GetComponent<Player>().initHealth;

            Image playerSliderColor = playerSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            playerSliderColor.color = player.GetChild(0).GetChild(1).GetComponent<Renderer>().materials[0].color;
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
