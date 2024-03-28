using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Transform treeRoom;
    public Transform rockRoom;

    void LateUpdate()
    {
        Vector3 pos = mainCamera.transform.position;
        Vector3 fwd = mainCamera.transform.forward;
        pos = rockRoom.InverseTransformPoint(pos);
        fwd = rockRoom.InverseTransformDirection(fwd);
        pos = treeRoom.TransformPoint(pos);
        fwd = treeRoom.TransformDirection(fwd);
        transform.position = pos;
        transform.LookAt(pos + fwd);
    }
}
