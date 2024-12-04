using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterMovement : MonoBehaviour
{
    public CharacterController _controller;
    private bool _hasController = false;
    private Vector3 _playerVelocity;
    private float _gravityValue = -9.81f;
    public float _speed = 10.0F;
    public float _rotateSpeed = 10.0F;
    public float _FB = 0;
    public float _LR = 0;

    //private ObjectTracker objectTracker;
    private Monster _monster;

    void Awake()
    {
        //objectTracker = FindObjectOfType<ObjectTracker>();
        _monster = GetComponent<Monster>();
        _controller = GetComponent<CharacterController>();
    }

    public void Move(float FB, float LR)
    {
        //clamp the values of LR and FB
        LR = Mathf.Clamp(LR, -1, 1);
        FB = Mathf.Clamp(FB, 0, 1);

        //move the agent
        if (!_monster.isDead)
        {
            // Rotate around y - axis
            transform.Rotate(0, LR * _rotateSpeed, 0);

            // Move forward / backward
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            _controller.SimpleMove(forward * _speed * FB * -1);
        }

        GravityCheck();
    }

    void GravityCheck()
    {
        //Checks to see if the monster is grounded, if not apply gravity
        if (_controller.isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }
        else
        {
            // Gravity
            _playerVelocity.y += _gravityValue * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);
        }
    }
}