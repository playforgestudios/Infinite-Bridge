using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class SavedGamesGPGS : MonoBehaviour
{
    private bool isSaving;
    
    void OpenSavedGame(string filename, bool saving)
    {
        isSaving = saving;
        if (Social.localUser.authenticated)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }
        
    }

    public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
            if (isSaving)
            {
                //save
            }
            else
            {
                
            }
        }
        else
        {
            // handle error
        }
    }
    
    void SaveGame (ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now.ToString());
        // if (savedImage != null) {
        //     // This assumes that savedImage is an instance of Texture2D
        //     // and that you have already called a function equivalent to
        //     // getScreenshot() to set savedImage
        //     // NOTE: see sample definition of getScreenshot() method below
        //     byte[] pngData = savedImage.EncodeToPNG();
        //     builder = builder.WithUpdatedPngCoverImage(pngData);
        // }
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            // handle reading or writing of saved game.
        } else {
            // handle error
        }
    }
}
