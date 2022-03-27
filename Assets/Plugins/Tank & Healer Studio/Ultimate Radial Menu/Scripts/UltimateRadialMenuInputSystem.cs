//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Tank & Healer Studio/Ultimate Radial Menu/Scripts/UltimateRadialMenuInputSystem.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @UltimateRadialMenuInputSystem : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @UltimateRadialMenuInputSystem()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""UltimateRadialMenuInputSystem"",
    ""maps"": [
        {
            ""name"": ""Radial Menu"",
            ""id"": ""ebdb7cf6-1585-4dcf-9c22-c291f3901d56"",
            ""actions"": [
                {
                    ""name"": ""EnableMenu"",
                    ""type"": ""Button"",
                    ""id"": ""06c25b5f-e111-4050-8b41-8d7def41fc77"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""277dd20f-b29d-4e5e-88b0-cee697424eef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DirectionalInput"",
                    ""type"": ""Value"",
                    ""id"": ""3e0638a7-778e-4276-8ff3-2b7d37efe20e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""38b6dac2-d7e6-49ea-9daf-4831a16bd27f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""EnableMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9371d4c6-0b26-4d5f-ae87-c3bbc75f9617"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""EnableMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa188ba9-996b-40a1-8476-beb8bb4755c6"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a96e11d2-cdfa-4a35-bda2-e59f625ef72b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""624d44e9-212e-45fc-970d-f3b1308fbf84"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""DirectionalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Left Stick [Gamepad]"",
                    ""id"": ""e3104533-1c15-4871-ac2f-951015b13f83"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DirectionalInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2c8be096-f056-4648-bab7-e342819dcfca"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""DirectionalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f3fd2ecc-4dcc-4006-9a6d-1f0c04028c59"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""DirectionalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7b5e5f48-bb0f-42e4-8d81-71d765ecc4b5"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""DirectionalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e8c19aa4-28bc-492b-a1ec-9912a8866d1a"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""DirectionalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse and Keyboard"",
            ""bindingGroup"": ""Mouse and Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Virtual Reality"",
            ""bindingGroup"": ""Virtual Reality"",
            ""devices"": [
                {
                    ""devicePath"": ""<XRController>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<XRHMD>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<TrackedDevice>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Radial Menu
        m_RadialMenu = asset.FindActionMap("Radial Menu", throwIfNotFound: true);
        m_RadialMenu_EnableMenu = m_RadialMenu.FindAction("EnableMenu", throwIfNotFound: true);
        m_RadialMenu_Interact = m_RadialMenu.FindAction("Interact", throwIfNotFound: true);
        m_RadialMenu_DirectionalInput = m_RadialMenu.FindAction("DirectionalInput", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Radial Menu
    private readonly InputActionMap m_RadialMenu;
    private IRadialMenuActions m_RadialMenuActionsCallbackInterface;
    private readonly InputAction m_RadialMenu_EnableMenu;
    private readonly InputAction m_RadialMenu_Interact;
    private readonly InputAction m_RadialMenu_DirectionalInput;
    public struct RadialMenuActions
    {
        private @UltimateRadialMenuInputSystem m_Wrapper;
        public RadialMenuActions(@UltimateRadialMenuInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @EnableMenu => m_Wrapper.m_RadialMenu_EnableMenu;
        public InputAction @Interact => m_Wrapper.m_RadialMenu_Interact;
        public InputAction @DirectionalInput => m_Wrapper.m_RadialMenu_DirectionalInput;
        public InputActionMap Get() { return m_Wrapper.m_RadialMenu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RadialMenuActions set) { return set.Get(); }
        public void SetCallbacks(IRadialMenuActions instance)
        {
            if (m_Wrapper.m_RadialMenuActionsCallbackInterface != null)
            {
                @EnableMenu.started -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnEnableMenu;
                @EnableMenu.performed -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnEnableMenu;
                @EnableMenu.canceled -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnEnableMenu;
                @Interact.started -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnInteract;
                @DirectionalInput.started -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnDirectionalInput;
                @DirectionalInput.performed -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnDirectionalInput;
                @DirectionalInput.canceled -= m_Wrapper.m_RadialMenuActionsCallbackInterface.OnDirectionalInput;
            }
            m_Wrapper.m_RadialMenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @EnableMenu.started += instance.OnEnableMenu;
                @EnableMenu.performed += instance.OnEnableMenu;
                @EnableMenu.canceled += instance.OnEnableMenu;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @DirectionalInput.started += instance.OnDirectionalInput;
                @DirectionalInput.performed += instance.OnDirectionalInput;
                @DirectionalInput.canceled += instance.OnDirectionalInput;
            }
        }
    }
    public RadialMenuActions @RadialMenu => new RadialMenuActions(this);
    private int m_MouseandKeyboardSchemeIndex = -1;
    public InputControlScheme MouseandKeyboardScheme
    {
        get
        {
            if (m_MouseandKeyboardSchemeIndex == -1) m_MouseandKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse and Keyboard");
            return asset.controlSchemes[m_MouseandKeyboardSchemeIndex];
        }
    }
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    private int m_VirtualRealitySchemeIndex = -1;
    public InputControlScheme VirtualRealityScheme
    {
        get
        {
            if (m_VirtualRealitySchemeIndex == -1) m_VirtualRealitySchemeIndex = asset.FindControlSchemeIndex("Virtual Reality");
            return asset.controlSchemes[m_VirtualRealitySchemeIndex];
        }
    }
    public interface IRadialMenuActions
    {
        void OnEnableMenu(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDirectionalInput(InputAction.CallbackContext context);
    }
}