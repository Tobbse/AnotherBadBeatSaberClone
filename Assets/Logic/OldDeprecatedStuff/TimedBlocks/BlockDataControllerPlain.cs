using UnityEngine;
using PSpectrumInfo;

public class BlockDataControllerPlain : ScriptableObject
{
    private FastList<AnalyzedSpectrumData> _spectrumDataList;

    public void createBlockData(FastList<AnalyzedSpectrumData> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        FastList<BlockDataPlain> blockDataList = new FastList<BlockDataPlain>();
    }

}
