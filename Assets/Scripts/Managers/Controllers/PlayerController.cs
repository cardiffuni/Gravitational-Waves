using UnityEngine;
using Mirror;
using Game.Players;
using System.Collections.Generic;
using System.Collections;

namespace Game.Managers.Controllers
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        public CharacterController characterController;

        public Camera PlayerCamera { get; private set; }

        [SyncVar(hook = nameof(UpdateID))]
        public int PlayerID = -1;
        public Player Player;

        public void SetPlayer(Player player) {
            Player = player;
            PlayerID = player.ID;
        }

        void OnValidate()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
        }

        void Start()
        {
            characterController.enabled = isLocalPlayer;
            
        }

        public override void OnStartLocalPlayer() {
            Debug.LogFormat("OnStartLocalPlayer");
            if (isLocalPlayer) {
                PlayerCamera = transform.Find("Player Camera").GetComponent<Camera>();
                CameraManager.Select(PlayerCamera);
                Camera.SetupCurrent(PlayerCamera);
            }
        }

        private void UpdateID(int _, int playerID) {
            Debug.LogFormat("PlayerID: {0}", playerID);
            PlayerID = playerID;
        }

        void OnDisable() {
            if (isLocalPlayer) {
                CameraManager.Overview();
            }
            
        }

        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public float turnSensitivity = 5f;
        public float maxTurnSpeed = 150f;

        [Header("Diagnostics")]
        public float horizontal;
        public float vertical;
        public float turn;
        public float jumpSpeed;
        public bool isGrounded = true;
        public bool isFalling;
        public Vector3 velocity;

        void Update()
        {
            if (!isLocalPlayer || !characterController.enabled)
                return;

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            // Q and E cancel each other out, reducing the turn to zero
            if (Input.GetKey(KeyCode.Q))
                turn = Mathf.MoveTowards(turn, -maxTurnSpeed, turnSensitivity);
            if (Input.GetKey(KeyCode.E))
                turn = Mathf.MoveTowards(turn, maxTurnSpeed, turnSensitivity);
            if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
                turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
            if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
                turn = Mathf.MoveTowards(turn, 0, turnSensitivity);

            if (isGrounded)
                isFalling = false;

            if ((isGrounded || !isFalling) && jumpSpeed < 1f && Input.GetKey(KeyCode.Space))
            {
                jumpSpeed = Mathf.Lerp(jumpSpeed, 1f, 0.5f);
            }
            else if (!isGrounded)
            {
                isFalling = true;
                jumpSpeed = 0;
            }
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer || characterController == null)
                return;

            transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

            Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = transform.TransformDirection(direction);
            direction *= moveSpeed;

            if (jumpSpeed > 0)
                characterController.Move(direction * Time.fixedDeltaTime);
            else
                characterController.SimpleMove(direction);

            isGrounded = characterController.isGrounded;
            velocity = characterController.velocity;
        }
    }
}
