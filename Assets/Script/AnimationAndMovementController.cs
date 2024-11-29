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
        bool isMovementPressed;
        private float rotataionFactorPerFrame = 1.0f;

        void Awake()
        {
            playerInput = new PlayerInputActions();
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            playerInput.CharacterControls.Move.started += onMovementInput;
            playerInput.CharacterControls.Move.canceled += onMovementInput;
            playerInput.CharacterControls.Move.performed += onMovementInput;
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
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
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
            
        }

        void Update()
        {
            handleRotation();
            handleAnimation();
            characterController.Move(currentMovement * Time.deltaTime);
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
