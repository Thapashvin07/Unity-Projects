using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToGame : MonoBehaviour
{
    public void ConnectToGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
