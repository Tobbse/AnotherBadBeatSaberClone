using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatMappingConfigs;

/**
 * This class exposes an API to the MainEffectController to control laser effects.
 **/
public class LaserController : MonoBehaviour
{
    private Rigidbody[] _laserRigids = {};
    private List<Transform> _laserTransform = new List<Transform>();

    void Start()
    {
        _laserRigids = gameObject.GetComponentsInChildren<Rigidbody>();

        GameObject[] laserObjects = GameObject.FindGameObjectsWithTag("Laser");
        foreach (GameObject laser in laserObjects)
        {
            _laserTransform.Add(laser.transform);
        }
    }

    public void setRandomAngularSpeed(float magnitude)
    {
        foreach (Rigidbody laser in _laserRigids)
        {
            laser.angularVelocity = new Vector3(Random.Range(1, 3) * magnitude, Random.Range(1, 3) * magnitude, Random.Range(1, 3) * magnitude);
        }
    }

    public void multAngularSpeed(float speedFactor)
    {
        Vector3 currentRot;
        foreach (Rigidbody laser in _laserRigids)
        {
            currentRot = laser.angularVelocity;
            laser.angularVelocity = new Vector3(currentRot.x * speedFactor, currentRot.y * speedFactor, currentRot.z * speedFactor);
        }
    }

    public void setAbsoluteAngeularSpeed(float x, float y, float z)
    {
        foreach (Rigidbody laser in _laserRigids)
        {
            laser.angularVelocity = new Vector3(x, y, z);
        }
    }

    public void setRandomRotation(float magnitude)
    {
        foreach (Transform laser in _laserTransform)
        {
            laser.rotation = Quaternion.Euler(Random.Range(0, 30) * magnitude, Random.Range(0, 30) * magnitude, Random.Range(0, 30) * magnitude);
        }
    }
}
