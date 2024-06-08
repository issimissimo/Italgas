using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using System.IO;
using UnityEngine;
using System.Windows.Forms;

public class FileDownloader
{
    public enum STATE { SUCCESS, NOT_FOUND, NETWORK_ERROR }
    public class Result
    {
        public STATE state;
        public string downloadedText;
    }

    public IEnumerator LoadTextFromUrl(string url, Action<Result> callback)
    {
        Result result = new Result();
        UnityWebRequest www = new UnityWebRequest(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            if (www.responseCode == 404) result.state = STATE.NOT_FOUND;
            else result.state = STATE.NETWORK_ERROR;
        }
        else
        {
            result.state = STATE.SUCCESS;
            result.downloadedText = www.downloadHandler.text;
        }
        callback(result);
    }

    public IEnumerator LoadFileFromUrlToRawImage(string url, RawImage rawImage, Action<Result> callback = null)
    {
        Result result = new Result();
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                if (www.responseCode == 404) result.state = STATE.NOT_FOUND;
                else result.state = STATE.NETWORK_ERROR;
            }
            else
            {
                result.state = STATE.SUCCESS;
                var texture = DownloadHandlerTexture.GetContent(www);
                rawImage.texture = texture;
            }
        }
        if (callback != null) callback(result);
    }

    public IEnumerator DownloadAndSave(string localFilePath, string remoteFilePath, Action<Result> callback)
    {
        Result result = new Result();
        UnityWebRequest www = UnityWebRequest.Get(remoteFilePath);
        www.SendWebRequest();

        while (!www.isDone)
        {
            // downloadProgress = (int)(_www.downloadProgress * 100);
            yield return null;
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ERRORE " + www.error);
            if (www.responseCode == 404) result.state = STATE.NOT_FOUND;
            else result.state = STATE.NETWORK_ERROR;
        }
        else
        {
            result.state = STATE.SUCCESS;
            File.WriteAllBytes(Path.Combine(localFilePath), www.downloadHandler.data);
            Debug.Log("Download saved to: " + localFilePath.Replace("/", "\\") + "\r\n" + www.error);
        }

        www.Dispose();

        if (callback != null) callback(result);
    }

}
