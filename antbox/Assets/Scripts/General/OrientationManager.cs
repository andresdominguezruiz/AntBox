using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrientationManager : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector2 lastPosition;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate direction vector
        Vector2 currentPosition = transform.position;
        Vector2 direction = currentPosition - lastPosition;
        
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Rotate around Z-axis to face the direction of movement
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        lastPosition = currentPosition;
        
    }
}
