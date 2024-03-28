using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject m_target;
    public Vector3 m_targetOffset;
    public float m_panSpeed = 180.0f;   // degrees per second
    public float m_tiltSpeed = 180.0f;  // degrees per second
    public float m_tiltMax = 60.0f;      // degrees
    public float m_tiltMin = 0.0f;    // degrees
    public float m_collRad = 0.1f;
    public float m_distSpeed = 4.0f;

    float m_distanceCurrent;
    float m_distanceOrig;
    float m_azimuth;
    float m_elevation;

    public class CamInput
    {
        public float m_pan;
        public float m_tilt;
    }
    CamInput m_input;

    void Start()
    {
        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        Vector3 p = transform.position - target;
        m_distanceCurrent = p.magnitude;
        m_distanceOrig = m_distanceCurrent;
        Vector3 pxz = p;
        p.y = 0.0f;
        float dxz = p.magnitude;
        m_azimuth = Mathf.Atan2(p.x, p.z);
        m_elevation = Mathf.Atan2(p.y, dxz);

        m_input = new CamInput();
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        m_input.m_pan = Input.GetAxis("Mouse X");
        m_input.m_tilt = -Input.GetAxis("Mouse Y");
        if (Input.GetKey(KeyCode.LeftArrow))
            m_input.m_pan -= 1.0f;
        if (Input.GetKey(KeyCode.RightArrow))
            m_input.m_pan += 1.0f;
        if (Input.GetKey(KeyCode.UpArrow))
            m_input.m_tilt += 1.0f;
        if (Input.GetKey(KeyCode.DownArrow))
            m_input.m_tilt -= 1.0f;
        m_input.m_pan = Mathf.Clamp(m_input.m_pan, -1.0f, 1.0f);
        m_input.m_tilt = Mathf.Clamp(m_input.m_tilt, -1.0f, 1.0f);
    }

    void LateUpdate()
    {
        m_azimuth += m_input.m_pan * m_panSpeed * Mathf.Deg2Rad * Time.deltaTime;
        if (m_azimuth > 180.0f)
            m_azimuth -= 360.0f;
        if (m_azimuth < -180.0f)
            m_azimuth += 360.0f;
        m_elevation += m_input.m_tilt * m_tiltSpeed * Mathf.Deg2Rad * Time.deltaTime;
        m_elevation = Mathf.Clamp(m_elevation, Mathf.Deg2Rad * m_tiltMin, Mathf.Deg2Rad * m_tiltMax);

        Vector3 target = m_target.transform.TransformPoint(m_targetOffset);
        Vector3 p;
        p.y = m_distanceCurrent * Mathf.Sin(m_elevation);
        float dxz = m_distanceCurrent * Mathf.Cos(m_elevation);
        p.x = dxz * Mathf.Sin(m_azimuth);
        p.z = dxz * Mathf.Cos(m_azimuth);

        // camera collisions
        Ray ray = new Ray(target, p);
        RaycastHit hitInfo;
        if (Physics.SphereCast(ray, m_collRad, out hitInfo, m_distanceCurrent + m_collRad))
        {
            m_distanceCurrent = hitInfo.distance - m_collRad;
            p.y = m_distanceCurrent * Mathf.Sin(m_elevation);
            dxz = m_distanceCurrent * Mathf.Cos(m_elevation);
            p.x = dxz * Mathf.Sin(m_azimuth);
            p.z = dxz * Mathf.Cos(m_azimuth);
        }
        else
        {
            float lerp = Mathf.Clamp01(m_distSpeed * Time.deltaTime);
            m_distanceCurrent = Mathf.Lerp(m_distanceCurrent, m_distanceOrig, lerp);
        }

        p += target;
        transform.position = p;
        transform.LookAt(target);
    }
}
