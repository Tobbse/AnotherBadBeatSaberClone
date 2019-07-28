using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PAnalyzerConfig
{
    private FastList<PBeatConfig> _beatConfigs = new FastList<PBeatConfig>();
    private int _bands;
    private int _clipSampleRate;

    private float _sampleRate = (float)AudioSettings.outputSampleRate;
    private float _maxFreq;
    private float _hzPerBin;

    public PAnalyzerConfig(int clipSampleRate)
    {
        _maxFreq = _sampleRate / 2;
        _hzPerBin = _maxFreq / PSpectrumProvider.NUM_BINS;
        _clipSampleRate = clipSampleRate;

        _createBeatConfigs();
        _bands = _beatConfigs.Count;
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

    // TODO maybe also pass a parameter for pre- and after pruned flux multipliers to determine beats?
    private void _createBeatConfigs()
    {
        _beatConfigs.Add(_makeConfig(0, 0, 6, 20, 5, 4.0f));
        _beatConfigs.Add(_makeConfig(1, 30, 450, 20, 10, 2.5f));
    }

    private PBeatConfig _makeConfig(int band, int startIndex, int endIndex, int thresholdSize, int beatTime, float tresholdMult)
    {
        return new PBeatConfig(band, startIndex, endIndex, _hzPerBin * startIndex, _hzPerBin * endIndex, thresholdSize, beatTime, tresholdMult);
    }

}
