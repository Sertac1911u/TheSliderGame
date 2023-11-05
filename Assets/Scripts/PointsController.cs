using UnityEngine;

public class PointsController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float moveSpeed, minmoveSpeed, maxmoveSpeed, rotationSpeed, maxRotationSpeed, minRotationSpeed;
    [SerializeField] private bool isBlast = false;
    public Color color;

    private int currentColorIndex = 0;
    private float nextColorChangeTime = 0f;
    Color[] colors =
    {
        new Color32(52,168,83,255),
        new Color32(251,188,5,255),
        new Color32(234,67,53,255),
        new Color32(66,133,244,255)
    };
    private void Start()
    {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        color = GetComponent<SpriteRenderer>().color;
        moveSpeed = Random.Range(minmoveSpeed, maxmoveSpeed);
    }

    private void Update()
    {
        if (!isBlast)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            Vector3 newPos = transform.position;
            newPos.y -= moveSpeed * Time.deltaTime;
            transform.position = newPos;

            if (gameObject.tag == "RainBow")
            {
                if (Time.time >= nextColorChangeTime)
                {
                    currentColorIndex = (currentColorIndex + 1) % colors.Length;
                    nextColorChangeTime = Time.time + 0.1f;

                    gameObject.GetComponent<SpriteRenderer>().color = colors[currentColorIndex];
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && gameObject.tag == "Point")
        {
            BoxCollider2D boxcollider = gameObject.GetComponent<BoxCollider2D>();
            boxcollider.enabled = false;


            isBlast = true;

            float RandomScore = Random.Range(5, 15);
            GameManager.instance.IncreaseScore(RandomScore);

            Destroy(gameObject);

        }
        if (collision.tag == "Player" && gameObject.tag == "RainBow")
        {
            BoxCollider2D boxcollider = gameObject.GetComponent<BoxCollider2D>();
            boxcollider.enabled = false;


            isBlast = true;

            float RandomScore = Random.Range(5, 15);
            GameManager.instance.IncreaseScore(RandomScore);

            Destroy(gameObject);

        }
        if (collision.tag == "Player" && gameObject.tag == "Enemy")
        {
            if(GameManager.instance.health == 2)
            {
                GameManager.instance.adPanel.gameObject.SetActive(true);
                GameManager.instance.adRefuseText.gameObject.SetActive(true);
                GameManager.instance.SecondChange = true;
            }

                

            BoxCollider2D boxcollider = gameObject.GetComponent<BoxCollider2D>();
            boxcollider.enabled = false;

            isBlast = true;


            //cameraController.StartCoroutine(cameraController.CameraShake(0.2f, 0.2f));

            Destroy(gameObject);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "CheckCollider")
        {
            Destroy(gameObject);
        }
    }
}
