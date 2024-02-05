using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntAnimatorManager : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator=this.gameObject.GetComponentInChildren<Animator>();
        
    }
    public void SufferCritical(){
        animator.SetTrigger("SufferCritical");
    }

    public void DodgeAttack(){
        animator.SetTrigger("Dodge");
    }

    public void BeingHurt(){
        animator.SetTrigger("Hurt");
    }
}
