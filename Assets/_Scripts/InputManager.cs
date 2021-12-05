using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //public variables
    public Vector3 moveAxisInput = Vector3.zero;
    public Vector3 moveAxis = Vector3.zero;
    public Vector3 lastValidMoveAxis = Vector3.zero;
    public Vector3 cameraAxis = Vector3.zero;

    public bool buttonDown_swing = false;
    public bool buttonUp_swing = false;
    public bool button_swing = false;

    public bool buttonDown_glide = false;
    public bool buttonUp_glide = false;
    public bool button_glide = false;

    public bool buttonDown_jump = false;
    public bool buttonUp_jump = false;
    public bool button_jump = false;

    public bool button_sprint = false;
    
    public bool buttonDown_dash= false;

    //private variables
    private Player player = null;

    //public methods
    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
        ComponentManager<InputManager>.Value = this;
    }
    public void UpdateInput()
    {
        Transform cameraTransform = Camera.main.transform;

        moveAxisInput = new Vector3(player.GetAxis("Horizontal"), 0, player.GetAxis("Vertical"));

        moveAxis = Vector3.zero;
        moveAxis += cameraTransform.right * moveAxisInput.x;
        moveAxis += cameraTransform.forward * moveAxisInput.z;
        moveAxis.y = 0f;

        cameraAxis = new Vector3(player.GetAxis("HorizontalCamera"), 0, player.GetAxis("VerticalCamera"));

        buttonDown_swing = player.GetButtonDown("Swing");
        buttonUp_swing = player.GetButtonUp("Swing");
        button_swing = player.GetButton("Swing");

        buttonDown_glide = player.GetButtonDown("Glide");
        buttonUp_glide = player.GetButtonUp("Glide");
        button_glide = player.GetButton("Glide");

        buttonUp_jump = player.GetButtonUp("Jump");
        buttonDown_jump = player.GetButtonDown("Jump");
        button_jump = player.GetButton("Jump");

        button_sprint = player.GetButton("Sprint");

        buttonDown_dash = player.GetButtonDown("Dash");

        if (moveAxis != Vector3.zero)
        {
            lastValidMoveAxis = moveAxis;
        }
    }

    //private methods
}
