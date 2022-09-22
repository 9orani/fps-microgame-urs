using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Unity.FPS.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        GameFlowManager m_GameFlowManager;
        PlayerCharacterController m_PlayerCharacterController;
        bool m_FireInputWasHeld;

        void Start()
        {
            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerInputHandler>(
                m_PlayerCharacterController, this, gameObject);
            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, PlayerInputHandler>(m_GameFlowManager, this);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            m_FireInputWasHeld = GetFireInputHeld();
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked && !m_GameFlowManager.GameIsEnding;
        }

        protected float getAxisRaw(string axis){
            if(axis == GameConstants.k_AxisNameHorizontal){
                return getAxisRawHorizental();
            }
            else if(axis == GameConstants.k_AxisNameVertical){
                return getAxisRawVertical();
            }
            else{
                return 0.0f;
            }
        }
        protected float getAxisRawHorizental(){
            if(Keyboard.current.dKey.wasPressedThisFrame){
                return 1.0f;
            }
            else if(Keyboard.current.aKey.wasPressedThisFrame){
                return -1.0f;
            }
            else{
                return 0.0f;
            }
        }
        protected float getAxisRawVertical(){
            if(Keyboard.current.wKey.wasPressedThisFrame){
                return 1.0f;
            }
            else if(Keyboard.current.sKey.wasPressedThisFrame){
                return -1.0f;
            }
            else{
                return 0.0f;
            }
        }
        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(getAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    getAxisRaw(GameConstants.k_AxisNameVertical));

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal,
                GameConstants.k_AxisNameJoystickLookHorizontal);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical,
                GameConstants.k_AxisNameJoystickLookVertical);
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return ((KeyControl)Keyboard.current[GameConstants.k_ButtonNameJump]).wasPressedThisFrame;
            }

            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return ((KeyControl)Keyboard.current[GameConstants.k_ButtonNameJump]).isPressed;            
            }

            return false;
        }

        public bool GetFireInputDown()
        {
            return GetFireInputHeld() && !m_FireInputWasHeld;
        }

        public bool GetFireInputReleased()
        {
            return !GetFireInputHeld() && m_FireInputWasHeld;
        }

        public bool GetFireInputHeld()
        {
            if (CanProcessInput())
            {
                return Mouse.current.leftButton.isPressed;
            }

            return false;
        }

        public bool GetAimInputHeld()
        {
            if (CanProcessInput())
            {
                bool i = Mouse.current.rightButton.isPressed;
                return i;
            }

            return false;
        }

        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return ((KeyControl)Keyboard.current[GameConstants.k_ButtonNameSprint]).isPressed;
            }

            return false;
        }

        public bool GetCrouchInputDown()
        {
            if (CanProcessInput())
            {
                return ((KeyControl)Keyboard.current[GameConstants.k_ButtonNameCrouch]).wasPressedThisFrame;
            }

            return false;
        }

        public bool GetCrouchInputReleased()
        {
            if (CanProcessInput())
            {
                return ((KeyControl)Keyboard.current[GameConstants.k_ButtonNameCrouch]).wasReleasedThisFrame;
            }

            return false;
        }

        public bool GetReloadButtonDown()
        {
            if (CanProcessInput())
            {
                return ((KeyControl)Keyboard.current[GameConstants.k_ButtonReload]).wasPressedThisFrame;
            }

            return false;
        }

        public int GetSwitchWeaponInput()
        {
            if (CanProcessInput())
            {

                string axisName = GameConstants.k_ButtonNameSwitchWeapon;

                if (Input.GetAxis(axisName) > 0f)
                    return -1;
                else if (Input.GetAxis(axisName) < 0f)
                    return 1;
            }

            return 0;
        }

        public int GetSelectWeaponInput()
        {
            if (CanProcessInput())
            {
                if (((KeyControl)Keyboard.current["1"]).wasPressedThisFrame)
                    return 1;
                else if (((KeyControl)Keyboard.current["2"]).wasPressedThisFrame)
                    return 2;
                else if (((KeyControl)Keyboard.current["3"]).wasPressedThisFrame)
                    return 3;
                else if (((KeyControl)Keyboard.current["4"]).wasPressedThisFrame)
                    return 4;
                else if (((KeyControl)Keyboard.current["5"]).wasPressedThisFrame)
                    return 5;
                else if (((KeyControl)Keyboard.current["6"]).wasPressedThisFrame)
                    return 6;
                else if (((KeyControl)Keyboard.current["7"]).wasPressedThisFrame)
                    return 7;
                else if (((KeyControl)Keyboard.current["8"]).wasPressedThisFrame)
                    return 8;
                else if (((KeyControl)Keyboard.current["9"]).wasPressedThisFrame)
                    return 9;
                else
                    return 0;
            }

            return 0;
        }

        float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
        {
            if (CanProcessInput())
            {
                // Check if this look input is coming from the mouse
                float i = Input.GetAxisRaw(mouseInputName);
                // handle inverting vertical input
                if (InvertYAxis)
                    i *= -1f;

                // apply sensitivity multiplier
                i *= LookSensitivity;

                    // reduce mouse input amount to be equivalent to stick movement
                i *= 0.01f;
#if UNITY_WEBGL
                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                i *= WebglLookSensitivityMultiplier;
#endif
                return i;
            }

            return 0f;
        }
    }
}