using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;
    public float PlayerSpeed = 2f;


    private NetworkManagerBase _networkManager;


    void Awake()
    {
        _controller = GetComponent<CharacterController>();

        /// Retrieve the networkManager of the scene
        NetworkManagerBase[] entities = FindObjectsOfType<NetworkManagerBase>();
        _networkManager = entities[0];
        
        

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (HasStateAuthority == false) return;

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

        _controller.Move(move);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
}
