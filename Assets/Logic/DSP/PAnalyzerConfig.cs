using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PAnalyzerConfig
{
    private FastList<PBeatConfig> _beatConfigs;
    private int _bands;
    private const int NUM_FREQ_BINS = PSpectrumProvider.NUM_BINS;

    private float _frequencyPerBin;

    public PAnalyzerConfig(int bands)
    {
        _bands = bands;
        _frequencyPerBin = (float)AudioSettings.outputSampleRate / NUM_FREQ_BINS;
        _beatConfigs = new FastList<PBeatConfig>();

        _setupBeatConfigs();
    }

    private void _setupBeatConfigs()
    {
        IEnumerable<double> enumerator = PLogarithmUtils.LinSpace(0.0d, (double)NUM_FREQ_BINS, _bands + 1);
        List<double> logValues = enumerator.ToList();

        for (var i = 0; i < logValues.Count - 1; i++) {
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
    }


    public FastList<PBeatConfig> BeatConfigs
    {
        get { return _beatConfigs; }
    }

    public int Bands
    {
        get { return _bands; }
    }

}
