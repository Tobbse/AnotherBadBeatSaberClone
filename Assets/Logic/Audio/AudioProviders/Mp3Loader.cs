using System;
using NLayer;
using UnityEngine;

public static class Mp3Loader
{
    private static MpegFile mpegFile = null;
    private static string _filePath = String.Empty;

    public static AudioClip LoadMp3(string filePath)
    {
        _filePath = filePath;

        mpegFile = new MpegFile(filePath);

        // assign mpegFile's info into AudioClip
        AudioClip ac = AudioClip.Create(System.IO.Path.GetFileNameWithoutExtension(filePath),
                                        (int)(mpegFile.Length / sizeof(float) / mpegFile.Channels),
                                        mpegFile.Channels,
                                        mpegFile.SampleRate,
                                        false,
                                        OnMp3Read,
                                        OnClipPositionSet);

        return ac;
    }

    // PCMReaderCallback will called each time AudioClip reads data.
    private static void OnMp3Read(float[] data)
    {
        int actualReadCount = mpegFile.ReadSamples(data, 0, data.Length);
    }

    // PCMSetPositionCallback will called when first loading this audioclip
    private static void OnClipPositionSet(int position)
    {
        mpegFile = new MpegFile(_filePath);
    }
}