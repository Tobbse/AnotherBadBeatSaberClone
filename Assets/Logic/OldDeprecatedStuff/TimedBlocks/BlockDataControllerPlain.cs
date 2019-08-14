using UnityEngine;
using AudioSpectrumInfo;
using System.Collections.Generic;

public class BlockDataControllerPlain : ScriptableObject
{
    private List<AnalyzedSpectrumData> _spectrumDataList;

    public void createBlockData(List<AnalyzedSpectrumData> spectrumDataList)
    {
        _spectrumDataList = spectrumDataList;

        List<BlockDataPlain> blockDataList = new List<BlockDataPlain>();
    }

}
