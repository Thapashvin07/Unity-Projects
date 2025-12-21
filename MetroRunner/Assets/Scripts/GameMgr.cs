using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class GameMgr : MonoBehaviour
{
    public static bool gameStart, inGame;
    public GameObject[] terrains;
    public List<GameObject> activeTerrains;
    float TerrainLength = 196f;
    Player player;
    public float spawnZ = 0f;
    int no_of_tracks = 3;
    [SerializeField]
    GameObject[] obstacles, collectables;
    public GameObject[] _obstacles
    {
        get { return obstacles; }
    }
    public GameObject[] _collectables
    {
        get { return obstacles; }
    }
    public static int[] points =
    {
        5,//id 0
        2,
        4,
        7,
        5,
        10,
        6,
        5,
        3
    };
    [SerializeField]
    GameObject mainMenu,leaderBoard;
    public GameObject gameOver;
    public static GameMgr gameMgr;
    public int[] HighScores = new int[3];//0->2 ,,,,highest->lowest
    public Text finalScore;
    public Sprite[] posters;
    [SerializeField]
    Text[] scores;
    // 0=>no 
    // 1=> left 
    // 2=> ri8
    // 3=> all 3 
    // 4=> diagonal
    int[] probForPattern = {30,50,70,85,100};
    public float obstacleProb = 0.35f, collectableProb = 0.65f;
    public bool isQuitGame;
    private void Awake() 
    {
        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
        #else
            Debug.unityLogger.logEnabled = false;
        #endif
        Debug.unityLogger.logEnabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        if (SceneManager.GetActiveScene().name == "TrainScene")
        {
            Debug.Log("inside bfore spawntrack!");
            for (int i = 0; i < no_of_tracks; i++) SpawnTrack();
        }
        gameMgr = this;
        for(int i=0;i<HighScores.Length;i++)
        {
            HighScores[i] = PlayerPrefs.GetInt("HighScore"+i,0);
            Debug.Log("Highscore"+i+":"+HighScores[i]);
        }
        finalScore = gameOver.transform.GetChild(1).Find("Score_Txt").GetComponent<Text>();
        mainMenu.GetComponent<Image>().sprite = posters[Random.Range(0,posters.Count())];
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "TrainScene")
        {
            if (player.transform.position.z - 220 > (spawnZ - no_of_tracks * TerrainLength))
            {
                SpawnTrack();
                DeleteTrack();
            }
        }
    }
    public void StartGame(bool isnewGame = true)
    {
        Debug.Log("[StartGame]");
        inGame = true;
        if(isnewGame)player.ResetPlayer();
        StartCoroutine(MyWaitFunction());
        mainMenu.SetActive(false);
        mainMenu.GetComponent<Image>().sprite = posters[Random.Range(0,posters.Count())];
    }
    IEnumerator MyWaitFunction()
    {
        yield return new WaitForEndOfFrame();
        if(!isQuitGame) gameStart = true;
        gameOver.SetActive(false);
        isQuitGame = false;
    }
    IEnumerator SpawnAfterWait()
    {
        yield return new WaitForEndOfFrame();
        //get ready with new track again!
        for (int i = 0; i < no_of_tracks; i++) SpawnTrack();
    }
    public void ExitGame()
    {
        Debug.Log("[ExitGame]");
        Application.Quit();
    }
    void SpawnTrack()
    {
        GameObject track = Instantiate(terrains[Random.Range(0, terrains.Length)], Vector3.forward * spawnZ, Quaternion.identity);
        activeTerrains.Add(track);
        spawnZ += TerrainLength;
        Debug.Log("inside after spawntrack!");
        SpawnObstacles(track.transform);
    }
    void DeleteTrack()
    {
        Destroy(activeTerrains[0]);
        activeTerrains.RemoveAt(0);
        Debug.Log("inside deletetrack!");
    }
    void SpawnObstacles(Transform track)
    {
        List<int> obstacleSpawnPts = new List<int>();
        int ObjsToBeSpawn = Random.Range(10, 13);
        List<int> availArr = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
        for (int i = 1; i <= ObjsToBeSpawn; i++)
        {
            var randVal = availArr[Random.Range(0, availArr.Count())];
            obstacleSpawnPts.Add(randVal);
            availArr.Remove(randVal);
        }
        int no_of_obstacles = Mathf.RoundToInt(ObjsToBeSpawn * obstacleProb);
        int no_of_collectables = Mathf.RoundToInt(ObjsToBeSpawn * collectableProb);
        Debug.Log("no_of_obstacles:"+no_of_obstacles);
        Debug.Log("no_of_collectables:" + no_of_collectables);
        for (int i = 0; i < no_of_obstacles; i++)
        {
            var go = track.GetChild(track.childCount - 1 - 12 + obstacleSpawnPts[i]).gameObject;
            Debug.Log("go.name:" + go.name + "check val:" + obstacleSpawnPts[i]);
            int count = (go.transform.position.y > 0) ? 3 : obstacles.Count();
            // 0=>no 
            // 1=> left 
            // 2=> ri8
            // 3=> all 3 
            // 4=> diagonal
            var randNum = Random.Range(0,101);
            var obstacleId = Random.Range(0, count);
            if(obstacleId < 4)
            {
                float left_right_space = (obstacleId == 3)?3:1.5f;
                for(int j = 0; j<probForPattern.Count();j++)
                {
                    if(probForPattern[j] >= randNum)
                    {
                        Debug.Log("inside randnum case:"+randNum+" j value :"+j);
                        if(j==1)
                        {
                            //below obstacleid can be changed if we need diff obstacle pattern
                            var obstacle1 = Instantiate(obstacles[obstacleId], go.transform);
                            obstacle1.transform.localPosition = new Vector3(-left_right_space,obstacle1.transform.localPosition.y,obstacle1.transform.localPosition.z);                        // Debug.Break();
                            break;
                        }
                        else if(j==2)
                        {
                            var obstacle1 = Instantiate(obstacles[obstacleId], go.transform);
                            obstacle1.transform.localPosition = new Vector3(left_right_space,obstacle1.transform.localPosition.y,obstacle1.transform.localPosition.z);
                            break;
                        }
                        else if(j==3)
                        {
                            var obstacle1 = Instantiate(obstacles[obstacleId], go.transform);
                            obstacle1.transform.localPosition = new Vector3(-left_right_space,obstacle1.transform.localPosition.y,obstacle1.transform.localPosition.z);
                            var obstacle2 = Instantiate(obstacles[obstacleId], go.transform);
                            obstacle2.transform.localPosition = new Vector3(left_right_space,obstacle2.transform.localPosition.y,obstacle2.transform.localPosition.z);
                            break;
                        }
                        else if(j==4)
                        {
                            var obstacle1 = Instantiate(obstacles[obstacleId], go.transform);
                            obstacle1.transform.localPosition = new Vector3(-left_right_space,obstacle1.transform.localPosition.y,obstacle1.transform.localPosition.z+1.5f);
                            var obstacle2 = Instantiate(obstacles[obstacleId], go.transform);
                            obstacle2.transform.localPosition = new Vector3(left_right_space,obstacle2.transform.localPosition.y,obstacle2.transform.localPosition.z+1.5f);
                            break;
                        }
                    }
                }
            }
            var obstacle = Instantiate(obstacles[obstacleId], go.transform);
            if(obstacleId == 3 || obstacleId == 4)
            {
                var spawnAbove = Random.Range(0,101);
                if(spawnAbove > 20)
                {
                    if(spawnAbove > 70)
                    {
                        // var abv_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], obstacle.transform.GetChild(0));
                        var bfore_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], go.transform);
                        bfore_obstacle.transform.localPosition  = new Vector3(bfore_obstacle.transform.localPosition.x,bfore_obstacle.transform.localPosition.y,obstacle.transform.localPosition.z - 1.5f);
                        // abv_obstacle.transform.localPosition = obstacles[obstacleId].GetChild(0)
                        var after_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], go.transform);
                        after_obstacle.transform.localPosition  = new Vector3(after_obstacle.transform.localPosition.x,after_obstacle.transform.localPosition.y,go.transform.localPosition.z + 1.5f);
                        var after_obstacle1 = Instantiate(collectables[Random.Range(0,collectables.Count())], go.transform);
                        after_obstacle1.transform.localPosition  = new Vector3(after_obstacle1.transform.localPosition.x,after_obstacle1.transform.localPosition.y,go.transform.localPosition.z + 1.5f*2);
                    }
                    else if(spawnAbove > 40)
                    {
                        // var abv_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], obstacle.transform.GetChild(0));
                        var bfore_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], go.transform);
                        bfore_obstacle.transform.localPosition  = new Vector3(bfore_obstacle.transform.localPosition.x,bfore_obstacle.transform.localPosition.y,obstacle.transform.localPosition.z - 1.5f);
                        // abv_obstacle.transform.localPosition = obstacles[obstacleId].GetChild(0)
                        var after_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], go.transform);
                        after_obstacle.transform.localPosition  = new Vector3(after_obstacle.transform.localPosition.x,after_obstacle.transform.localPosition.y,go.transform.localPosition.z + 1.5f);
                    }
                    // else
                    // {
                    //     var abv_obstacle = Instantiate(collectables[Random.Range(0,collectables.Count())], obstacle.transform.GetChild(0));
                    // }
                }
            }
            Debug.Log("obstacle name:" + obstacle.name);
        }
        for (int i = no_of_obstacles; i < no_of_collectables; i++)
        {
            var go = track.GetChild(track.childCount - 1 - 12 + obstacleSpawnPts[i]).gameObject;
            Debug.Log("go.name:" + go.name + "check val:" + obstacleSpawnPts[i]);
            // 0=>no 
            // 1=> left 
            // 2=> ri8
            // 3=> all 3 
            // 4=> diagonal
            var randNum = Random.Range(0,101);
            // randNum = 15;
            var collectableId = Random.Range(0, collectables.Count());
            for(int j = 0; j<probForPattern.Count();j++)
            {
                if(probForPattern[j] >= randNum)
                {
                    Debug.Log("inside randnum case:"+randNum+" j value :"+j);
                    if(j==1)
                    {
                        var rand = Random.Range(1,4);
                        for(int itr = 0; itr < rand; itr++)
                        {
                            var collectable1 = Instantiate(collectables[collectableId], go.transform);
                            collectable1.transform.localPosition = new Vector3(-1.5f,collectable1.transform.localPosition.y,collectable1.transform.localPosition.z+(1.5f*itr));
                        }
                        break;
                    }
                    else if(j==2)
                    {
                       var rand = Random.Range(1,4);
                        for(int itr = 0; itr < rand; itr++)
                        {
                            var collectable1 = Instantiate(collectables[collectableId], go.transform);
                            collectable1.transform.localPosition = new Vector3(1.5f,collectable1.transform.localPosition.y,collectable1.transform.localPosition.z+(1.5f*itr));
                        }
                        break;
                    }
                    else if(j==3)
                    {
                        var collectable1 = Instantiate(collectables[collectableId], go.transform);
                        collectable1.transform.localPosition = new Vector3(-1.5f,collectable1.transform.localPosition.y,collectable1.transform.localPosition.z);
                        var collectable2 = Instantiate(collectables[collectableId], go.transform);
                        collectable2.transform.localPosition = new Vector3(1.5f,collectable2.transform.localPosition.y,collectable2.transform.localPosition.z);
                        break;
                    }
                    else if(j==4)
                    {
                        var collectable1 = Instantiate(collectables[collectableId], go.transform);
                        collectable1.transform.localPosition = new Vector3(-1.5f,collectable1.transform.localPosition.y,collectable1.transform.localPosition.z+1.5f);
                        var collectable2 = Instantiate(collectables[collectableId], go.transform);
                        collectable2.transform.localPosition = new Vector3(1.5f,collectable2.transform.localPosition.y,collectable2.transform.localPosition.z+1.5f);
                        break;
                    }
                }
            }
            var collectable = Instantiate(collectables[collectableId], go.transform);
            var no_of_fwdspawns = Random.Range(0,101);
            if(no_of_fwdspawns>25 && no_of_fwdspawns <= 50)
            {
                var collectable1 = Instantiate(collectables[collectableId], go.transform);
                collectable1.transform.localPosition = new Vector3(collectable1.transform.localPosition.x,collectable1.transform.localPosition.y,collectable1.transform.localPosition.z+1.5f);
            }
            else if(no_of_fwdspawns > 50)
            {
                var collectable1 = Instantiate(collectables[collectableId], go.transform);
                collectable1.transform.localPosition = new Vector3(collectable1.transform.localPosition.x,collectable1.transform.localPosition.y,collectable1.transform.localPosition.z+1.5f); 

                var collectable2 = Instantiate(collectables[collectableId], go.transform);
                collectable2.transform.localPosition = new Vector3(collectable2.transform.localPosition.x,collectable2.transform.localPosition.y,collectable2.transform.localPosition.z+3f);
            }
            Debug.Log("collectable name:" + collectable.name);
        }
    }
    public void GoToMenu(bool gotomenu = true)
    {
        if (gotomenu)
        {
            gameOver.SetActive(false);
            mainMenu.SetActive(true);
        }
        player.transform.position = new Vector3(0, -1.39999998f, -98);
        for (int i = 0; i < activeTerrains.Count; i++)
        {
            DeleteTrack();
        }
        gameStart = inGame = false;
        spawnZ = 0f;
        StartCoroutine(SpawnAfterWait());
    }
    /*
    //this is for getting highscores
    for(int i=0;i<HighScores.Length;i++)
    {
        HighScores[i] = PlayerPrefs.GetInt("HighScore"+i,0);
        Debug.Log("Highscore"+i+":"+HighScores[i]);
    }
    */
    public void ShowLeaderBoard()
    {
        leaderBoard.SetActive(true);
        for(int i=0;i<HighScores.Length;i++)
        {
            HighScores[i] = PlayerPrefs.GetInt("HighScore"+i,0);
            Debug.Log("Highscore"+i+":"+HighScores[i]);
            scores[i].text = HighScores[i].ToString();
        }
    }
    public void PauseGame()
    {
        if(player._animator!=null)
        {
            player._animator.enabled = false;
            player.canProcessInput = false;
            player.tapGo.SetActive(true);
            gameStart = false;
        }
    }
    public void ResumeGame()
    {
        StartCoroutine(MyWaitFunction());
    }
    public void RestartGame()
    {
        player.ResetPlayer();
        GoToMenu(false);
        StartGame();
        player._animator.enabled = true;
        player._animator.SetBool("Idle",true);
    }
    public void SetHighScore(int points)
    {
        //this is for updating highscores
        if (points > HighScores[0])
        {
            HighScores[2] = HighScores[1];
            HighScores[1] = HighScores[0];
            HighScores[0] = points;
        }
        else if (points > HighScores[1])
        {
            HighScores[2] = HighScores[1];
            HighScores[1] = points;
        }
        else if (points > HighScores[2])
        {
            HighScores[2] = points;
        }
        for (int i = 0; i < HighScores.Length; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, HighScores[i]);
        }
    }
    public void QuitGame()
    {
        isQuitGame = true;
    }
}