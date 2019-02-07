

/*Created By Alcash Pirynas@mail.ru 
 * https://github.com/Alcash
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {


    public Transform m_ARCameraTransform;

    public GameObject m_BallPrefab;
    GameObject ballInstance;
    Rigidbody ballRigidbody;
    //MazeController mazeController;

    float speed = 2;

    private void Start()
    {
        //mazeController = GetComponent<MazeController>();
        //mazeController.OnMazeComplete += BallSpawn;
    }

    void BallSpawn()
    {
        if (ballInstance != null)
        {
            Destroy(ballInstance);
        }
        Debug.Log("BallSpawn");

        var spawnTransform = transform.parent;
        //var spawnTransform = mazeController.GetFirstEmpty();
        if (spawnTransform != null)
        {
            Debug.Log("spawnTransform " + spawnTransform.position);
            var position = spawnTransform.localPosition;
            position.y = 0.0234f;
            ballInstance = Instantiate(m_BallPrefab, position, Quaternion.identity, transform);

            ballRigidbody = ballInstance.GetComponent<Rigidbody>();
        }

    }


    private void FixedUpdate()
    {
        if (ballInstance != null)
        {
            var direction = m_ARCameraTransform.position - transform.position;
            direction.y = 0;
            ballRigidbody.velocity = direction*speed;
        }
    }
}
