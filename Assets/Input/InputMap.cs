// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMap"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a75b6c7c-cfdf-4c67-8205-732e204e6864"",
            ""actions"": [
                {
                    ""name"": ""MovementAxes"",
                    ""type"": ""Value"",
                    ""id"": ""1141c2a9-ffe8-4c8f-aefb-8cb4e12a356a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""0239016a-e85d-41d9-b33a-8de6a406bfe6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""8e2dd8d9-6879-433f-828a-c77eb2df8809"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""d4b3f1ff-3698-423c-a31e-6b1c6eca2d86"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""a58add29-0d72-41be-8b1f-2e3c4f0b13a2"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5906131b-fa97-4983-a187-5a754b70045a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""84b5b29d-e9df-42c1-a8b4-b679fcb028e4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0e62373f-558e-48b4-864b-120fb525f077"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""db7a7b5b-b54f-44d9-9f26-a92f5bb79105"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""19c22c4c-c904-417f-a655-c4c188b72c2f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c068e7cb-5c71-4d37-80e4-e0f3902c7e7c"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1ce360c5-6850-4904-83ae-5b5a033e3a01"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""424f451c-f379-4c4d-bfcc-65030cbfd2ec"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""77901d9e-589f-4df9-9db4-647bfa0176c3"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c72e0c77-f0da-4261-9563-89d54f622092"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12f5cb01-a011-4169-9d2a-1eebfc5561f1"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a64623ae-4d4d-4262-ad5d-26b52e4fca5a"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e53efd37-a80d-4a03-8f7f-ab446e2b05da"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c7af81c-6c85-429e-9778-1b670a493386"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86a7267c-4cb0-4592-8f0d-2a1c90db5a10"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Grid Editor"",
            ""id"": ""8713fedc-7e93-4c97-93a9-e49762f1852e"",
            ""actions"": [
                {
                    ""name"": ""CursorMovement"",
                    ""type"": ""Button"",
                    ""id"": ""ff6c2454-f332-40c4-b427-963590064c4c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HeightModifier"",
                    ""type"": ""Button"",
                    ""id"": ""14897c81-c6bd-4c34-9e3f-c8cf5976810f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Next Tile"",
                    ""type"": ""Button"",
                    ""id"": ""2f1f2286-fc94-4f92-98f5-a2cd372ed94a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Previous Tile"",
                    ""type"": ""Button"",
                    ""id"": ""1dc987a5-6a35-4a89-8792-90a8e9bfbe93"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate Tile"",
                    ""type"": ""Button"",
                    ""id"": ""40ce89f6-8730-40f5-ad07-ec05b67f179a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Delete Tile"",
                    ""type"": ""Button"",
                    ""id"": ""6bf9968d-201e-4a13-aa17-f6b4e3de545b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Place Tile"",
                    ""type"": ""Button"",
                    ""id"": ""ccb0a875-75af-41a8-8340-f037120be160"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""bebeff1b-63c9-44b0-93c0-5655192c6faf"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""306b4c32-78ef-4fd1-9139-3281057fc992"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f0d20e89-0031-4506-b0e4-5ab444c82838"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7f5f16f8-8ae8-48f0-b242-00a6c728723a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""cb654eb5-17a2-4b0e-a38d-8be6cc189a24"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""c970ccca-213d-4a2b-99d7-4441e8238a45"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a7bca947-28fd-4817-a846-0cdd6669b195"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""306b859e-ddb4-4484-be49-050f94f60433"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""10487d06-5357-414f-8cc1-2d596152beed"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""95ef39a1-f9d4-447e-8f7b-a8c536c97ae1"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""7c01dd13-e28b-4dea-a93f-0c1a9142d11e"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""47d7d9c5-35e7-42e6-8247-470f22a3ed27"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ca28b56e-e9c8-4a67-8db8-27c294df1ee7"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""302975f8-2f43-40ce-b7f7-c7aeb215b7d7"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""10f0c788-c98c-462f-b1f1-2f53849562e8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0b451647-18d2-4115-88d1-5acaadcfd77f"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeightModifier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""598ee559-1223-4b50-8ea3-b05e72c7f4e7"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next Tile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a2f8eea-5ff1-4377-aa3b-86fd50e72c78"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Previous Tile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c0c79c5-53ca-4e51-a238-1f79a44b942c"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Tile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d093867-71fe-4fd0-91a4-9ce930d76349"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Delete Tile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30722938-f5b3-4d5b-a0ab-e4fc7a92f40a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Place Tile"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Free Camera"",
            ""id"": ""c83a2006-33f1-475c-ad94-cc0602b5b271"",
            ""actions"": [
                {
                    ""name"": ""KeyboardMovement"",
                    ""type"": ""Value"",
                    ""id"": ""d6fde5ec-874e-4f41-be5c-9302f13c105d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseRotation"",
                    ""type"": ""Value"",
                    ""id"": ""e401166f-162d-4af4-ac94-1b25a72480ba"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""366c3fed-8f56-4269-a6f5-e0ff6b4e5470"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""37aa8165-2a23-4999-9da8-c5d04b5c7277"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3ed690a4-b1e1-442c-bade-2990516e8fcd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8f88caec-ea4e-46c3-92be-959bbdb5173d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""beac0bf8-12ff-4cd3-9b32-66d0e1bcbce8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KeyboardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""599c2ba1-1bf1-4968-8f09-b1e51fc8e1b4"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Global"",
            ""id"": ""3e41d99f-d468-44e0-9e0f-9f2983545c27"",
            ""actions"": [
                {
                    ""name"": ""CameraOn"",
                    ""type"": ""Button"",
                    ""id"": ""28c8fed0-93b2-40da-96d4-662799b3d05c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dcb695a2-7e0a-46aa-b5f2-8c009d9bd019"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MovementAxes = m_Player.FindAction("MovementAxes", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Shoot = m_Player.FindAction("Shoot", throwIfNotFound: true);
        // Grid Editor
        m_GridEditor = asset.FindActionMap("Grid Editor", throwIfNotFound: true);
        m_GridEditor_CursorMovement = m_GridEditor.FindAction("CursorMovement", throwIfNotFound: true);
        m_GridEditor_HeightModifier = m_GridEditor.FindAction("HeightModifier", throwIfNotFound: true);
        m_GridEditor_NextTile = m_GridEditor.FindAction("Next Tile", throwIfNotFound: true);
        m_GridEditor_PreviousTile = m_GridEditor.FindAction("Previous Tile", throwIfNotFound: true);
        m_GridEditor_RotateTile = m_GridEditor.FindAction("Rotate Tile", throwIfNotFound: true);
        m_GridEditor_DeleteTile = m_GridEditor.FindAction("Delete Tile", throwIfNotFound: true);
        m_GridEditor_PlaceTile = m_GridEditor.FindAction("Place Tile", throwIfNotFound: true);
        // Free Camera
        m_FreeCamera = asset.FindActionMap("Free Camera", throwIfNotFound: true);
        m_FreeCamera_KeyboardMovement = m_FreeCamera.FindAction("KeyboardMovement", throwIfNotFound: true);
        m_FreeCamera_MouseRotation = m_FreeCamera.FindAction("MouseRotation", throwIfNotFound: true);
        // Global
        m_Global = asset.FindActionMap("Global", throwIfNotFound: true);
        m_Global_CameraOn = m_Global.FindAction("CameraOn", throwIfNotFound: true);
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
    private readonly InputAction m_Player_MovementAxes;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Shoot;
    public struct PlayerActions
    {
        private @InputMap m_Wrapper;
        public PlayerActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @MovementAxes => m_Wrapper.m_Player_MovementAxes;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Shoot => m_Wrapper.m_Player_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MovementAxes.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementAxes;
                @MovementAxes.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementAxes;
                @MovementAxes.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementAxes;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MovementAxes.started += instance.OnMovementAxes;
                @MovementAxes.performed += instance.OnMovementAxes;
                @MovementAxes.canceled += instance.OnMovementAxes;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Grid Editor
    private readonly InputActionMap m_GridEditor;
    private IGridEditorActions m_GridEditorActionsCallbackInterface;
    private readonly InputAction m_GridEditor_CursorMovement;
    private readonly InputAction m_GridEditor_HeightModifier;
    private readonly InputAction m_GridEditor_NextTile;
    private readonly InputAction m_GridEditor_PreviousTile;
    private readonly InputAction m_GridEditor_RotateTile;
    private readonly InputAction m_GridEditor_DeleteTile;
    private readonly InputAction m_GridEditor_PlaceTile;
    public struct GridEditorActions
    {
        private @InputMap m_Wrapper;
        public GridEditorActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @CursorMovement => m_Wrapper.m_GridEditor_CursorMovement;
        public InputAction @HeightModifier => m_Wrapper.m_GridEditor_HeightModifier;
        public InputAction @NextTile => m_Wrapper.m_GridEditor_NextTile;
        public InputAction @PreviousTile => m_Wrapper.m_GridEditor_PreviousTile;
        public InputAction @RotateTile => m_Wrapper.m_GridEditor_RotateTile;
        public InputAction @DeleteTile => m_Wrapper.m_GridEditor_DeleteTile;
        public InputAction @PlaceTile => m_Wrapper.m_GridEditor_PlaceTile;
        public InputActionMap Get() { return m_Wrapper.m_GridEditor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GridEditorActions set) { return set.Get(); }
        public void SetCallbacks(IGridEditorActions instance)
        {
            if (m_Wrapper.m_GridEditorActionsCallbackInterface != null)
            {
                @CursorMovement.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnCursorMovement;
                @CursorMovement.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnCursorMovement;
                @CursorMovement.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnCursorMovement;
                @HeightModifier.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnHeightModifier;
                @HeightModifier.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnHeightModifier;
                @HeightModifier.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnHeightModifier;
                @NextTile.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnNextTile;
                @NextTile.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnNextTile;
                @NextTile.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnNextTile;
                @PreviousTile.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnPreviousTile;
                @PreviousTile.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnPreviousTile;
                @PreviousTile.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnPreviousTile;
                @RotateTile.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnRotateTile;
                @RotateTile.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnRotateTile;
                @RotateTile.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnRotateTile;
                @DeleteTile.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnDeleteTile;
                @DeleteTile.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnDeleteTile;
                @DeleteTile.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnDeleteTile;
                @PlaceTile.started -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnPlaceTile;
                @PlaceTile.performed -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnPlaceTile;
                @PlaceTile.canceled -= m_Wrapper.m_GridEditorActionsCallbackInterface.OnPlaceTile;
            }
            m_Wrapper.m_GridEditorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CursorMovement.started += instance.OnCursorMovement;
                @CursorMovement.performed += instance.OnCursorMovement;
                @CursorMovement.canceled += instance.OnCursorMovement;
                @HeightModifier.started += instance.OnHeightModifier;
                @HeightModifier.performed += instance.OnHeightModifier;
                @HeightModifier.canceled += instance.OnHeightModifier;
                @NextTile.started += instance.OnNextTile;
                @NextTile.performed += instance.OnNextTile;
                @NextTile.canceled += instance.OnNextTile;
                @PreviousTile.started += instance.OnPreviousTile;
                @PreviousTile.performed += instance.OnPreviousTile;
                @PreviousTile.canceled += instance.OnPreviousTile;
                @RotateTile.started += instance.OnRotateTile;
                @RotateTile.performed += instance.OnRotateTile;
                @RotateTile.canceled += instance.OnRotateTile;
                @DeleteTile.started += instance.OnDeleteTile;
                @DeleteTile.performed += instance.OnDeleteTile;
                @DeleteTile.canceled += instance.OnDeleteTile;
                @PlaceTile.started += instance.OnPlaceTile;
                @PlaceTile.performed += instance.OnPlaceTile;
                @PlaceTile.canceled += instance.OnPlaceTile;
            }
        }
    }
    public GridEditorActions @GridEditor => new GridEditorActions(this);

    // Free Camera
    private readonly InputActionMap m_FreeCamera;
    private IFreeCameraActions m_FreeCameraActionsCallbackInterface;
    private readonly InputAction m_FreeCamera_KeyboardMovement;
    private readonly InputAction m_FreeCamera_MouseRotation;
    public struct FreeCameraActions
    {
        private @InputMap m_Wrapper;
        public FreeCameraActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @KeyboardMovement => m_Wrapper.m_FreeCamera_KeyboardMovement;
        public InputAction @MouseRotation => m_Wrapper.m_FreeCamera_MouseRotation;
        public InputActionMap Get() { return m_Wrapper.m_FreeCamera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FreeCameraActions set) { return set.Get(); }
        public void SetCallbacks(IFreeCameraActions instance)
        {
            if (m_Wrapper.m_FreeCameraActionsCallbackInterface != null)
            {
                @KeyboardMovement.started -= m_Wrapper.m_FreeCameraActionsCallbackInterface.OnKeyboardMovement;
                @KeyboardMovement.performed -= m_Wrapper.m_FreeCameraActionsCallbackInterface.OnKeyboardMovement;
                @KeyboardMovement.canceled -= m_Wrapper.m_FreeCameraActionsCallbackInterface.OnKeyboardMovement;
                @MouseRotation.started -= m_Wrapper.m_FreeCameraActionsCallbackInterface.OnMouseRotation;
                @MouseRotation.performed -= m_Wrapper.m_FreeCameraActionsCallbackInterface.OnMouseRotation;
                @MouseRotation.canceled -= m_Wrapper.m_FreeCameraActionsCallbackInterface.OnMouseRotation;
            }
            m_Wrapper.m_FreeCameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @KeyboardMovement.started += instance.OnKeyboardMovement;
                @KeyboardMovement.performed += instance.OnKeyboardMovement;
                @KeyboardMovement.canceled += instance.OnKeyboardMovement;
                @MouseRotation.started += instance.OnMouseRotation;
                @MouseRotation.performed += instance.OnMouseRotation;
                @MouseRotation.canceled += instance.OnMouseRotation;
            }
        }
    }
    public FreeCameraActions @FreeCamera => new FreeCameraActions(this);

    // Global
    private readonly InputActionMap m_Global;
    private IGlobalActions m_GlobalActionsCallbackInterface;
    private readonly InputAction m_Global_CameraOn;
    public struct GlobalActions
    {
        private @InputMap m_Wrapper;
        public GlobalActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraOn => m_Wrapper.m_Global_CameraOn;
        public InputActionMap Get() { return m_Wrapper.m_Global; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlobalActions set) { return set.Get(); }
        public void SetCallbacks(IGlobalActions instance)
        {
            if (m_Wrapper.m_GlobalActionsCallbackInterface != null)
            {
                @CameraOn.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnCameraOn;
                @CameraOn.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnCameraOn;
                @CameraOn.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnCameraOn;
            }
            m_Wrapper.m_GlobalActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CameraOn.started += instance.OnCameraOn;
                @CameraOn.performed += instance.OnCameraOn;
                @CameraOn.canceled += instance.OnCameraOn;
            }
        }
    }
    public GlobalActions @Global => new GlobalActions(this);
    public interface IPlayerActions
    {
        void OnMovementAxes(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
    public interface IGridEditorActions
    {
        void OnCursorMovement(InputAction.CallbackContext context);
        void OnHeightModifier(InputAction.CallbackContext context);
        void OnNextTile(InputAction.CallbackContext context);
        void OnPreviousTile(InputAction.CallbackContext context);
        void OnRotateTile(InputAction.CallbackContext context);
        void OnDeleteTile(InputAction.CallbackContext context);
        void OnPlaceTile(InputAction.CallbackContext context);
    }
    public interface IFreeCameraActions
    {
        void OnKeyboardMovement(InputAction.CallbackContext context);
        void OnMouseRotation(InputAction.CallbackContext context);
    }
    public interface IGlobalActions
    {
        void OnCameraOn(InputAction.CallbackContext context);
    }
}
