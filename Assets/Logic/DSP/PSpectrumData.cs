namespace PSpectrumData {
    public class PSpectrumInfo
    {
        public float[] spectrum;
        public float time;
        public bool hasPeak;
        public FastList<int> peakBands = new FastList<int>();
        public FastList<PSpectrumBandData> bandData = new FastList<PSpectrumBandData>();
    }

    public class PSpectrumBandData
    {
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public float band;
        public float peakBPM;
        public bool isPeak;
    }

}