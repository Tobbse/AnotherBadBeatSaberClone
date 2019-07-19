using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PAnalyzerConfig
{
    private float _sampleRate = (float)AudioSettings.outputSampleRate;
    private float _maxFreq;
    private float _hzPerBin;
    private FastList<PBeatConfig> _beatConfigs;
    private int _bands;
    private int _clipSampleRate;

    public PAnalyzerConfig(int clipSampleRate)
    {
        _maxFreq = _sampleRate / 2;
        _hzPerBin = _maxFreq / PSpectrumProvider.NUM_BINS;
        _beatConfigs = _getConfigs();
        _clipSampleRate = clipSampleRate;
    }

    /*private void _setupBeatConfigs()
    {
        float sampleRate = (float)AudioSettings.outputSampleRate;
        double maxSamplerateToConsider = (double)sampleRate / SAMPLE_RATE_DIVIDER;


        //double start, double stop, int num, bool endpoint = true, double numericBase = 10.0d // Log
        //double start, double stop, int num, bool endpoint = true // Lin
        //IEnumerable<double> enumerator = PLogarithmUtils.LogSpace(0.0d, maxSamplerateToConsider, _bands);
        IEnumerable<double> enumerator = PLogarithmUtils.LogSpace(0.0d, 4, _bands);
        List<double> logValues = enumerator.ToList();

        for (var i = 0; i < logValues.Count; i++) {
            double lowerBound = logValues[i];
            double upperBound = logValues[i + 1];
            int lowerIndex = _getLowerIndexFromFrequency(lowerBound);
            int upperIndex = _getUpperIndexFromFrequency(upperBound);
            if (upperIndex > NUM_FREQ_BINS) upperIndex = NUM_FREQ_BINS;

            PBeatConfig beatCfg = new PBeatConfig();
            beatCfg.band = i;
            beatCfg.startIndex = lowerIndex;
            beatCfg.endIndex = upperIndex;
            beatCfg.startFrequency = Mathf.RoundToInt((float)lowerBound);
            beatCfg.endFrequency = Mathf.RoundToInt((float)upperBound);

            _beatConfigs.Add(beatCfg);
        }
    }

    private int _getLowerIndexFromFrequency(double freq)
    {
        return Mathf.CeilToInt((float)freq / _frequencyPerBin);
    }

    private int _getUpperIndexFromFrequency(double freq)
    {
        return Mathf.FloorToInt((float)freq / _frequencyPerBin);
    }*/

    public FastList<PBeatConfig> BeatConfigs
    {
        get { return _beatConfigs; }
    }

    public int Bands
    {
        get { return _bands; }
    }

    public int ClipSampleRate
    {
        get { return _clipSampleRate; }
    }

    private FastList<PBeatConfig> _getConfigs()
    {
        FastList<PBeatConfig> configs = new FastList<PBeatConfig>();

        // Config for samplesize = 2048, also 1024 bins
        /*configs.Add(_makeConfig(0, 0, 3));
        configs.Add(_makeConfig(1, 3, 6));
        configs.Add(_makeConfig(2, 6, 11));
        configs.Add(_makeConfig(3, 11, 18));
        configs.Add(_makeConfig(4, 18, 30));
        configs.Add(_makeConfig(5, 30, 45));
        configs.Add(_makeConfig(6, 45, 75));
        configs.Add(_makeConfig(7, 75, 110));
        configs.Add(_makeConfig(8, 110, 150));
        configs.Add(_makeConfig(9, 150, 230));
        configs.Add(_makeConfig(10, 230, 350));
        configs.Add(_makeConfig(11, 350, 500));
        configs.Add(_makeConfig(12, 500, 800));*/

        // Config for samplesize = 512, also 256 bins
        /*configs.Add(_makeConfig(0, 0, 1));
        configs.Add(_makeConfig(1, 1, 2));
        configs.Add(_makeConfig(2, 2, 4));
        configs.Add(_makeConfig(3, 4, 7));
        configs.Add(_makeConfig(4, 7, 15));
        configs.Add(_makeConfig(5, 15, 25));
        configs.Add(_makeConfig(6, 25, 46));
        configs.Add(_makeConfig(7, 46, 120));*/

        // Config for samplesize = 512, also 256 bins
        /*configs.Add(_makeConfig(0, 0, 2));
        configs.Add(_makeConfig(1, 2, 7));
        configs.Add(_makeConfig(2, 7, 25));
        configs.Add(_makeConfig(3, 25, 120));*/

        // Config for samplesize = 256, also 128 bins
        //configs.Add(_makeConfig(0, 0, 1, 200, 50, 5.0f));
        //configs.Add(_makeConfig(1, 3, 7, 200, 50, 3.0f));
        //configs.Add(_makeConfig(2, 8, 25, 300, 100, 3.0f));

        configs.Add(_makeConfig(0, 0, 1, 150, 50, 6.0f));
        configs.Add(_makeConfig(1, 4, 8, 250, 80, 6.0f));
        configs.Add(_makeConfig(2, 9, 25, 300, 100, 3.0f));

        _bands = configs.Count;

        return configs;
    }

    private PBeatConfig _makeConfig(int band, int startIndex, int endIndex, int thresholdSize, int beatTime, float tresholdMult)
    {
        return new PBeatConfig(band, startIndex, endIndex, _hzPerBin * startIndex, _hzPerBin * endIndex, thresholdSize, beatTime, tresholdMult);
    }

}
