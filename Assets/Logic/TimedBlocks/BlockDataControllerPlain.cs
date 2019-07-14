using UnityEngine;
using AudioAnalyzerPlain;

public class BlockDataControllerPlain : ScriptableObject
{
    private FastList<PSpectrumData> _spectrumDataList;

    public void createBlockData(FastList<PSpectrumData> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        FastList<BlockDataPlain> blockDataList = new FastList<BlockDataPlain>();
    }

}
