using UnityEngine;
using PSpectrumData;

public class BlockDataControllerPlain : ScriptableObject
{
    private FastList<PSpectrumInfo> _spectrumDataList;

    public void createBlockData(FastList<PSpectrumInfo> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        FastList<BlockDataPlain> blockDataList = new FastList<BlockDataPlain>();
    }

}
