// GENERATED AUTOMATICALLY FROM 'Assets/Motor/Scripts/MotorActionMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MotorActionMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MotorActionMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MotorActionMap"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""31374f73-1d79-46ab-bf83-1fe126c7e95d"",
            ""actions"": [
                {
                    ""name"": ""ClickLeft"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6a5f52ad-7012-4976-bb9f-adf9f8bd1d64"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePos"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a32091fe-6698-4ac2-85ae-d7168713837e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""674e9736-126c-4784-bf13-08fdd5379f55"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClickLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""365557dc-9716-4c48-92d4-cf2ba37826f1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_ClickLeft = m_Mouse.FindAction("ClickLeft", throwIfNotFound: true);
        m_Mouse_MousePos = m_Mouse.FindAction("MousePos", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_ClickLeft;
    private readonly InputAction m_Mouse_MousePos;
    public struct MouseActions
    {
        private @MotorActionMap m_Wrapper;
        public MouseActions(@MotorActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @ClickLeft => m_Wrapper.m_Mouse_ClickLeft;
        public InputAction @MousePos => m_Wrapper.m_Mouse_MousePos;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @ClickLeft.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnClickLeft;
                @ClickLeft.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnClickLeft;
                @ClickLeft.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnClickLeft;
                @MousePos.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePos;
                @MousePos.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePos;
                @MousePos.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePos;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ClickLeft.started += instance.OnClickLeft;
                @ClickLeft.performed += instance.OnClickLeft;
                @ClickLeft.canceled += instance.OnClickLeft;
                @MousePos.started += instance.OnMousePos;
                @MousePos.performed += instance.OnMousePos;
                @MousePos.canceled += instance.OnMousePos;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    public interface IMouseActions
    {
        void OnClickLeft(InputAction.CallbackContext context);
        void OnMousePos(InputAction.CallbackContext context);
    }
}
