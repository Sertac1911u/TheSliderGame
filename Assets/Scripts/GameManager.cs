using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region DeclareVariables
    //GameManager making singleton object
    public static GameManager instance { get; private set; }

    [Header("Properties")]
    public bool IsGameOver = false;
    public bool SecondChange;

    [Header("Randomizer")]
    [SerializeField] private bool isRandomingColor;
    public bool isGameStarted;

    [Header("Score")]
    [SerializeField] private float score;
    public int health;

    [Header("Sound/SFX")]
    [SerializeField] private AudioClip blastClip;
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private AudioClip bestScoreClip;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI gameInPointText, gameEndPointText, bestScoreText, newOrGameOverText;
    [SerializeField] private GameObject backGround, loseBackGround, bestScorePanel, RePlayPanel, UI, player, menuPanel;
    public GameObject adPanel;
    public Button adRefuseText;
    [SerializeField] private Button soundButton, adButton;
    [SerializeField] private Sprite[] soundImg;
    [SerializeField] private TextMeshProUGUI[] soundText;
    [SerializeField] private GameObject[] effects;

    [Header("Components and Scripts")]
    [SerializeField] private AudioSource audioSource, playerAudioSource;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AdsManager adsManager;
    [SerializeField] private SpawnManager spawnManager;



    //Create four different
    Color[] colors =
    {
        new Color32(52,168,83,255),
        new Color32(251,188,5,255),
        new Color32(234,67,53,255),
        new Color32(66,133,244,255)
    };
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    #endregion

    #region Start

    private void Start()
    {
        Application.targetFrameRate = 60;
        adsManager.LoadLoadInterstitialAd();

        float randomEffect = UnityEngine.Random.Range(0, 2);
        int randomEffectIndex = (int)randomEffect;
        effects[randomEffectIndex].SetActive(true);

        score = 0;

        int soundMusic = PlayerPrefs.GetInt("SoundMuted", 0);

        if (soundMusic == 1)
        {
            audioSource.mute = true;
            playerAudioSource.mute = true;
            soundButton.gameObject.GetComponent<Image>().sprite = soundImg[1];
            soundText[0].gameObject.SetActive(true);
            soundText[1].gameObject.SetActive(false);
        }

    }
    #endregion

    #region Update
    private void Update()
    {
        if (isGameStarted)
        {
            StartCoroutine(Play());
            spawnManager.gameObject.SetActive(true);

        }
        gameInPointText.text = score.ToString();

        if (IsGameOver)
        {
            StartCoroutine(GameOver());
        }

        if (health == 0)
        {
            SecondChange = false;
            IsGameOver = true;
        }
    }
    #endregion

    #region IEnumerators


    private IEnumerator Play()
    {
        // Menü panelini gizle.
        menuPanel.gameObject.SetActive(false);

        // Oyun içi puan yazısını göster.
        if (!IsGameOver)
            gameInPointText.gameObject.SetActive(true);

        float durationPlay = 0.5f;

        Vector3 targetPos = backGround.transform.position;
        Vector3 initialPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);

        float elapsedTimePlay = 0;

        while (elapsedTimePlay < durationPlay)
        {
            Camera.main.transform.position = Vector3.Lerp(initialPos, targetPos, elapsedTimePlay / durationPlay);
            elapsedTimePlay += Time.deltaTime;
            yield return null;
        }
        // Camera'nın hareketini tamamla.
        Camera.main.transform.position = targetPos;



    }
    private IEnumerator GameOver()
    {
        if (health == 0)
        {
            yield return new WaitForSeconds(1f);

            float duration = 0.1f;
            Vector3 targetPos = loseBackGround.transform.position;

            Vector3 initialPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                Camera.main.transform.position = Vector3.Lerp(initialPos, targetPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (PlayerPrefs.GetFloat("HighScore") < score)
            {
                PlayerPrefs.SetFloat("HighScore", score);
                bestScoreText.text = PlayerPrefs.GetFloat("HighScore").ToString();
                newOrGameOverText.text = "NEW BEST!";
                audioSource.PlayOneShot(bestScoreClip);
            }
            else if (score < PlayerPrefs.GetFloat("HighScore"))
            {
                bestScoreText.text = PlayerPrefs.GetFloat("HighScore").ToString();
                newOrGameOverText.text = "GAME OVER";
            }

            Camera.main.transform.position = targetPos;

            gameInPointText.gameObject.SetActive(false);
            gameEndPointText.gameObject.SetActive(true);
            newOrGameOverText.gameObject.SetActive(true);
            bestScorePanel.gameObject.SetActive(true);
            UI.gameObject.SetActive(true);
            gameEndPointText.text = gameInPointText.text;
            RePlayPanel.SetActive(true);
        }
    }
    #endregion

    #region Sounds

    public void PlayButtonClip()
    {
        audioSource.PlayOneShot(buttonClip);
    }
    public void PlayBlastClip()
    {
        audioSource.PlayOneShot(blastClip);
    }
    public IEnumerator PlayMoveClip()
    {
        audioSource.PlayOneShot(moveClip);
        yield return new WaitForSeconds(0.4f);
    }

    public void SoundsOff()
    {
        if (audioSource.mute && playerAudioSource.mute)
        {
            audioSource.mute = false;
            playerAudioSource.mute = false;
            soundButton.gameObject.GetComponent<Image>().sprite = soundImg[0];
            PlayerPrefs.SetInt("SoundMuted", 0);
            soundText[0].gameObject.SetActive(false);
            soundText[1].gameObject.SetActive(true);
        }

        else if (!audioSource.mute && !playerAudioSource.mute)
        {
            audioSource.mute = true;
            playerAudioSource.mute = true;
            soundButton.gameObject.GetComponent<Image>().sprite = soundImg[1];
            PlayerPrefs.SetInt("SoundMuted", 1);
            soundText[0].gameObject.SetActive(true);
            soundText[1].gameObject.SetActive(false);
        }
    }
    public void RateButton()
    {
        var url = "https://play.google.com/store/apps/details?id=com.your.app.id";
        Application.OpenURL(url);
    }

    public void RemoveAdButton()
    {
        //console aldıktan sonra
    }
    #endregion

    #region Game

    public void StartButton()
    {
        isGameStarted = true;
    }
    public void PlayAgain()
    {
        if (IsGameOver)
        {
            SceneManager.LoadScene(0);
        }
    }
    public void RefuseAd()
    {
        adPanel.SetActive(false);
        SecondChange = false;
        health = 0;
        IsGameOver = true;
    }

    public void ShowAd()
    {
        if (SecondChange && health >= 0)
        {
            adsManager.ShowInterstitialAd();
            adPanel.gameObject.SetActive(false);
            adRefuseText.gameObject.SetActive(false);
            SecondChange = false;
            //Instantiate(player, transform.position, Quaternion.identity);
            player.gameObject.SetActive(true);
        }
    }
    public Color RandomColor()
    {
        int index = UnityEngine.Random.Range(0, colors.Length);
        return colors[index];
    }

    public void IncreaseScore(float plusScore)
    {
        score += plusScore;
    }
    #endregion

    #region UI

    #endregion

}
