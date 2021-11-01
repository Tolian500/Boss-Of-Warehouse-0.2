using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float PanSpeed = 10.0f;
    public bool fixedToPlayer = false;
    [SerializeField] Vector3 mousePos;

    int mDelta = 10; // Pixels. The width border at the edge in which the movement work
    [SerializeField] float mSpeed = 3.0f; // Scale. Speed of the movement
    Vector3 mRightDirection = Vector3.right;
    Vector3 mUpDirection = Vector3.forward;
    private GameObject player;
    public bool isFixedToPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        player = GameObject.Find("Player");
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
        else if (Input.mousePosition.x <= 0 - mDelta)
        {
            // Move the camera
            transform.position += mRightDirection * Time.deltaTime * mSpeed;
        }

        else if (Input.mousePosition.y >= Screen.height - mDelta)
        {
            // Move the camera
            transform.position -= mUpDirection * Time.deltaTime * mSpeed;
        }

        else if (Input.mousePosition.y <= 0 - mDelta)
        {
            // Move the camera
            transform.position += mUpDirection * Time.deltaTime * mSpeed;
        }


    }
    void KeybordMoveControl()
    {
        
        Vector3  move = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), -Input.GetAxis("Mouse ScrollWheel"));
        gameObject.transform.position = gameObject.transform.position + new Vector3(-move.y, move.z, -move.x) * PanSpeed * Time.deltaTime;
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
    }
    public void ChangeFixedToPlayer()
    {
        fixedToPlayer = !fixedToPlayer;
    }
}
