using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[Serializable]
public struct User
{
    public string name;
    public string id;
    public bool mentionActive;
}


public static class Helpers
{
    public static string GetRawUsers(this User[] users)
    {
        string rawUsers = "";
        
        foreach (var user in users)
        {
            if(user.mentionActive)
                rawUsers += $"<@{user.id}> ";
        }

        return rawUsers;
    }
}

public class DiscordNotifier : MonoBehaviour
{
    [SerializeField] private string URL_WEBHOOK;
    [SerializeField] private string SECRET_CODE;
    [Space]
    [SerializeField,TextArea(5,10)] private string defaultMsg;
    [SerializeField] private User[] userMentions;
    [SerializeField] private string[] features;
    [SerializeField] private KeyCode OpennerKey;
    [SerializeField] private SendMessageUI ui;


    Coroutine senderCoroutine;
    private void Awake()
    {
        ui.SetActiveElement(false);
   
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(OpennerKey))
        {
            OpenUI();
        }
    }

    private void OpenUI()
    {
        ui.OnTrySendMsg += ValidateSecretCode;
        ui.SetActiveElement(true);
    }

    private void ValidateSecretCode(string secretCode)
    {
        if (SECRET_CODE != secretCode)
        {
            ui.ShowError();
        }
        
        SendDiscordNotification();
    }
    
    public void  SendDiscordNotification()
    {
        if (senderCoroutine != null)
        {
            StopCoroutine(senderCoroutine);
        }

        senderCoroutine =  StartCoroutine(SendMessage(defaultMsg,OnMsgSent));
        
        void OnMsgSent(bool success)
        {
            if (success)
            {
                ui.SetActiveElement(false);
                Destroy(this);
            }
           
        }
    }
    
    private IEnumerator SendMessage(string msg, Action<bool> OnComplete)
    {
        WWWForm formData = new WWWForm();
        
        formData.AddField("content",$"{userMentions.GetRawUsers()} {msg}\n {GetRawFeatures()}");
      
        
        UnityWebRequest postRequest = UnityWebRequest.Post(URL_WEBHOOK, formData);
        
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(postRequest.error);
            OnComplete?.Invoke(false);
            yield break;
        }

        OnComplete?.Invoke(true);
        Debug.Log("Msg sent!");
    }
    
    
    public string GetRawFeatures()
    {
        if (features.Length == 0) return "";
        
        string rawFeatures = ":white_check_mark: ***Features:***\n";
        foreach (var feature in features)
        {
            rawFeatures += $"- {feature}\n ";
        }
        
        return rawFeatures+ "\n";
    }

    
    

}
