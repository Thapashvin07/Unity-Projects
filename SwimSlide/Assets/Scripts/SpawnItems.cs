using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class SpawnItems : MonoBehaviour
{
    int minY=-3,maxX=18,maxY=3;
    float spawnFishTimeInterval = 3f;//this should b converted as an array based on category of fishes
    float spawnPowerTimeInterval = 7f;
    float lastFishSpawnTime,lastPowerSpawnTime;
    public GameObject[] Fishes;
    public GameObject[] Powers;
    public Image health_bar;
    public GameObject GameCanvas,MainMenuCanvas,StartPanel;
    public static SpawnItems spawnItems;
    public int[] FishesProbablity, PowersProbablity;
    public bool gameStart;
    [SerializeField]
    GameObject gameOver;
    public int[] HighScores = new int[3];//0->2 ,,,,highest->lowest
    [SerializeField]
    float GameStartimer;
    [SerializeField]
    GameObject LeaderBoard;
    GameObject HighScoreParent;
    public static float playerSpeed=1f, obstacleSpeed=1.5f;
    public float speedIncreaseTimer;
    void Start()
    {
        Debug.Log("Screen.height:"+Screen.height+"Screen.width:"+Screen.width);
        if(spawnItems == null)spawnItems = this;
        // lastPowerSpawnTime = lastFishSpawnTime = Time.time;this is done when game is started!
        GameCanvas = GameObject.Find("Game_Canvas");
        MainMenuCanvas = GameObject.Find("GameStart_Canvas");
        StartPanel = GameObject.Find("start_panel").gameObject;
        health_bar = GameCanvas.transform.Find("Health_Bar").transform.Find("Health_Filling").GetComponent<Image>();
        HighScoreParent = LeaderBoard.transform.Find("Scores_parent").gameObject;
        for(int i=0;i<HighScores.Length;i++)
        {
            HighScores[i] = PlayerPrefs.GetInt("HighScore"+i,0);
            Debug.Log("Highscore"+i+":"+HighScores[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameStart) return;
        if(Time.time - lastFishSpawnTime > spawnFishTimeInterval)
        {
            Debug.Log("im inside update()");
            SpawnItem(Fishes[Random.Range(0, Fishes.Length)]);
            lastFishSpawnTime = Time.time;
        }
        if(Time.time - lastPowerSpawnTime > spawnPowerTimeInterval)
        {
            Debug.Log("im inside update()");
            SpawnItem(Powers[Random.Range(0, Powers.Length)]);
            lastPowerSpawnTime = Time.time;
        }
        if(Time.time - speedIncreaseTimer > 10)
        {
            playerSpeed+=Time.deltaTime;
            speedIncreaseTimer = Time.time; 
            obstacleSpeed+= Time.deltaTime;
        }
    }
    public void SpawnItem(GameObject fish)
    {
        fish = Instantiate(fish,this.transform.position+new Vector3(maxX,Random.Range(minY,maxY)),Quaternion.Euler(0,180f,0));
    }
    public void UpdateHealthBar(float health)
    {
        if(health_bar!=null)
        {
            health_bar.fillAmount = health/100f;
        }
    }
    public void HideMainMenu()
    {
        gameStart = true;
        health_bar.transform.parent.gameObject.SetActive(true);
        MainMenuCanvas.SetActive(false);
        StartPanel.SetActive(false);
        if(!gameOver.activeInHierarchy) gameOver.SetActive(false);
        if(!LeaderBoard.activeInHierarchy) LeaderBoard.SetActive(false);
        GameStartimer = lastPowerSpawnTime = lastFishSpawnTime = speedIncreaseTimer = Time.time;
        this.transform.Find("Player").GetComponent<PlayerMovement>().health = 100f;
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.5f));
        // transform.position = worldPos;
        this.transform.Find("Player").GetComponent<PlayerMovement>().targetPos = worldPos;
        this.transform.Find("Player").GetComponent<PlayerMovement>().transform.position = worldPos;
        playerSpeed = 1f;
        obstacleSpeed = 1.5f;
    }
    public void ShowGameOver()
    {
        if(!gameStart) return;
        gameStart = false;
        var fishes = GameObject.FindGameObjectsWithTag("Obstacles");
        foreach(var obj in fishes)
        {
            if(obj!=null)Destroy(obj);
        }
        var splpowers = GameObject.FindObjectsOfType(typeof(SpecialPowers));
        foreach(var obj in splpowers)
        {
            if(obj!=null)Destroy(obj);
        }
        float GameEndTimer = Time.time - GameStartimer;
        health_bar.transform.parent.gameObject.SetActive(false);
        gameOver.transform.Find("EndScore").GetComponentInChildren<Text>().text = ((int)GameEndTimer).ToString();
        MainMenuCanvas.SetActive(true);
        gameOver.SetActive(true);
        //getting top scores and performing sorting
        if(((int)GameEndTimer) > HighScores[0])
        {
            HighScores[2] = HighScores[1];
            HighScores[1] = HighScores[0];
            HighScores[0] = (int)GameEndTimer; 
        }
        else if(((int)GameEndTimer) > HighScores[1])
        {
            HighScores[2] = HighScores[1];
            HighScores[1] = (int)GameEndTimer;
        }
        else if(((int)GameEndTimer)> HighScores[2])
        {
            HighScores[2] = (int)GameEndTimer;
        }
        for(int i=0;i<HighScores.Length;i++)
        {
            PlayerPrefs.SetInt("HighScore"+i,HighScores[i]);
            // Debug.Log("scores"+PlayerPrefs.GetInt("HighScore"+i,0));
        }
    }
    public void showMainMenu()
    {
        gameOver.SetActive(false);
        LeaderBoard.SetActive(false);
        StartPanel.SetActive(true);
    }
    public void showLeaderBoard()
    {
        gameOver.SetActive(false);
        for(int i=0;i<HighScores.Length;i++)
        {
            HighScoreParent.transform.GetChild(i).GetComponentInChildren<Text>().text = HighScores[i].ToString();
        }
        LeaderBoard.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
