using UnityEngine;
using AudioSpectrumInfo;
using System.Collections.Generic;

public class BlockDataControllerPlain : ScriptableObject
{
    private List<AnalyzedSpectrumConfig> _spectrumDataList;

    public void createBlockData(List<AnalyzedSpectrumConfig> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        List<BlockDataPlain> blockDataList = new List<BlockDataPlain>();
    }

}
