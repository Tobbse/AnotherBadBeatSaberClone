namespace PSpectrumInfo {
    public class PAnalyzedSpectrumData
    {
        public float[] spectrum;
        public float time;
        public bool hasPeak;
        public FastList<int> peakBands = new FastList<int>();
        public FastList<PBeatInfo> beatData = new FastList<PBeatInfo>();
    }

    public class PBeatInfo
    {
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public float band;
        public float peakBPM;
        public bool isPeak;
    }

}