using UnityEngine;

public static class PAudioSampleProvider
{
    public static float[] getMonoSamples(AudioSource audioSource)
    {
        return _getMonoSamples(_getSamples(audioSource), audioSource.clip.channels);
    }

    public static float[] _getSamples(AudioSource audioSource)
    {
        int numTotalSamples = audioSource.clip.samples * audioSource.clip.channels;
        float[] stereoSamples = new float[numTotalSamples];
        audioSource.clip.GetData(stereoSamples, 0);
        return stereoSamples;
    }

    private static float[] _getMonoSamples(float[] stereoSamples, int numChannels)
    {
        float[] monoSamples = new float[stereoSamples.Length / numChannels];
        int numProcessed = 0;

        for (int i = 0; i < stereoSamples.Length; i+= numChannels)
        {
            float channelAverage = 0.0f;
            for (int j = 0; j < numChannels; j++) {
                channelAverage += stereoSamples[i + j];
            }
            monoSamples[numProcessed] = channelAverage / numChannels;
            numProcessed++;
        }
        return monoSamples;
    }

}
