using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SphericalFog : MonoBehaviour
{
	protected MeshRenderer sphericalFogObject;
	public Material sphericalFogMaterial;
	public float scaleFactor = 1;

	void OnEnable ()
	{
		sphericalFogObject = gameObject.GetComponent<MeshRenderer>();
		if (sphericalFogObject == null)
			Debug.LogError("Volume Fog Object must have a MeshRenderer Component!");

        //Note: In forward lightning path, the depth texture is not automatically generated.

        // Fixes a bug in the Volumetric Fog library. This will prevent an error from being shown.
        // HOWEVER this will also completely stop the fog from being rendered for some reason.
        bool useBugfix = true;
        if (useBugfix)
        {
            if (Camera.main != null)
            {
                if (Camera.main.depthTextureMode == DepthTextureMode.None)
                {
                    Camera.main.depthTextureMode = DepthTextureMode.Depth;
                }
            }
            else
            {
                GameObject leftCameraObj = GameObject.Find("LeftEyeAnchor");
                if (leftCameraObj)
                {
                    Camera leftCamera = leftCameraObj.GetComponent<Camera>();
                    if (leftCamera != null && leftCamera.depthTextureMode == DepthTextureMode.None)
                    {
                        leftCamera.depthTextureMode = DepthTextureMode.Depth;
                    }
                }
                GameObject rightCameraObj = GameObject.Find("LeftEyeAnchor");
                if (rightCameraObj)
                {
                    Camera rightCamera = rightCameraObj.GetComponent<Camera>();
                    if (rightCamera != null && rightCamera.depthTextureMode == DepthTextureMode.None)
                    {
                        rightCamera.depthTextureMode = DepthTextureMode.Depth;
                    }
                }
            }
        } else // This will throw an error but make the fog work. // ACTUALLY it doesn't even do that? Why did it work before? Wtf?
        {
            if (Camera.main.depthTextureMode == DepthTextureMode.None)
                Camera.main.depthTextureMode = DepthTextureMode.Depth;
        }
        sphericalFogObject.material = sphericalFogMaterial;
	}

	void Update ()
	{
		float radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6;
		Material mat = Application.isPlaying ? sphericalFogObject.material : sphericalFogObject.sharedMaterial;
		if (mat)
			mat.SetVector ("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, radius * scaleFactor));
	}
}
