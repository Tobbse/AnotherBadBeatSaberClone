namespace PSpectrumData {
    public class SpectrumInfo
    {
        public float[] spectrum;
        public float time;
        public bool hasPeak;
        public FastList<SpectrumBandData> bandData = new FastList<SpectrumBandData>();
    }

    public class SpectrumBandData
    {
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public float band;
        public float peakBPM;
        public bool isPeak;
    }

}