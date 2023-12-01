using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

// using https://danqzq.itch.io/leaderboard-creator to create leaderboard
public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject playerRowPrefab;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform rowsParent;
    [SerializeField] private GameObject loadingPanel;

    [SerializeField] private int maxRows = 30;
    private const string publicLeaderboardKey = "33162b8a3e23694f973b7ae9a098f5aaa16ee619df0b72f96e3f74022e789955";

    private void Awake()
    {
        GameManager.OnEvent += GameManager_OnEvent;
    }

    private void Start()
    {
        loadingPanel.SetActive(false);
        this.gameObject.SetActive(false);

        var isFirstLaunch = PlayerPrefs.GetInt("isFirstLaunch", 1);
        if (isFirstLaunch == 1)
            return;

        SetLeaderboardEntry(GameManager.Instance.player.bestScore); // not run when first launch
    }

    void GetLeaderboard()
    {
        loadingPanel.SetActive(true);
        ClearLeaderboardUI();

        LeaderboardCreator.GetPersonalEntry(publicLeaderboardKey, ((msg) =>
        {
            GameObject playerRow = Instantiate(playerRowPrefab, rowsParent);

            playerRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = msg.Rank.ToString();
            playerRow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = msg.Username;
            playerRow.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = msg.Score.ToString();
            playerRow.transform.SetAsFirstSibling();
        }));
        
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < maxRows) ? msg.Length : maxRows;
            for (int i = 0; i < loopLength; i++)
            {
                if(msg[i].IsMine())
                    continue;
                GameObject row = Instantiate(rowPrefab, rowsParent);
                row.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = msg[i].Rank.ToString();
                row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = msg[i].Username;
                row.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = msg[i].Score.ToString();
            }

            loadingPanel.SetActive(false);
        }));
    }

    private void SetLeaderboardEntry(int score)
    {
        LeaderboardCreator.Ping((isConnected) =>
        {
            if (!isConnected)
                return;

            string username = GameManager.PlayerName;
            LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) =>
            {
                //GetLeaderboard();
                GetLeaderboard();
            }));
        });
    }

    public void ClearLeaderboardUI()
    {
        if (rowsParent.childCount == 0)
            return;
        foreach (var child in rowsParent.GetComponentsInChildren<Transform>())
        {
            if (child != rowsParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void GameManager_OnEvent(string name, object value)
    {
        if (name is "SetLeaderboard")
            SetLeaderboardEntry((int)value);
    }

    void OnDestroy()
    {
        GameManager.OnEvent -= GameManager_OnEvent;
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
            SoundManager.Instance.Click();
        }
#endif
    }
}