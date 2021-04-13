using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIJoinGame : MonoBehaviour
{
    public TMPro.TMP_Text gameCodeText;

    private void Start()
    {
        if (gameCodeText == null)
        {
            Debug.LogError("The Game Code text field is not attached.");
        }
    }

    public void JoinGame()
    {
        var gameCode = gameCodeText.text;
        MySceneManager.SetSceneArgument("GameView", "IsHost", false);
        MySceneManager.SetSceneArgument("GameView", "GameCode", gameCode);
        SceneManager.LoadSceneAsync("GameView");
    }
}
