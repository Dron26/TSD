using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CVSLoader : MonoBehaviour
{
    private bool _debug = true;
    private const string url = "https://docs.google.com/spreadsheets/d/*/export?format=csv";
    private string _urlId="1kFemjH7YaDZsj_Tqp2wbEG9CN5IABScee9KhbTn5v_o";
    public void DownloadTable( Action<string> onSheetLoadedAction)
    {
        string actualUrl = url.Replace("*", _urlId);
        StartCoroutine(DownloadRawCvsTable(actualUrl, onSheetLoadedAction));
    }

    private IEnumerator DownloadRawCvsTable(string actualUrl, Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(actualUrl))
        {
            request.timeout = 30;
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
//                Debug.LogError(request.error);
            }
            else
            {
                if (_debug)
                {
//   Debug.Log("Successful download");
//                    Debug.Log(request.downloadHandler.text);
                }

                callback(request.downloadHandler.text);
            }
            
        }
        yield return null;
    }

    public void SetUrlId(string urlId)
    {
        _urlId=urlId;
    }
}