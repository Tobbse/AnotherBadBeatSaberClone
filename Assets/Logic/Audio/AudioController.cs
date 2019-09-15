using UnityEngine;
using SpectrumConfigs;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using AudioAnalyzerConfigs;
using BeatMappingConfigs;
using System.Threading;

/**
 * Loads an audio file and then triggers the AudioAnalyzerHandler.
 * First a .mp3 file or a .ogg file  (may also be named .egg for whatever reason, but it's exactly the same)
 * is loaded. This can unforunately not happen in a thread, because functions from the Unity package are needed
 * for this, which are not threadsafe. May lead to small ~ 1 second spikes.
 * 
 * After that, a Thread is created which handles the audio analysis, so that the main thread is not blocked and
 * the player can still move and look around while the processing is done.
 * 
 * If a beat mapping for the chosen audio file and the chosen difficulty level already exists, that mapping is
 * loaded from cache and the audio analysis is skipped. This is much faster obviously.
 * Mappings are saved in the same json format as mappings from bsaber.com. This allows any custom mapping from
 * external sources to also be loaded, unless they have bpm changes which is not supported and will mess up some
 * timing.
 * After the loading is done, the Game scene is loaded.
 **/
public class AudioController : MonoBehaviour
{
    public AudioSource testAudioSource;

    private AudioClip _audioClip;
    private string _difficulty;
    private SpectrumProvider _spectrumProvider;
    private bool _isComplete;
    private TrackConfig _trackConfig;
    private JsonController _jsonController;
    private float[] _stereoSamples;
    private AudioAnalyzerHandler _audioAnalyzerHandler;
    private int _channels;

    private void Awake()
    {
        // MainMenu can be inactive when the FastGameStart Scene was used to start the game.
        GameObject playerMenu = GameObject.Find("PlayerMenu");
        if (playerMenu != null) playerMenu.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(LoadAudioClip(GlobalStorage.getInstance().AudioPath));
    }

    // _isComplete is true once the song has been loaded and the mappings have been created,
    // either from analyzing the spectrum data or loading a mapping from cache.
    private void Update()
    {
        if (_isComplete == true)
        {
            SceneManager.LoadSceneAsync("MainGame");
            _isComplete = false;
            enabled = false;
        }
    }

    // Gets mono samples and spectrum data from audio data. Then creates a list of configs and the
    // mapping container that will be used in the analysis.
    public void ProcessAudioData()
    {
        float[] monoSamples = AudioSampleProvider.getMonoSamples(_stereoSamples, _channels);
        List<double[]> spectrumsList = _spectrumProvider.getSpectrums(monoSamples);

        List<AnalyzedSpectrumConfig> spectrumDataList = _spectrumProvider.getSpectrumConfigs(spectrumsList, _trackConfig.Bands);
        _audioAnalyzerHandler = new AudioAnalyzerHandler(_trackConfig, spectrumDataList, new MappingContainer());
        _audioAnalyzerHandler.analyzeSpectrumsList(analysisFinished);
    }

    // Fills mapping container with data loaded from a cached mapping.
    private void loadMappingFromCache()
    {
        string mappingPath = _jsonController.getFullMappingPath(
            JsonController.MAPPING_FOLDER_PATH, _trackConfig.TrackName, GlobalStorage.getInstance().Difficulty);
        string infoPath = _jsonController.getFullInfoPath(JsonController.MAPPING_FOLDER_PATH, _trackConfig.TrackName);

        MappingContainer mappingContainer = _jsonController.readMappingFile(mappingPath);
        mappingContainer.MappingInfo = _jsonController.readInfoFile(infoPath);
        GlobalStorage.getInstance().MappingContainer = mappingContainer;

        done();
    }

    // Fills mapping container with data created in the audio analysis.
    private void analysisFinished()
    {
        MappingContainer mappingContainer = _audioAnalyzerHandler.getBeatMappingContainer();
        mappingContainer.MappingInfo.Bpm = 1;
        mappingContainer.sortMappings(); // Because multiple band spectrums are analyzed sequentially, we have to sort the mappings by time.
        GlobalStorage.getInstance().MappingContainer = mappingContainer;

        _jsonController.writeMappingFile(mappingContainer, _trackConfig.TrackName, _difficulty);
        _jsonController.writeInfoFile(mappingContainer, _trackConfig.TrackName, mappingContainer.MappingInfo.Bpm);

        done();
    }

    private void done()
    {
        GlobalStorage.getInstance().TrackConfig = _trackConfig;
        _isComplete = true;
    }

    // Loads .mp3, .ogg or .egg audio file using either WWW or the MP3AudioLoader.
    private IEnumerator LoadAudioClip(string path)
    {
        if (path.Contains(".mp3"))
        {
            _audioClip = Mp3Loader.LoadMp3(path);
        }
        else if (path.Contains(".ogg") || path.Contains(".egg"))
        {
            _audioClip = new WWW("file://" + path).GetAudioClip(true, false, AudioType.OGGVORBIS);
        }
        else
        {
            Debug.LogError("Audio File not supported. Files has to be one of those types: .egg, .ogg, .mp3");
            yield break;
        }
        while (!_audioClip.isReadyToPlay)
        {
            yield return _audioClip;
        }
        _audioClip.LoadAudioData();
        _clipLoaded();
    }

    // Prepares track config after loading the audio clip. Extracts the track name from the file path.
    // Loads mapping from cache if it exists or creates Thread for audio analysis.
    private void _clipLoaded()
    {
        GlobalStorage.getInstance().AudioClip = _audioClip;
        _channels = _audioClip.channels;

        _difficulty = GlobalStorage.getInstance().Difficulty;
        _jsonController = new JsonController();

        // When loading .ogg files using WWW, the audio clip has no name, so we have to extract the name from the path. 
        string clipName = _audioClip.name;
        if (_audioClip.name == "") clipName = _getClipName();
        _trackConfig = new TrackConfig(_audioClip.frequency, clipName);

        string fullPath = _jsonController.getFullMappingPath(JsonController.MAPPING_FOLDER_PATH, _trackConfig.TrackName, _difficulty);
        if (DevSettings.USE_CACHE && _jsonController.fileExists(fullPath))
        {
            Debug.Log("Loading track mapping from cache.");
            loadMappingFromCache();
            return;
        }

        _spectrumProvider = new SpectrumProvider(_trackConfig.ClipSampleRate);
        _stereoSamples = AudioSampleProvider.getSamples(_audioClip);
        
        Thread thread = new Thread(ProcessAudioData);
        thread.Start();
    }

    // Extracts the name of a clip from the file path by pruning it until just the file name is left.
    private string _getClipName()
    {
        string clipName = GlobalStorage.getInstance().AudioPath;
        while (clipName.Contains("/"))
        {
            clipName = clipName.Substring(clipName.LastIndexOf("/") + 1);
        }
        while (clipName.Contains("\\"))
        {
            clipName = clipName.Substring(clipName.LastIndexOf("\\") + 1);
        }
        while (clipName.Length > 2)
        {
            if (clipName[clipName.Length - 1] != '.')
            {
                clipName = clipName.Remove(clipName.Length - 1);
            }
            else
            {
                clipName = clipName.Remove(clipName.Length - 1);
                break;
            }
        }
        return clipName;
    }
}
