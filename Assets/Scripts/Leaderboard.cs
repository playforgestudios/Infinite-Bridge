using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform rowsParent;

    private void OnEnable()
    {
        print("enabling leaderboard");
        ShowLeaderBoardUI();
    }

    private void ShowLeaderBoardUI()
    {
        
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_infinite_bridge,
            LeaderboardStart.TopScores,
            100,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
        (data) => LoadUsersAndDisplay(data));
    }
    
    private void LoadUsersAndDisplay(LeaderboardScoreData data)
    {
        // Get the user ids
        var userIds = new List<string>();

        foreach (IScore score in data.Scores)
        {
            userIds.Add(score.userID);
        }
        // Load the profiles and display
        Social.LoadUsers(userIds.ToArray(), (users) =>
        {
            string status = "Leaderboard loading: " + data.Title + " count = " +
                data.Scores.Length;
            foreach (IScore score in data.Scores)
            {
                IUserProfile user = FindUser(users, score.userID);

                GameObject row = Instantiate(rowPrefab, rowsParent);
                row.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = score.rank.ToString();
                row.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = user.userName;
                row.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = score.value.ToString();

                 status += "\n" + score.formattedValue + " by " +
                     (string)(
                         (user != null) ? user.userName : "**unk_" + score.userID + "**");
            }
            print(status);
        });
    }

    private IUserProfile FindUser(IUserProfile[] users, string userid)
    {
        foreach (IUserProfile user in users)
        {
            if (user.id == userid)
            {
                return user;
            }
        }
        return null;
    }

    internal static void PostToLeaderBoard(int score)
    {
        Social.ReportScore(score, GPGSIds.leaderboard_infinite_bridge, (success) =>
        {
            print(success ? "Posted to leaderboard": "Failed to post to leaderboard");
        });
    }

    private void OnDisable()
    {
        foreach(Transform child in rowsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
