using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class Player : NetworkBehaviour
{
    //LOCAL VARIABLES
    [SerializeField] GameObject cardPrefab;
    
    Transform content;
    public List<Card> myCards = new List<Card>();
    public enum CardAbilityType
    {
        GainPts,
        DoublePts,
        TriplePts,
        MakeOppMinusTwo
    }
    [SerializeField] GameObject LobbyScreen, GameOver;
    bool start;
    [SerializeField] TMP_Text my_scoreText,opp_scoreText, timerText, turnText;
    [SerializeField] public int costAdded = 0;
    [SerializeField] public Dictionary<GameObject,int> clickedCost = new Dictionary<GameObject, int>();
    [SerializeField] GameObject playCardBtn, end_Turn, Re_SelectBtn;
    [SerializeField] Transform my_RevealScroll_Content, opp_RevealScroll_Content;
    [SerializeField]int count = -1;
    // [SerializeField] CardClicks cardClicks;


    //NETWORKED VARIABLES
    [Networked]public bool isMaster{get;set;}
    [Networked]public float Timer{get;set;}
    [Networked]public int score{get;set;}
    [Networked]public int opp_score{get;set;}
    [Networked, OnChangedRender(nameof(DisableInApprCostCards))]public int turnCount{get;set;}
    [Networked] public TickTimer tickTimer{get;set;}
    [Networked, OnChangedRender(nameof(GameStartOrEndForPlayers))] public string GameStartOrEnd{get;set;}
    [Networked, OnChangedRender(nameof(RevealEvent))] public string RevealInfo{get;set;}
    [Networked, OnChangedRender(nameof(ReduceScore))] public int minusScore{get;set;}

    public override void Spawned()
    {
        Debug.Log("A new player has joined: " +HasStateAuthority);
        Debug.Log("Activeplayers: "+Runner.ActivePlayers.Count());
        my_scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
        opp_scoreText = GameObject.Find("OppScore").GetComponent<TMP_Text>();
        timerText = GameObject.Find("Time").GetComponent<TMP_Text>();
        turnText = GameObject.Find("Turn").GetComponent<TMP_Text>();
        LobbyScreen = GameObject.Find("Canvas").transform.GetChild(5).gameObject;
        GameOver = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
        content = GameObject.Find("My_Scroll").transform.GetChild(0).GetChild(0);
        playCardBtn = GameObject.Find("PlayCard").gameObject;
        Re_SelectBtn = GameObject.Find("ReSelect").gameObject;
        my_RevealScroll_Content = GameObject.Find("My_RevealScroll").transform.GetChild(0).GetChild(0);
        opp_RevealScroll_Content = GameObject.Find("Opp_Scroll").transform.GetChild(0).GetChild(0);
        end_Turn = GameObject.Find("EndTurn").gameObject;
        if(Runner.IsSharedModeMasterClient) {
            isMaster = true;
            turnCount = -1;
        }
        if(HasStateAuthority)
        // if(isMaster)
        {
            Debug.Log("A new player has joined2: " + HasStateAuthority+"named:"+this.gameObject.name);
            for(int i=0;i<4;i++)
            {
                int RandCardId = 0;
                int costID = -1;
                if(i==0)//cost 1 card
                {
                    RandCardId = UnityEngine.Random.Range(0, 3);
                }
                else if(i==1)
                {
                    RandCardId = UnityEngine.Random.Range(3, 6);
                }
                else if(i==2)
                {
                    RandCardId = UnityEngine.Random.Range(6, 9);
                }
                else
                {
                    RandCardId = UnityEngine.Random.Range(3, Menu.DeckCards.Count);
                }
                int abilityIndex = 0, score = 0;
                GameObject cardObj = null;
                if(i==0 || i==1 || i==2)
                {
                    myCards.Add(Menu.InitSpawncards[RandCardId]);
                    cardObj = Instantiate(cardPrefab, content);
                    cardObj.transform.GetChild(0).GetComponent<TMP_Text>().text = Menu.InitSpawncards[RandCardId].Name;
                    cardObj.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = Menu.InitSpawncards[RandCardId].cost.ToString();

                    abilityIndex = Menu.InitSpawncards[RandCardId].ability.type;
                    score = Menu.InitSpawncards[RandCardId].ability.value;
                    costID = Menu.InitSpawncards[RandCardId].cost;
                }
                else
                {
                    myCards.Add(Menu.DeckCards[RandCardId]);
                    cardObj = Instantiate(cardPrefab, content);
                    cardObj.transform.GetChild(0).GetComponent<TMP_Text>().text = Menu.DeckCards[RandCardId].Name;
                    cardObj.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = Menu.DeckCards[RandCardId].cost.ToString();

                    abilityIndex = Menu.DeckCards[RandCardId].ability.type;
                    score = Menu.DeckCards[RandCardId].ability.value;
                    costID = Menu.DeckCards[RandCardId].cost;
                }
                cardObj.GetComponent<Button>().onClick.AddListener(() =>CardClicked(RandCardId,costID));

                if(abilityIndex == (int)CardAbilityType.GainPts)
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = score.ToString();
                }
                else if(abilityIndex == (int)CardAbilityType.DoublePts)
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text =(score * 2). ToString();
                }
                else if(abilityIndex == (int)CardAbilityType.TriplePts)
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "3 X "+score;
                }
                else
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "- OPP"+score;
                }
            }
            playCardBtn.GetComponent<Button>().onClick.AddListener(()=>PlayCard());
            end_Turn.GetComponent<Button>().onClick.AddListener(()=>EndTurn());
            Re_SelectBtn.GetComponent<Button>().onClick.AddListener(()=>ReSelectCards());
            Debug.Log("Adding onclick function" +HasStateAuthority);
            LobbyScreen.SetActive(true);
            GameOver.SetActive(false);
        }
    }
    //NETWORKED ONCHANGED EVENTS
    void DisableInApprCostCards()
    {
        // if(turnCount==-1) return;
        // if(turnCount>1)count +=1;
        // if(HasStateAuthority)
        // {
            // if(playCardBtn.GetComponent<Button>().onClick.GetPersistentEventCount() > 0)SpawnCardFromDeck();
        // }
        playCardBtn.SetActive(true);
        clickedCost.Clear();
        costAdded = 0;
        foreach(Transform val in content)
        {
            val.GetComponent<Button>().interactable = true;
        }
        // if(Object.HasStateAuthority)
        // {
            int index = 0;
            foreach(Transform val in content)
            {
                int costVal = int.Parse(val.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text);
                if(costVal > turnCount){
                    val.GetComponent<Button>().interactable = false;
                }
                index++;
            }
        // }
    }
    void GameStartOrEndForPlayers()
    {
        // if(!HasStateAuthority) return;
        // count = 1;
        // if(GameStartOrEnd.Count()==0) return;
        Debug.Log("[GameStartOrEndForPlayers]"+GameStartOrEnd);
        var info = GameStartOrEnd;
        if(info.Count()>0 && info[info.Count()-1]!='}') info+="}";
        Debug.Log("[GameStartOrEndForPlayers2]:"+info);
        var startorend = JsonConvert.DeserializeObject<GameStartEnd>(info);
        if(startorend!=null)
        {
            if(startorend.isStart)
            {
                Debug.Log("Game is starting!");
                LobbyScreen.SetActive(false);
                if(isMaster && HasStateAuthority)
                {
                    StartTimer();
                    Debug.Log("Game is starting2!");
                    turnCount = 1;//initially
                }
            }
            else
            {
                Debug.Log("Game is ending!");
                GameOver.SetActive(true);
                //handle who win!
                var my_score = int.Parse(my_scoreText.text.Split(":")[1]);
                int opp_score = -1;
                if(opp_scoreText.text.Count() > 10) opp_score = int.Parse(opp_scoreText.text.Split(":")[1]);
                Debug.Log("My score: "+my_score);
                Debug.Log("Opponent Score: "+opp_score);
                var result = GameOver.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text;
                if(my_score > opp_score)
                {
                    result = "You WIN!";
                }
                else if(my_score < opp_score)
                {
                    result = "GOOD LUCK NEXT TIME!";
                }
                else
                {
                    //TIE CASE
                    result = (UnityEngine.Random.Range(0, 2) == 0)? "GOOD LUCK NEXT TIME!":"You WIN!";
                }
            }
        }
    }
    void RevealEvent()
    {
        var info = RevealInfo;
        if(info.Count()>0 && info[info.Count()-1]!='}') info+="}";
        var reveal = JsonConvert.DeserializeObject<RevealCard>(info);
        if(reveal==null) return;
        clickedCost.Clear();
        costAdded = 0;
        Transform parent = null;
        if(HasStateAuthority)
        {
            Debug.Log("[RevealEvent] for myself");
            parent = my_RevealScroll_Content;
        }
        else
        {
            parent = opp_RevealScroll_Content;
        }
        foreach(var val in reveal.cardID)
        {
            if(val!=-1)
            {
                var cardObj = Instantiate(cardPrefab, parent);
                cardObj.transform.GetChild(0).GetComponent<TMP_Text>().text = Menu.DeckCards[val].Name;
                cardObj.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = Menu.DeckCards[val].cost.ToString();
                int abilityIndex = Menu.DeckCards[val].ability.type;
                int _score = Menu.DeckCards[val].ability.value;
                var negScore = false;
                if(abilityIndex == (int)CardAbilityType.GainPts)
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = _score.ToString();
                }
                else if(abilityIndex == (int)CardAbilityType.DoublePts)
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text =(_score * 2). ToString();
                    _score*=2;
                }
                else if(abilityIndex == (int)CardAbilityType.TriplePts)
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "3X"+_score;
                    _score*=3;
                }
                else
                {
                    cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "-OPP"+_score;
                    negScore = true;
                }
                //Updating score for owner
                if(HasStateAuthority){
                    score+=_score;
                }
                else
                {
                    if(negScore)minusScore = 1;
                }
            }
        }
    }
    void ReduceScore()
    {
        if(!HasStateAuthority && minusScore!=0)
        {
            score -= minusScore;
            minusScore = 0;
        }
    }
    IEnumerator QuitToMenu()
    {
        Debug.Log("Game end to be handled1!");
        yield return new WaitForSeconds(1.5f);
        var go = GameObject.FindObjectOfType<NetworkRunner>();
        Destroy(go.gameObject);
        SceneManager.LoadScene(0);
    }
    IEnumerator WaitForSomeTime()
    {
        Debug.Log("Game end to be handled1!");
        yield return new WaitForSeconds(3f);
        var gamestartorend = new GameStartEnd(false);
        var info = JsonConvert.SerializeObject(gamestartorend);
        Debug.Log("check startend string check now:"+GameStartOrEnd.ToString());
        if(info[info.Count()-1] != '}') 
        {
            Debug.Log("inside check"+info);
            info+='}';
        }
        GameStartOrEnd = info;
        StartCoroutine(QuitToMenu());
    }
    void Update()
    {
        if(Runner == null) return;
        if(!start && Runner.ActivePlayers.Count() == 2 && isMaster && HasStateAuthority)
        {
            start = true;
            var gamestartorend = new GameStartEnd(true);
            var info = JsonConvert.SerializeObject(gamestartorend);
            Debug.Log("check startend string check now:"+GameStartOrEnd.ToString());
            GameStartOrEnd = info;
            Debug.Log("check startend string now:"+gamestartorend.ToString());
        }
        if(isMaster && turnCount!=-1){
            timerText.text = ((int)tickTimer.RemainingTime(Runner)).ToString();
            turnText.text = turnCount.ToString();
        }
        if(isMaster && HasStateAuthority)
        {
            if(tickTimer.Expired(Runner))
            {
                if(turnCount>=6 && !GameOver.activeInHierarchy)
                {
                    StartCoroutine(WaitForSomeTime());
                }
                else
                {
                    turnCount++;
                    Debug.Log("current turn count is:"+turnCount);
                    EndTurn();
                    StartTimer();
                }
            }
        }
        if(HasStateAuthority)
        {
            my_scoreText.text = "MyScore:"+score;
        }
        else
        {
            opp_scoreText.text = "OppScore:"+score;
        }
    }


    //LOCAL CUTTON CLICK FUNCTIONS
    public void PlayCard()
    {
        Debug.Log("[PlayCard]");
        if(clickedCost.Count()!=0 && costAdded >= int.Parse(turnText.text))
        {
            //DISABLE PLAY BTN
            playCardBtn.SetActive(false);
        }
    }
    public void EndTurn()
    {
        if(costAdded >= int.Parse(turnText.text) && !playCardBtn.activeInHierarchy)
        {
            //REVEAL LOGIC
            if(clickedCost.Count!=0)
            {
                int[] ids = new int[clickedCost.Count];
                int idx = 0;
                foreach(var val in clickedCost)
                {
                    Debug.Log("check idx:"+idx);
                    ids[idx] = val.Value;
                    idx++;
                }
                foreach(var val in clickedCost.Keys)
                {
                    Destroy(val.gameObject);
                }
                var reveal = new RevealCard(ids);
                var info = JsonConvert.SerializeObject(reveal);
                Debug.Log("reveal info str:"+info[info.Count()-1]);
                if(info[info.Count()-1] != '}') {
                    info+=("}");
                    Debug.Log("Revealinfo str doublecheck:"+RevealInfo);
                }
                RevealInfo = info;
                Debug.Log("Revealinfo str check:"+RevealInfo);
                SpawnCardFromDeck();//spawn card only if player reveals card every round!
            }
        }
    }
    void ReSelectCards()
    {
        clickedCost.Clear();
        costAdded = 0;
    }
    void CardClicked(int cardid,int costid)
    {
        if(costAdded >= int.Parse(turnText.text) && !playCardBtn.activeInHierarchy) return;
        var clickedGO = EventSystem.current.currentSelectedGameObject;
        if(clickedCost.ContainsKey(clickedGO)) return;
        clickedCost.Add(clickedGO,cardid);
        costAdded += costid;
        Debug.Log("[CardClicked] clickedcost:"+clickedCost+" cardid:"+cardid);
        
    }
    public void StartTimer()
    {
        tickTimer = TickTimer.CreateFromSeconds(Runner,31f);
    }
    public void SpawnCardFromDeck()
    {
        int RandCardId = UnityEngine.Random.Range(3, Menu.DeckCards.Count);
        myCards.Add(Menu.DeckCards[RandCardId]);
        var cardObj = Instantiate(cardPrefab, content);
        cardObj.transform.GetChild(0).GetComponent<TMP_Text>().text = Menu.DeckCards[RandCardId].Name;
        cardObj.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = Menu.DeckCards[RandCardId].cost.ToString();
        int abilityIndex = Menu.DeckCards[RandCardId].ability.type;
        int _score = Menu.DeckCards[RandCardId].ability.value;
        int costID = Menu.DeckCards[RandCardId].cost;
        if(abilityIndex == (int)CardAbilityType.GainPts)
        {
            cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = _score.ToString();
        }
        else if(abilityIndex == (int)CardAbilityType.DoublePts)
        {
            cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text =(_score * 2). ToString();
        }
        else if(abilityIndex == (int)CardAbilityType.TriplePts)
        {
            cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "3X"+_score;
        }
        else
        {
            cardObj.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "-OPP"+_score;
        }
        cardObj.GetComponent<Button>().onClick.AddListener(() =>CardClicked(RandCardId,costID));
    }

    //ADDITIONAL CLASS REFERENCES
    public class GameStartEnd
    {
        public bool isStart;//if it is true it indicates game is start else game end
        public GameStartEnd(bool _start)
        {
            isStart = _start;
        }
    }
    public class RevealCard
    {
        public int[] cardID; //cardIds is enough to spawn cards
        public RevealCard(int[] arr)
        {
            cardID = arr;
        }
        
    }

}
