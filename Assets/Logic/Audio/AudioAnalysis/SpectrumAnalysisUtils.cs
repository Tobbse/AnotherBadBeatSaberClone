using UnityEngine;

namespace AudioAnalysis
{
    /// <summary>
    // Provides some util functionality for the spectrum analysis.
    // Currently none of this is actually used.
    /// </summary>
    public static class SpectrumAnalysisUtils
    {
        public static bool shouldBeExtraPeak(float totalAverageFlux, float currentAverageFlux)
        {
            return currentAverageFlux > totalAverageFlux * 1.5;
        }

        public static bool sampleIsCloser(float newVal, float oldVal, float trueVal)
        {
            return Mathf.Abs(trueVal - newVal) < Mathf.Abs(trueVal - oldVal);
        }
    }

}
