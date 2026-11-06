
using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float sideSpeed = 12f;

    public float jumpForce = 13f;
    public float gravity = 20f;
    public float groundY = 1f;

    int lane = 1;
    float verticalVelocity;
    bool grounded = true;

    void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.gameOver)
            return;

        // Move left and right
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.A))
        {
            lane = Mathf.Max(0, lane - 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.D))
        {
            lane = Mathf.Min(2, lane + 1);
        }

        // Jump
        if ((Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.UpArrow)) && grounded)
        {
            verticalVelocity = jumpForce;
            grounded = false;
        }

        Vector3 pos = transform.position;

        // Lane movement
        float targetX = (lane - 1) * laneDistance;

        pos.x = Mathf.Lerp(
            pos.x,
            targetX,
            sideSpeed * Time.deltaTime
        );

        // Jump and gravity
        if (!grounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            pos.y += verticalVelocity * Time.deltaTime;

            if (pos.y <= groundY)
            {
                pos.y = groundY;
                verticalVelocity = 0f;
                grounded = true;
            }
        }
        else
        {
            pos.y = groundY;
        }

        // Forward movement
        pos.z += forwardSpeed * Time.deltaTime;

        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance == null)
            return;

        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.EndGame();
        }
        else if (other.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Gem"))
        {
            GameManager.Instance.AddGem();
            Destroy(other.gameObject);
        }
    }
}
