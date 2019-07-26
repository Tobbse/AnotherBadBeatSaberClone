public class PBeatConfig
{
    public int band;
    public int startIndex;
    public int endIndex; // End index is NOT included
    public int thresholdSize;
    public int beatBlockCounter;
    public float startFrequency;
    public float endFrequency;
    public float tresholdMult;

    public PBeatConfig(int bandP = 0, int startIndexP = 0, int endIndexP = 0, float startFrequencyP = 0, float endFrequencyP = 0, int thresholdSizeP = 0, int beatTimeP = 0, float tresholdMultP = 0)
    {
        band = bandP;
        startIndex = startIndexP;
        endIndex = endIndexP;
        startFrequency = startFrequencyP;
        endFrequency = endFrequencyP;
        thresholdSize = thresholdSizeP;
        beatBlockCounter = beatTimeP;
        tresholdMult = tresholdMultP;
    }
}
