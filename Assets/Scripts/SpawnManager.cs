using System.Collections;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnPrefabs;

    [SerializeField] private float startSpawnCoolDown, spawnCount, spawnWait, WaveWait, wave = 0f;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Y Spawn Range")]
    [SerializeField] private float xMin, xMax;

    private void Start()
    {
        StartCoroutine(StartSpawn());
    }



    private IEnumerator StartSpawn()
    {
        yield return new WaitForSeconds(startSpawnCoolDown);
        while (GameManager.instance.IsGameOver == false)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = "Wave" + " " + wave;
            yield return new WaitForSeconds(1f);
            waveText.gameObject.SetActive(false);
            wave++;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(xMin, xMax), transform.position.y);
                Quaternion spawnRotation = Quaternion.identity;

                Color colour = GameManager.instance.RandomColor();

                int randomIndex = Random.Range(0, 3);
                GameObject prefab = Instantiate(spawnPrefabs[randomIndex], spawnPosition, spawnRotation);


                spawnPrefabs[0].gameObject.GetComponent<SpriteRenderer>().color = colour;

                yield return new WaitForSeconds(spawnWait);

            }
            spawnCount++;
            yield return new WaitForSeconds(WaveWait);
        }

    }

}
