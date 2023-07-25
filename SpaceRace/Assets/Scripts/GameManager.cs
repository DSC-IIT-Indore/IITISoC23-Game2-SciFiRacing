using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    private MapGenerator mapGenerator;

    public GameObject loadingScreen;
    public Slider progressSlider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mapGenerator = GetComponent<MapGenerator>();
        loadingScreen = GameObject.Find("Loading");
        StartCoroutine(FillSlider());
        
        mapGenerator.enabled = true;
        player.GetComponent<ShipMovement>().enabled = true;
        player.GetComponent<ShipMovement>().ActivateInput();
    }

    IEnumerator FillSlider()
    {
        for(int i=0; i <= 100; i += Random.Range(1, 5)){
            progressSlider.value = i/100f;
            yield return new WaitForSeconds(0.01f);
        }
        loadingScreen.SetActive(false);
    }

    void Update()
    {
        
    }
}
