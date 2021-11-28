using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float PanSpeed = 10.0f;
    public bool fixedToPlayer = false;
    
    int mDelta = 10; // Pixels. The width border at the edge in which the movement work
    [SerializeField] float mSpeed = 15.0f; // Scale. Speed of the movement
    Vector3 mRightDirection = Vector3.right;
    Vector3 mUpDirection = Vector3.forward;
    private GameObject player;
    [SerializeField] float maxHight;
    [SerializeField] float minHight;
    [SerializeField] float[] minMaxRotationX;
    public Vector3 minBorderPos;
    public Vector3 maxBorderPos;

    private float highCoef;
    private float angleCoef;

    // Start is called before the first frame update
    void Start()
    {
        minBorderPos = new Vector3(-20, 5, -20);
        maxBorderPos = new Vector3(20, 5, 10);
    }
    private void Awake()
    {
        player = GameObject.Find("Player");
        highCoef = 100 / (maxHight - minHight);
        angleCoef = (minMaxRotationX[1] - minMaxRotationX[0]) / 100;
    }
    // Update is called once per frame
    void Update()
    {
        
        KeybordMoveControl();

        MouseMoveControl();
        if (fixedToPlayer)
        {
            gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 7f, player.transform.position.z + 5f);

        }
        



    }

    void MouseMoveControl()
    {
        if (Input.mousePosition.x >= Screen.width - mDelta)
        {
            // Move the camera
            transform.position -= mRightDirection * Time.deltaTime * mSpeed;
        }
        else if (Input.mousePosition.x <= 20 - mDelta)
        {
            // Move the camera
            transform.position += mRightDirection * Time.deltaTime * mSpeed;
        }

        else if (Input.mousePosition.y >= Screen.height - mDelta)
        {
            // Move the camera
            transform.position -= mUpDirection * Time.deltaTime * mSpeed;
        }

        else if (Input.mousePosition.y <= 20 - mDelta)
        {
            // Move the camera
            transform.position += mUpDirection * Time.deltaTime * mSpeed;
        }
        CameraBorder();

    }
    void KeybordMoveControl()
    {
        
        Vector3  move = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), -Input.GetAxis("Mouse ScrollWheel"));
        gameObject.transform.position = gameObject.transform.position + new Vector3(-move.y, move.z, -move.x) * PanSpeed * Time.deltaTime;
        //gameObject.transform.eulerAngles = gameObject.transform.eulerAngles + new Vector3 (move.z/5,0,0);
        
        gameObject.transform.eulerAngles = new Vector3(minMaxRotationX[0] + angleCoef * highCoef * gameObject.transform.position.y, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        CameraBorder();
    }
    public void ChangeFixedToPlayer()
    {
        fixedToPlayer = !fixedToPlayer;
    }
    public void CameraBorder()
    {
        if (transform.position.y >= maxHight)
        {
            gameObject.transform.position = new Vector3(transform.position.x, maxHight, transform.position.z);

        }
        if (transform.position.y <= minHight)
        {
            gameObject.transform.position = new Vector3(transform.position.x, minHight, transform.position.z);
        }
       if (transform.eulerAngles.x >= minMaxRotationX[1])
        {
           gameObject.transform.eulerAngles = new Vector3(minMaxRotationX[1], transform.eulerAngles.y, transform.eulerAngles.z);
        }
       if (transform.eulerAngles.x <= minMaxRotationX[0])
        {
            gameObject.transform.eulerAngles = new Vector3(minMaxRotationX[0], transform.eulerAngles.y, transform.eulerAngles.z);
        }
       
        if (transform.position.x <= minBorderPos.x)
        {
            gameObject.transform.position = new Vector3(minBorderPos.x, transform.position.y, transform.position.z);
        }
        if (transform.position.x >= maxBorderPos.x)
        {
            gameObject.transform.position = new Vector3(maxBorderPos.x, transform.position.y, transform.position.z);
        }
        if (transform.position.z <= minBorderPos.z)
        {
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, minBorderPos.z);
        }
        if (transform.position.z >= maxBorderPos.z)
        {
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, maxBorderPos.z);
        }
    }
}
