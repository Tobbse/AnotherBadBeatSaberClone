using UnityEngine;

public class VolumetricFogEffect : MonoBehaviour
{
    private int _activeFrames;
    private MeshRenderer meshRenderer;

    public int ActiveFrames { get => _activeFrames; set => _activeFrames = value; }

    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    void Update()
    {
        // TODO don't update this all the time?
        if (_activeFrames > 0)
        {
            meshRenderer.enabled = true;
            _activeFrames--;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
