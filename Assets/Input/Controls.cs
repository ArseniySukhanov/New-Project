// GENERATED AUTOMATICALLY FROM 'Assets/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""2bb127bf-3ef8-491f-8a85-3cb929db4215"",
            ""actions"": [
                {
                    ""name"": ""Move and Attack"",
                    ""type"": ""Button"",
                    ""id"": ""9a60a642-1d23-4e50-9015-0e5e0bcf4539"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Additional Action"",
                    ""type"": ""Button"",
                    ""id"": ""3bc78400-e5e3-4be3-ae6a-06f7a960f873"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePlace"",
                    ""type"": ""Value"",
                    ""id"": ""c2ad7d4e-c3f9-4bc1-92bb-afbd4fb6dfc8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PauseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""d13e00af-bdf8-4f14-9654-d643719c37a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bc9de992-d8d7-406f-a4e8-4d4f4a743020"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Additional Action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""079c8ad9-56c0-441c-b337-fc4d7938f693"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move and Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""242aff9f-b7ca-4a5c-bd8d-4459b10a296e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MousePlace"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c4548ea2-b602-410d-8adc-a6d7b73f0ee0"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PauseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MoveandAttack = m_Player.FindAction("Move and Attack", throwIfNotFound: true);
        m_Player_AdditionalAction = m_Player.FindAction("Additional Action", throwIfNotFound: true);
        m_Player_MousePlace = m_Player.FindAction("MousePlace", throwIfNotFound: true);
        m_Player_PauseMenu = m_Player.FindAction("PauseMenu", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MoveandAttack;
    private readonly InputAction m_Player_AdditionalAction;
    private readonly InputAction m_Player_MousePlace;
    private readonly InputAction m_Player_PauseMenu;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveandAttack => m_Wrapper.m_Player_MoveandAttack;
        public InputAction @AdditionalAction => m_Wrapper.m_Player_AdditionalAction;
        public InputAction @MousePlace => m_Wrapper.m_Player_MousePlace;
        public InputAction @PauseMenu => m_Wrapper.m_Player_PauseMenu;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MoveandAttack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveandAttack;
                @MoveandAttack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveandAttack;
                @MoveandAttack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveandAttack;
                @AdditionalAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAdditionalAction;
                @AdditionalAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAdditionalAction;
                @AdditionalAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAdditionalAction;
                @MousePlace.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePlace;
                @MousePlace.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePlace;
                @MousePlace.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePlace;
                @PauseMenu.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPauseMenu;
                @PauseMenu.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPauseMenu;
                @PauseMenu.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPauseMenu;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveandAttack.started += instance.OnMoveandAttack;
                @MoveandAttack.performed += instance.OnMoveandAttack;
                @MoveandAttack.canceled += instance.OnMoveandAttack;
                @AdditionalAction.started += instance.OnAdditionalAction;
                @AdditionalAction.performed += instance.OnAdditionalAction;
                @AdditionalAction.canceled += instance.OnAdditionalAction;
                @MousePlace.started += instance.OnMousePlace;
                @MousePlace.performed += instance.OnMousePlace;
                @MousePlace.canceled += instance.OnMousePlace;
                @PauseMenu.started += instance.OnPauseMenu;
                @PauseMenu.performed += instance.OnPauseMenu;
                @PauseMenu.canceled += instance.OnPauseMenu;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMoveandAttack(InputAction.CallbackContext context);
        void OnAdditionalAction(InputAction.CallbackContext context);
        void OnMousePlace(InputAction.CallbackContext context);
        void OnPauseMenu(InputAction.CallbackContext context);
    }
}
