using System.Collections.Generic;

/**
 * Configs in this namespace contain information about a spectrum that was analyzed.
 **/
namespace AudioSpectrumInfo
{
    /**
     * Config that contains band configs with analyzed spectrum data. Summarizes those band configs
     * and for example provides easier access to the peaks that have been found in the different bands.
     **/
    public class AnalyzedSpectrumConfig
    {
        private float[] _spectrum;
        private float _time;
        private bool _hasPeak;
        private List<int> _peakBands = new List<int>();
        private List<BeatInfo> _bandBeatData = new List<BeatInfo>();

        public float[] Spectrum { get { return _spectrum; } set { _spectrum = value; } }
        public float Time { get { return _time; } set { _time = value; } }
        public bool HasPeak { get { return _hasPeak; } set { _hasPeak = value; } }
        public List<int> PeakBands { get { return _peakBands; } set { _peakBands = value; } }
        public List<BeatInfo> BandBeatData { get { return _bandBeatData; } set { _bandBeatData = value; } }
    }

    /**
     * Config that contains data obtained from a specific spectrum that is analyzed in a specific band.
     **/
    public class BeatInfo
    {
        public float _spectralFlux;
        public float _threshold;
        public float _prunedSpectralFlux;
        public float _band;
        public float _peakBPM;
        public bool _isPeak;

        public float SpectralFlux { get { return _spectralFlux; } set { _spectralFlux = value; } }
        public float Threshold { get { return _threshold; } set { _threshold = value; } }
        public float PrunedSpectralFlux { get { return _prunedSpectralFlux; } set { _prunedSpectralFlux = value; } }
        public float Band { get { return _band; } set { _band = value; } }
        public float PeakBPM { get { return _peakBPM; } set { _peakBPM = value; } }
        public bool IsPeak { get { return _isPeak; } set { _isPeak = value; } }
    }

}