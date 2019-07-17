using UnityEngine;
using PSpectrumData;

public class BlockDataControllerPlain : ScriptableObject
{
    private FastList<SpectrumInfo> _spectrumDataList;

    public void createBlockData(FastList<SpectrumInfo> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        FastList<BlockDataPlain> blockDataList = new FastList<BlockDataPlain>();
    }

}
