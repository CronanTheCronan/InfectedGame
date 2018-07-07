using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{

    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Controls")]
        [SerializeField] float speed = 6f;
        [SerializeField] float jumpSpeed = 8f;
        [SerializeField] float gravity = 20f;
        [SerializeField] float moveThreshold = 1f;
        [SerializeField] float groundCheckDistance = 0.1f;
        [SerializeField] float movingTurnSpeed = 360f;
        [SerializeField] float stationaryTurnSpeed = 180f;
        [Range(1f, 4f)] [SerializeField] float gravityMultiplier = 2f;

        Transform mainCamera;
        Vector3 move;
        Vector3 cameraForwardDirection;
        Vector3 groundNormal;
        bool jumping = false;
        bool isGrounded;
        float turnAmount;
        float forwardAmount;


        void Start()
        {
            if (Camera.main != null)
            {
                mainCamera = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Warning: No camera in scene.  Third person character needs a camera tagged main camera.");
            }
        }

        private void FixedUpdate()
        {
            float x = CrossPlatformInputManager.GetAxis("Horizontal");
            float z = CrossPlatformInputManager.GetAxis("Vertical");
            
            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            if(mainCamera != null)
            {
                cameraForwardDirection = Vector3.Scale(mainCamera.forward, new Vector3(1,0,1)).normalized;
                move = z * cameraForwardDirection + x * mainCamera.right;
            }
            

            Move(move, jumping);
        }

        private void Move(Vector3 movement, bool jump)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            HandleVelocity(jumping);
        }

        private void SetForwardAndTurn(Vector3 movement)
        {
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }

            var localMove = transform.InverseTransformDirection(movement);
            CheckGroundStatus();
            movement = Vector3.ProjectOnPlane(movement, groundNormal);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        private void HandleVelocity(bool jump)
        {
            if(isGrounded)
            {
                HandleGroundedMovement(jump);
            }
            else
            {
                HandleAirborneMovement();
            }
        }

        private void HandleAirborneMovement()
        {
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            //playerRigidbody.AddForce(extraGravityForce);

            //groundCheckDistance = playerRigidBody.velocity.y < 0 ? originalGroundCheckDistance : 0.01f;
        }

        private void HandleGroundedMovement(bool jump)
        {
            // check whether conditions are right to allow a jump:
            //if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            //{
            //    // jump!
            //    playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, jumpPower, playerRigidbody.velocity.z);
            //    isGrounded = false;
            //    animator.applyRootMotion = false;
            //    groundCheckDistance = 0.1f;
            //}
        }

        private void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;

            if(Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                groundNormal = hitInfo.normal;
                isGrounded = true;
                //animator.applyRootMotion = true;
            }
            else
            {
                isGrounded = false;
                groundNormal = Vector3.up;
                //animator.applyRootMotion = false;
            }
        }
    }
}
