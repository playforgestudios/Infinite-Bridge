using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class FirstLaunchUI : MonoBehaviour
{
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TMP_InputField nameInputField;
    
    [SerializeField] private GameObject agePanel;
    [SerializeField] private TMP_InputField ageInputField;

    private void Awake()
    {
        var isFirstLaunch = PlayerPrefs.GetInt("isFirstLaunch", 1);
        if (isFirstLaunch == 1)
        {
            namePanel.SetActive(true);
            agePanel.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnNameSubmit()
    {
        if (nameInputField.text.Length <= 0) 
            return;
        GameManager.PlayerName = nameInputField.text;
        namePanel.SetActive(false);
        agePanel.SetActive(true);
    }

    public void OnAgeSubmit()
    {
        if(ageInputField.text.Length <= 0)
            return;
        GameManager.PlayerAge = int.Parse(ageInputField.text);
        PlayerPrefs.SetInt("isFirstLaunch", 0);
        
        this.gameObject.SetActive(false);
        
        GameManager.Instance.PublishEvent("SetLeaderboard", 0);
        GameManager.Instance.PublishEvent("Initialize Ads");
    }
    
}
