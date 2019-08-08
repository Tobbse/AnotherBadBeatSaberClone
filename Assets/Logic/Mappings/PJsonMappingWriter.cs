using UnityEngine;
using PSpectrumInfo;

public class PJsonMappingWriter : ScriptableObject
{
    private FastList<PBeatInfo> _beatConfig;
    private string _version;

    public PJsonMappingWriter(string version)
    {
        _version = version;
    }

    public void setData(FastList<PBeatInfo> beatConfig, string songName, string bandName)
    {
        _beatConfig = beatConfig;
    }

    public void writeData()
    {

    }

}