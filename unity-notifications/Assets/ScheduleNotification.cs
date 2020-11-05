using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Notifications.iOS;
using UnityEngine;
using UnityEngine.Networking;

public class ScheduleNotification : MonoBehaviour
{

    void Start()
    {
        attachmentDir = Application.persistentDataPath + "/Attachments";
        if (!Directory.Exists (attachmentDir)) {
            Directory.CreateDirectory (attachmentDir);
        }
        
        StartCoroutine(RequestAuthorization());
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
    }

    private IEnumerator RequestAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
        }
    }

    private string attachmentDir;

    private IEnumerator DownloadImage(string imageUrl, string filePath)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(imageUrl))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Failed to download image: " + www.error);
            }
            else
            {
                File.WriteAllBytes(filePath, www.downloadHandler.data);
            }
        }
    }
    
    [DllImport("__Internal")]
    private static extern void _Plugin_ScheduleLocalNotification(string instanceId, string title, string body, string attachmentFile, int timeIntervalSecs);
    
    public IEnumerator Schedule()
    {
        var imageFile = Path.Combine(attachmentDir,"notification.png");
        if (!File.Exists(imageFile))
        {
            yield return DownloadImage("https://www.dapperhacks.com/battle-stone.jpg", imageFile);
        }
        
        _Plugin_ScheduleLocalNotification(instanceId: DateTime.Now.ToString("yyyyMMddhhmmss"),title: "Battle Stones Received",body: "You received a 🎁 gift 🎁 of battlestones. Play now to claim!", attachmentFile: imageFile, timeIntervalSecs: 5);
    }
    
    public void OnClick()
    {
        StartCoroutine(Schedule());
    }

}
