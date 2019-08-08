using UnityEngine;

public static class PAudioSampleProvider
{
    public static float[] getMonoSamples(AudioClip audioClip)
    {
        return _getMonoSamples(_getSamples(audioClip), audioClip.channels);
    }

    public static float[] _getSamples(AudioClip audioClip)
    {
        int numTotalSamples = audioClip.samples * audioClip.channels;
        float[] stereoSamples = new float[numTotalSamples];
        audioClip.GetData(stereoSamples, 0);
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
