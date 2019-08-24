using System;
using System.Numerics;
using UnityEngine;
using DSPLib;
using AudioSpectrumInfo;
using System.Collections.Generic;

public class SpectrumProvider
{
    public const int SAMPLE_SIZE = 1024;
    public const int NUM_BINS = (SAMPLE_SIZE / 2) + 1;

    private float _timePerSample;
    private float _timePerSpectrumData;

    public SpectrumProvider(int audioClipSampleRate)
    {
        _timePerSample = 1f / audioClipSampleRate;
        _timePerSpectrumData = (1.0f / audioClipSampleRate) * SAMPLE_SIZE; // Duration per sample * amount of samples per spectrum.
    }

    public List<double[]> getSpectrums(float[] monoSamples)
    {
        int iterations = monoSamples.Length / SAMPLE_SIZE;
        List<double[]> spectrums = new List<double[]>();

        FFT fft = new FFT();
        fft.Initialize((UInt32)SAMPLE_SIZE);

        double[] sampleChunk = new double[SAMPLE_SIZE];

        for (int i = 0; i < iterations; i++)
        {
            int startIndex = i * SAMPLE_SIZE;
            int len = SAMPLE_SIZE;
            int maxIndex = startIndex + len;
            if (maxIndex >= monoSamples.Length)
            {
                len = monoSamples.Length - startIndex;
            }
            if (len <= 0)
            {
                break;
            }

            Array.Copy(monoSamples, i * SAMPLE_SIZE, sampleChunk, 0, len);
            double[] coefficients = DSP.Window.Coefficients(DSP.Window.Type.Hamming, (uint)SAMPLE_SIZE);
            double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, coefficients);
            double scaleFactor = DSP.Window.ScaleFactor.Signal(coefficients);

            // Perform the FFT and convert output (complex numbers) to Magnitude
            Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
            double[] scaledFFTSpectrum = DSP.ConvertComplex.ToMagnitude(fftSpectrum);
            scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

            spectrums.Add(scaledFFTSpectrum);
        }
        return spectrums;
    }

    public List<AnalyzedSpectrumConfig> getSpectrumData(List<double[]> spectrums, int bands)
    {
        List<AnalyzedSpectrumConfig> spectrumDataList = new List<AnalyzedSpectrumConfig>();

        for (int i = 0; i < spectrums.Count; i++)
        {
            AnalyzedSpectrumConfig data = new AnalyzedSpectrumConfig();
            data.time = _getAudioClipTimeFromIndex(i);
            data.hasPeak = false;
            data.spectrum = System.Array.ConvertAll(spectrums[i], doubleVal => (float)doubleVal);

            for (int j = 0; j < bands; j++)
            {
                BeatInfo bandData = new BeatInfo();
                data.bandBeatData.Add(bandData);
            }
            spectrumDataList.Add(data);
        }
        return spectrumDataList;
    }

    private float _getAudioClipTimeFromIndex(int spectrumDataIndex)
    {
        //Debug.Log(_timePerSpectrumData * spectrumDataIndex);
        return _timePerSpectrumData * spectrumDataIndex;
    }

}
