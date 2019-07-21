using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using NAudio.Wave;

public class AudioLoaderButton : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip _audioClip;
    private string _path;
    private int _systemSampleRate = AudioSettings.outputSampleRate;
    private float[] _audioData;
    private int _channels;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    public void openDialog()
    {
        _path = EditorUtility.OpenFilePanelWithFilters("Choose Audio File", "", ["mp3", "wav"]);
        if (_path.Length == 0)
        {
            openDialog();
            return;
        }
        _loadAudioData();
        AudioClip clip = AudioClip.Create(_path, _audioData.Length, _channels, _systemSampleRate, false);
        print(clip.frequency);
        print(clip.length);

        _audioSource = new AudioSource();
        _audioSource.playOnAwake = false;
        _audioSource.clip = _audioClip;
        _audioSource.Play();
    }

    private void _loadAudioData()
    {
        //StartCoroutine(LoadAudio());

        Mp3FileReader reader = new Mp3FileReader(_path);
        _audioData = new float[reader.Length];
        _channels = reader.WaveFormat.Channels;

        WaveFormat format = new WaveFormat(_systemSampleRate, _channels);
        MediaFoundationResampler resampler = new MediaFoundationResampler(reader, format);
        resampler.ToSampleProvider().Read(_audioData, 0, (int)reader.Length);
    }

    /*private IEnumerator LoadAudio()
    {
        WWW audioLoader = new WWW(_path);
        yield return audioLoader;
        _audioClip = audioLoader.GetAudioClip();
    }*/
}
