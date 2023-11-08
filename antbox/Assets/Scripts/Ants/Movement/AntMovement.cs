using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    public float speed=0.5f;

    private Navigator navigator;
    void Start()
    {
        this.navigator=FindObjectOfType<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            var screen=Input.mousePosition;
            var world=Camera.main.ScreenToWorldPoint(screen);
            world.z=1;
            var path=new List<Vector3>();
            path.Add(world);

            StopAllCoroutines();
            StartCoroutine(this.RunPath(path));
        }
    }

    IEnumerator RunPath(List<Vector3> path){
        foreach(var target in path){
            var origin=this.transform.position;
            float percent=0;
            while(percent<1f){
                this.transform.position=Vector3.Lerp(origin,target,percent);
                percent+=Time.deltaTime*this.speed;
                yield return null;
            }
        }
    }
}
