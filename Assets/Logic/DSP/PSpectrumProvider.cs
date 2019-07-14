using System;
using System.Numerics;
using DSPLib;

public class PSpectrumProvider
{
    private const int SPECTRUM_SAMPLE_SIZE = AudioAnalyzerPlain.PSpectrumAnalyzer.SPECTRUM_SAMPLE_SIZE;
    private const DSP.Window.Type WINDOW_TYPE = DSP.Window.Type.Hamming;

    public static FastList<double[]> getSpectrums(float[] monoSamples)
    {
        int iterations = monoSamples.Length / SPECTRUM_SAMPLE_SIZE;
        FastList<double[]> spectrums = new FastList<double[]>();

        FFT fft = new FFT();
        fft.Initialize((UInt32)SPECTRUM_SAMPLE_SIZE);

        double[] sampleChunk = new double[SPECTRUM_SAMPLE_SIZE];

        for (int i = 0; i < iterations; i++)
        {
            int startIndex = i * SPECTRUM_SAMPLE_SIZE;
            int len = SPECTRUM_SAMPLE_SIZE;
            int maxIndex = startIndex + len;
            if (maxIndex >= monoSamples.Length)
            {
                len = monoSamples.Length - startIndex;
            }
            if (len <= 0)
            {
                break;
            }

            Array.Copy(monoSamples, i * SPECTRUM_SAMPLE_SIZE, sampleChunk, 0, len);
            double[] coefficients = DSP.Window.Coefficients(WINDOW_TYPE, (uint)SPECTRUM_SAMPLE_SIZE);
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
