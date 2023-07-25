using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    private MapGenerator mapGenerator;

    public GameObject loadingScreen;
    public Slider progressSlider;
    public TextMeshProUGUI scoreText, speedText;

    public float score;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mapGenerator = GetComponent<MapGenerator>();
        loadingScreen = GameObject.Find("Loading");
        StartCoroutine(FillSlider());
        
        mapGenerator.enabled = true;
    }

    IEnumerator FillSlider()
    {
        for(int i=0; i <= 100; i += Random.Range(1, 5)){
            progressSlider.value = i/100f;
            yield return new WaitForSeconds(0.01f);
        }
        player.GetComponent<ShipMovement>().enabled = true;
        player.GetComponent<ShipMovement>().ActivateInput();
        loadingScreen.SetActive(false);
    }

    void Update()
    {
        if(player != null && player.GetComponent<ShipMovement>().alive == false){
            player.GetComponent<ShipMovement>().DeactivateInput();
            mapGenerator.enabled = false;
            Destroy(player, 1f);
        }else{
            float playerVel = player.GetComponent<Rigidbody>().velocity.magnitude;
            score += Time.deltaTime * playerVel;
            scoreText.text = score.ToString("0") + " m";
            speedText.text = playerVel.ToString("0.0") + " m/s";
        }

    }
}
