using UnityEngine;
using PSpectrumInfo;

public class BlockDataControllerPlain : ScriptableObject
{
    private FastList<PAnalyzedSpectrumData> _spectrumDataList;

    public void createBlockData(FastList<PAnalyzedSpectrumData> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        FastList<BlockDataPlain> blockDataList = new FastList<BlockDataPlain>();
    }

}
