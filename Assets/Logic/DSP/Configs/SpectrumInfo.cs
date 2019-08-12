namespace PSpectrumInfo {
    public class AnalyzedSpectrumData
    {
        public float[] spectrum;
        public float time;
        public bool hasPeak;
        public FastList<int> peakBands = new FastList<int>();
        public FastList<BeatInfo> beatData = new FastList<BeatInfo>();
    }

    public class BeatInfo
    {
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public float band;
        public float peakBPM;
        public bool isPeak;
    }

}