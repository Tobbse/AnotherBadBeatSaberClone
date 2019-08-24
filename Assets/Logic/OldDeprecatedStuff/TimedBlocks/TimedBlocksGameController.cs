using UnityEngine;
using AudioSpectrumInfo;
using System.Collections.Generic;

public class TimedBlocksGameController : MonoBehaviour
{
    private BlockDataControllerPlain _blockDataController;

    // Start is called before the first frame update
    void Start()
    {
        _blockDataController = new BlockDataControllerPlain();
    }

    public void setSpectrumData(List<AnalyzedSpectrumConfig> spectrumDataList)
    {
        _blockDataController.createBlockData(spectrumDataList);
    }

}
