using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed=1f;
    private Vector2 positionInput;

    // Update is called once per frame
    void Update()
    {
        float moveX=Input.GetAxisRaw("Horizontal");
        float moveY=Input.GetAxisRaw("Vertical");
        positionInput=new Vector2(moveX,moveY);
    }
    private void FixedUpdate()
    {
        gameObject.transform.position=new Vector2(gameObject.transform.position.x+positionInput.x*speed*Time.fixedDeltaTime
        ,gameObject.transform.position.y+positionInput.y*speed*Time.fixedDeltaTime);
    }
}
