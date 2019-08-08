using UnityEngine;
using UnityEditor;

public class RandomStuff : ScriptableObject
{

    /**
     * From the loading screen, trying to resample audio.
     * */
    /*private AudioClip _loadResampledAudioData(string path)
    {
        AudioFileReader reader = new AudioFileReader(path);
        int channels = reader.WaveFormat.Channels;
        int systemSampleRate = AudioSettings.outputSampleRate;
        int numSamples = (int)(reader.Length / channels / (reader.WaveFormat.BitsPerSample / 8));
        float[] audioData = new float[numSamples];

        WdlResamplingSampleProvider resampler = new WdlResamplingSampleProvider(reader, systemSampleRate);
        resampler.Read(audioData, 0, numSamples);

        AudioClip audioClip = AudioClip.Create(path, numSamples, channels, systemSampleRate, false);
        audioClip.SetData(audioData, 0);
        return audioClip;
    }*/


    /**
     * From the beat detection class: BPM Detection stuff.
     * */
    private float[] _bpms;
    private float _spectrumsPerBPMInterval;
    /*private float _getAveragedBpm(int currIndex) {
        int indexDist = Mathf.CeilToInt(_spectrumsPerBPMInterval);
        int minIndex = currIndex - indexDist;
        int maxIndex = currIndex - 1;

        if (minIndex <= 0) minIndex = 0;
        if (maxIndex <= 0) maxIndex = 0;
        if (maxIndex >= _spectrumData.Count) maxIndex = _spectrumData.Count - 1;

        int newIndexDist = maxIndex - minIndex;

        float sum = 0.0f;
        for (int i = minIndex; i < maxIndex; i++)
        {
            sum += _bpms[i];
        }
        return sum / newIndexDist;
    }

    private float _getBpm(int beatIndex)
    {
        int beats = 0;
        int indexDist = Mathf.CeilToInt(_spectrumsPerBPMInterval);
        int minIndex = beatIndex - indexDist;
        int maxIndex = beatIndex -1;
        int currentIndex = beatIndex - 1;

        if (minIndex <= 0) minIndex = 0;
        if (maxIndex <= 0) maxIndex = 0;
        if (maxIndex >= _spectrumData.Count) maxIndex = _spectrumData.Count - 1;

        int newIndexDist = maxIndex - minIndex;
        float newBpmInterval = newIndexDist * _timePerSpectrum;

        for (int i = minIndex; i < maxIndex; i++)
        {
            if (_spectrumData[i].hasPeak)
            {
                beats += 1;
            }
        }
        return beats * (60 / newBpmInterval);
    }*/

}