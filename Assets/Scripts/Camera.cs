using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    IEnumerator Start()
    {
        Vector3 targetPosition = transform.position;

        while(true){
            if(GameManager.instance.PVP || GameManager.instance.PVCOM){
                while(Vector3.Distance(transform.position, targetPosition) > 0.001f){
                    transform.RotateAround(Vector3.zero, Vector3.up, 100 * Vector3.Distance(transform.position, targetPosition) * Time.deltaTime);
                    yield return null;
                }

                targetPosition = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
                yield return new WaitForSeconds(30f);
            }

            yield return null;
        }
    }
}
