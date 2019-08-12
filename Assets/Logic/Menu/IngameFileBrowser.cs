using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class IngameFileBrowser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FileBrowser.SetDefaultFilter(".mp3");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowLoadDialog(new FileBrowser.OnSuccess(_onAudioFilePicked), new FileBrowser.OnCancel(_onAudioFileCancelled));
    }

    private void _onAudioFilePicked(string target)
    {
        Debug.Log("OMG IT WORKED");
    }

    private void _onAudioFileCancelled()
    {
        Debug.Log("Cancel File Dialog!");
    }
}
