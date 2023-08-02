using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed;
    Rigidbody2D rigidBody;

    // Conveyor Direction Key Is:  Right = 1, Up = 2, Left = 3, Down = 4
    public int conveyorDirection;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (conveyorDirection == 1)
        {
            Vector3 pos = rigidBody.position;
            rigidBody.position += Vector2.left * speed * Time.deltaTime;
            rigidBody.MovePosition(pos);    
        }

        if (conveyorDirection == 2)
        {
            Vector3 pos = rigidBody.position;
            rigidBody.position += Vector2.up * speed * Time.deltaTime;
            rigidBody.MovePosition(pos);
        }

        if (conveyorDirection == 3)
        {
            Vector3 pos = rigidBody.position;
            rigidBody.position += Vector2.right * speed * Time.deltaTime;
            rigidBody.MovePosition(pos);
        }

        if (conveyorDirection == 4)
        {
            Vector3 pos = rigidBody.position;
            rigidBody.position += Vector2.down * speed * Time.deltaTime;
            rigidBody.MovePosition(pos);
        }    
    } 
}
