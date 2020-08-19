using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        ia = GameObject.Find("IA 1").transform;

        while(true){
            if(!isPlaying && !currentCollectable && player && ia){
                yield return new WaitForSeconds(0.1f);
                InstantiateCollectable();
            }

            if(!player){
                Debug.Log("Gana ia");
                StopAllCoroutines();
            }

            else if(!ia){
                Debug.Log("Gana player");
                StopAllCoroutines();
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
        StartCoroutine(Animation());
    }

    IEnumerator Animation(){
        source.Play();
        Vector3 targetPosition = new Vector3(currentCollectable.position.x, 0.6f, 0f);

        while(Vector3.Distance(currentCollectable.position, targetPosition) > 0.001f){
            currentCollectable.position = Vector3.SmoothDamp(currentCollectable.position, targetPosition, ref velocity, 0.1f);
            yield return null;
        }
    }

    public void PlayerAttackToIA(){
        if(ia.GetComponent<IA>().shield){
            ia.GetComponent<IA>().shield = false;
        }
        
        else{
            ia.GetComponent<IA>().health -= player.GetComponent<Player>().damage;

            if(ia.GetComponent<IA>().health <= 0){
                Destroy(ia.gameObject);
                iaCount++;

                if(iaCount < ias.Length){
                    ia = Instantiate(ias[iaCount], new Vector3(0, 0.6f, 1), Quaternion.Euler(0, 180, 0)).transform;
                }
            }
        }

    }

    public void IAAttackToPlayer(){
        if(player.GetComponent<Player>().shield){
            player.GetComponent<Player>().shield = false;
        }

        else{
            player.GetComponent<Player>().health -= ia.GetComponent<IA>().damage;

            if(player.GetComponent<Player>().health <= 0){
                Destroy(player.gameObject);
            }
        }
    }

}
