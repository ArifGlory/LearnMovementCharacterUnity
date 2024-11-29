using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class AnimationAndMovementController : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        
        private PlayerInputActions playerInput;
        private CharacterController characterController;
        private Vector2 currentMovementInput;
        private Animator animator;
        
        Vector3 currentMovement;
        Vector3 currentRunMovement;
        bool isMovementPressed;
        bool isRunPressed;
        private float rotataionFactorPerFrame = 15.0f;
        private float runMultiplier = 3.0f;

        void Awake()
        {
            playerInput = new PlayerInputActions();
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            playerInput.CharacterControls.Move.started += onMovementInput;
            playerInput.CharacterControls.Move.canceled += onMovementInput;
            playerInput.CharacterControls.Move.performed += onMovementInput;
            
            playerInput.CharacterControls.Run.started += onRunInput;
            playerInput.CharacterControls.Run.canceled += onRunInput;
            playerInput.CharacterControls.Run.performed += onRunInput;
        }

        void handleRotation()
        {
            Vector3 positionToLookAt = default;
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = currentMovement.z;
            
            Quaternion currentRotation = transform.rotation;
            if (isMovementPressed)
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotataionFactorPerFrame * Time.deltaTime);
            }
        }

        void onMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            
            currentRunMovement.x = currentMovementInput.x * runMultiplier;
            currentRunMovement.z = currentMovementInput.y * runMultiplier;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }

        void onRunInput(InputAction.CallbackContext context)
        {
            isRunPressed = context.ReadValueAsButton();
        }

        void handleAnimation()
        {
            bool isWalking = animator.GetBool(IsWalking);
            bool isRunning = animator.GetBool(IsRunning);

            if (isMovementPressed && !isWalking)
            {
                animator.SetBool(IsWalking, true);
                
            }else if (!isMovementPressed && isWalking)
            {
                animator.SetBool(IsWalking, false);
            }


            if ((isMovementPressed && isRunPressed) && !isRunning)
            {
                animator.SetBool(IsRunning, true);
            }else if ((!isMovementPressed && !isRunPressed) && isRunning)
            {
                animator.SetBool(IsRunning, false);
            }
        }

        void handleGravity()
        {
            if (characterController.isGrounded)
            {
                float groundedGravity = -0.5f;
                currentMovement.y = groundedGravity;
                currentRunMovement.y = groundedGravity;
            }
            else
            {
                float gravity = -9.8f;
                currentMovement.y = gravity;
                currentRunMovement.y = gravity;
            }
        }
        
        void Update()
        {
            handleGravity();
            handleRotation();
            handleAnimation();
            
            if (isRunPressed)
            {
                characterController.Move(currentRunMovement * Time.deltaTime);
            }
            else
            {
                characterController.Move(currentMovement * Time.deltaTime);
            }
            
        }

        void OnEnable()
        {
            playerInput.CharacterControls.Enable();
        }

        void OnDisable()
        {
            playerInput.CharacterControls.Disable();
        }
        
    }
}
