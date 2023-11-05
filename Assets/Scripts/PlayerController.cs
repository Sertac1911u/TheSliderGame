using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region DeclareVariables
    [Header("Properties")]
    public float moveSpeed = 6f, maxmoveSpeed = 10f;
    [SerializeField] private bool isMoving = false;
    [Header("Points")]
    [SerializeField] private Transform[] transformPoints;

    [Header("Components and Others")]
    private TrailRenderer trailRenderer;
    private AudioSource audioSource;
    public GameObject blastEffectPrefab;
    [SerializeField] private int currentTransformIndex = 0;
    #endregion

    #region Start
    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();
        trailRenderer.endColor = gameObject.GetComponent<SpriteRenderer>().color;
        trailRenderer.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }
    #endregion

    #region Update


    private void Update()
    {
        //Moving

        if(GameManager.instance.IsGameOver)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    GameManager.instance.StartCoroutine(GameManager.instance.PlayMoveClip());
                    isMoving = true;
                    if (isMoving)
                    {
                        currentTransformIndex++;
                        if (currentTransformIndex >= transformPoints.Length)
                        {
                            currentTransformIndex = 0;
                        }
                    }
                }
            }

            if (isMoving)
            {
                if (currentTransformIndex < transformPoints.Length)
                {
                    Vector3 targetPos = transformPoints[currentTransformIndex].position;
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

                }
            }
        }

    }
    #endregion

    #region Check

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameManager.instance.PlayBlastClip();
            Instantiate(blastEffectPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            GameManager.instance.health--;
            //adsManager.ShowInterstitialAd();

        }
      
        if (collision.gameObject.tag == "Point" || collision.gameObject.tag == "RainBow")
        {
            GameObject blast = Instantiate(blastEffectPrefab, transform.position, Quaternion.identity);
            Color colour = collision.gameObject.GetComponent<SpriteRenderer>().color;
            audioSource.Play();
            trailRenderer.startColor = colour;
            trailRenderer.endColor = colour;

            blast.gameObject.GetComponent<ParticleSystem>().startColor = colour;

            gameObject.GetComponent<SpriteRenderer>().color = colour;

            Destroy(blast, 1f);
        }
    }
    #endregion

    #region IEnumerators

    #endregion
}
