using UnityEngine;
using AudioAnalyzerPlain;

public class BlockDataControllerPlain : ScriptableObject
{
    private FastList<SpectrumData> _spectrumDataList;

    public void createBlockData(FastList<SpectrumData> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        FastList<BlockDataPlain> blockDataList = new FastList<BlockDataPlain>();
    }

}
