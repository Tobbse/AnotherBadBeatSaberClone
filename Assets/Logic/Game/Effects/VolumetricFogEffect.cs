using UnityEngine;

/**
 * Effect behavior that is added to fog objects and enables or disables the fog renderer,
 * depending on whether or not it should currently be active (meaning an effect is running).
 **/
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
