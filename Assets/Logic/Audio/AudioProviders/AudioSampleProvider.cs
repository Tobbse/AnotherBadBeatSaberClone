using UnityEngine;

namespace AudioProviders
{
    /// <summary>
    /// Provider for audio samples from an audio clip.Can create stereo samples and get mono samples from that array.
    /// We want mono samples as they are easier to handle.
    /// </summary>
    public static class AudioSampleProvider
    {
        public static float[] getSamples(AudioClip audioClip)
        {
            int numTotalSamples = audioClip.samples * audioClip.channels;
            float[] stereoSamples = new float[numTotalSamples];
            audioClip.GetData(stereoSamples, 0);
            return stereoSamples;
        }

        public static float[] getMonoSamples(float[] stereoSamples, int numChannels)
        {
            float[] monoSamples = new float[stereoSamples.Length / numChannels];
            int numProcessed = 0;

            for (int i = 0; i < stereoSamples.Length; i += numChannels)
            {
                float channelAverage = 0.0f;
                for (int j = 0; j < numChannels; j++)
                {
                    channelAverage += stereoSamples[i + j];
                }
                monoSamples[numProcessed] = channelAverage / numChannels;
                numProcessed++;
            }
            return monoSamples;
        }
    }

}
