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
    private UIHandler uiHandler;

    public GameObject loadingScreen;
    public GameObject HUD;
    public Slider progressSlider;
    public TextMeshProUGUI scoreText, speedText;
    public TextMeshProUGUI warningText;

    public float score;
    private float minMinSpeed;
    public float maxAltitude = 200f;
    public float maxCountDown = 3f;
    private float countDown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        minMinSpeed = player.GetComponent<ShipMovement>().minSpeed;
        mapGenerator = GetComponent<MapGenerator>();
        uiHandler = GetComponent<UIHandler>();

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
        HUD.SetActive(true);
    }

    void Update()
    {
        if(player != null){
            if(player.GetComponent<ShipMovement>().alive == false){
                player.GetComponent<ShipMovement>().DeactivateInput();
                mapGenerator.enabled = false;
                Destroy(player, 1f);
                SaveHighScore();
            }else{
                float playerVel = player.GetComponent<Rigidbody>().velocity.magnitude;
                score += Time.deltaTime * playerVel;

                // Keep increasing player speed with score
                player.GetComponent<ShipMovement>().minSpeed = minMinSpeed + score/200f;
                player.GetComponent<ShipMovement>().maxSpeed = player.GetComponent<ShipMovement>().minSpeed + 200f;

                scoreText.text = score.ToString("0") + " m";
                speedText.text = playerVel.ToString("0.0") + " m/s";
            }
        }else{
            uiHandler.GameOver();
            HUD.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if(player != null){
            float altitude = player.transform.position.y;
            
            if(altitude > maxAltitude){
                warningText.text = "You are flying too high. Game over in " + countDown.ToString("0.0") + "s";
                countDown -= Time.deltaTime;
            }else{
                countDown = maxCountDown;
                warningText.text = "";
            }
            
            if(countDown <= 0){
                uiHandler.GameOver();
                HUD.SetActive(false);
            }
        }
    }

    void SaveHighScore(){
        if(PlayerPrefs.GetFloat("HighScore") < score){
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }


}
