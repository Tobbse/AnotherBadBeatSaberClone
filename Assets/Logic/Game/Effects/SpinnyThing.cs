using UnityEngine;

public class SpinnyThing : MonoBehaviour
{
    public GameObject topLight;
    public GameObject rightLight;
    public GameObject bottomLight;
    public GameObject leftLight;

    private int _horizontalLightFrames;
    private int _verticalLightFrames;

    public int HorizontalLightFrames { get => _horizontalLightFrames; set => _horizontalLightFrames = value; }
    public int VerticalLightFrames { get => _verticalLightFrames; set => _verticalLightFrames = value; }

    private void Start()
    {
        topLight.SetActive(false);
        rightLight.SetActive(false);
        bottomLight.SetActive(false);
        leftLight.SetActive(false);
    }

    void Update()
    {
        // TODO don't update this all the time?
        if (_horizontalLightFrames > 0)
        {
            leftLight.SetActive(true);
            rightLight.SetActive(true);
            _horizontalLightFrames--;
        } else
        {
            leftLight.SetActive(false);
            rightLight.SetActive(false);
        }

        if (_verticalLightFrames > 0)
        {
            bottomLight.SetActive(true);
            topLight.SetActive(true);
            _verticalLightFrames--;
        }
        else
        {
            bottomLight.SetActive(false);
            topLight.SetActive(false);
        }
    }
}
