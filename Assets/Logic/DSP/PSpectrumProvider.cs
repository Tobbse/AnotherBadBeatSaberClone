using System;
using System.Numerics;
using DSPLib;

public class PSpectrumProvider
{
    public const int SAMPLE_SIZE = 2048;
    public const int NUM_BINS = (SAMPLE_SIZE / 2) + 1;

    private const DSP.Window.Type WINDOW_TYPE = DSP.Window.Type.Hamming;

    public static FastList<double[]> getSpectrums(float[] monoSamples)
    {
        int iterations = monoSamples.Length / SAMPLE_SIZE;
        FastList<double[]> spectrums = new FastList<double[]>();

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
            double[] coefficients = DSP.Window.Coefficients(WINDOW_TYPE, (uint)SAMPLE_SIZE);
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

}
