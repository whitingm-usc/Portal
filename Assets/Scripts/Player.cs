using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_walkSpeed = 4.0f;
    public float m_turnSpeed = 360.0f;  // degrees per second

    public class CharInput
    {
        public Vector3 m_direction;
        public float m_facingAngle;
        public bool m_attack;
    }

    Animator m_anim;
    CharacterController m_char;
    CharInput m_input;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_char = GetComponent<CharacterController>();
        m_input = new CharInput();
    }

    // Update is called once per frame
    void Update()
    {
        // face the way the camera faces
        m_input.m_facingAngle = Camera.main.transform.localEulerAngles.y;

        // read WASD input
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move.z += 1.0f;
        if (Input.GetKey(KeyCode.S))
            move.z -= 1.0f;
        if (Input.GetKey(KeyCode.D))
            move.x += 1.0f;
        if (Input.GetKey(KeyCode.A))
            move.x -= 1.0f;
        move.x = Mathf.Clamp(move.x, -1.0f, 1.0f);
        move.z = Mathf.Clamp(move.z, -1.0f, 1.0f);

        // read attack input
        if (Input.GetMouseButtonDown(0))
            m_input.m_attack = true;

        // convert from camera-space to world-space movement
        Vector3 fwd = Camera.main.transform.forward;
        fwd.Normalize();
        Vector3 rt = Camera.main.transform.right;
        rt.Normalize();
        Vector3 moveWorld = move.z * fwd + move.x * rt;
        m_input.m_direction = moveWorld;

        // move in the direction of the input
        m_char.SimpleMove(m_input.m_direction * m_walkSpeed);

        // turn to face the angle in the input
        Vector3 ang = transform.localEulerAngles;
        float diff = m_input.m_facingAngle - ang.y;
        if (diff > 180.0f)
            diff -= 360.0f;
        if (diff < -180.0f)
            diff += 360.0f;
        float maxRate = m_turnSpeed * Time.deltaTime;
        diff = Mathf.Clamp(diff, -maxRate, maxRate);
        ang.y += diff;
        transform.localEulerAngles = ang;

        // update the animation
        Vector3 moveChar = transform.InverseTransformDirection(m_input.m_direction);

        if (m_anim)
        {
            m_anim.SetFloat("fwdSpeed", moveChar.z);
            m_anim.SetFloat("rightSpeed", moveChar.x);
        }
        if (m_input.m_attack)
        {
            if (m_anim)
                m_anim.SetTrigger("doAttack");
            m_input.m_attack = false;
        }
    }
}
