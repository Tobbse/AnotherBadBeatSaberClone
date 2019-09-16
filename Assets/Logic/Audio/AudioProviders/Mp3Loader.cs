using System;
using NLayer;
using UnityEngine;

namespace AudioProviders
{
    /// <summary>
    /// This class is used to load .mp3 files by using the NLayer library.
    /// </summary>
    public static class Mp3Loader
    {
        private static MpegFile mpegFile = null;
        private static string _filePath = String.Empty;

        public static AudioClip LoadMp3(string filePath)
        {
            _filePath = filePath;
            mpegFile = new MpegFile(filePath);

            // Assign mp3 file info into AudioClip
            AudioClip audioClip = AudioClip.Create(System.IO.Path.GetFileNameWithoutExtension(filePath),
                                            (int)(mpegFile.Length / sizeof(float) / mpegFile.Channels),
                                            mpegFile.Channels,
                                            mpegFile.SampleRate,
                                            false,
                                            OnMp3Read,
                                            OnClipPositionSet);
            return audioClip;
        }

        // PCMReaderCallback will be called each time AudioClip reads data.
        private static void OnMp3Read(float[] data)
        {
            int actualReadCount = mpegFile.ReadSamples(data, 0, data.Length);
        }

        // PCMSetPositionCallback will be called when first loading this AudioClip.
        private static void OnClipPositionSet(int position)
        {
            mpegFile = new MpegFile(_filePath);
        }
    }

}