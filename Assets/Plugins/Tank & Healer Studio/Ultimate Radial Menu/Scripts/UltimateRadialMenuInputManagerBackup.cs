/* UltimateRadialMenuInputManager.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UltimateRadialMenuInputManagerBackup : MonoBehaviour
{
    public static UltimateRadialMenuInputManagerBackup Instance
    {
        get;
        private set;
    }
    Camera mainCamera;
    public class UltimateRadialMenuInfomation
    {
        public UltimateRadialMenu radialMenu;
        public bool lastRadialMenuState = false;
    }
    public List<UltimateRadialMenuInfomation> UltimateRadialMenuInformations
    {
        get;
        private set;
    }

    // INTERNAL CALCULATIONS //
    bool enableMenu = false;
    bool disableMenu = false;
    bool inputDown = false;
    bool inputUp = false;
    Vector2 input = Vector2.zero;
    float distance = 0.0f;
    LayerMask worldSpaceMask;

    // INTERACT SETTINGS //
    public enum InvokeAction
    {
        OnButtonDown,
        OnButtonClick
    }
    [Header("Interact Settings")]
    [Tooltip("The action required to invoke the radial button.")]
    public InvokeAction invokeAction = InvokeAction.OnButtonDown;
    [Tooltip("Determines whether or not the Ultimate Radial Menu will receive input when the Ultimate Radial Menu is released and disabled.")]
    public bool onMenuRelease = false;
    [Tooltip("Determines if the Ultimate Radial Menu should be disabled when the interaction occurs. \n\nNOTE: World space radial menus will not be disabled on interact. They must be disabled manually.")]
    public bool disableOnInteract = false;

    // MOUSE SETTINGS //
    [Header("Mouse and Keyboard Settings")]
    [Tooltip("Determines if mouse and keyboard input should be used to send to the Ultimate Radial Menu.")]
    public bool keyboardInput = true;

    public InputActionAsset radialMenuInputSystem;

    // CONTROLLER SETTINGS //
    [Header("Controller Settings")]
    [Tooltip("Determines if controller input should be used to send to the Ultimate Radial Menu.")]
    public bool controllerInput = false;
    [Tooltip("Determines if the horizontal input should be inverted or not.")]
    public bool invertHorizontal = false;
    [Tooltip("Determines if the vertical input should be inverted or not.")]
    public bool invertVertical = false;

    // VR SETTINGS //
    [Header("Virtual Reality Settings")]
    [Tooltip("Determines if the menu should activated by the center of the screen.")]
    public bool virtualRealityInput = false;

    bool menuEnabled = false;


    void Awake()
    {
        // If this input manager is not located on the event system...
        if (!GetComponent<EventSystem>())
        {
            // Log an error to the user explaining the issue and what to do to fix it.
            Debug.LogError("Ultimate Radial Menu Input Manager\nThis component is not attached to the EventSystem in your scene. Please make sure that you have only one Ultimate Radial Menu Input Manager in your scene and that it is located on the EventSystem.");

            // Destroy this component and return.
            Destroy(this);
            return;
        }

        // Assign the instance as this.
        Instance = this;

        UltimateRadialMenuInformations = new List<UltimateRadialMenuInfomation>();

        // Store the LayerMask for the UI so that it can be used for world space menus.
        worldSpaceMask = LayerMask.GetMask("UI");
    }

    void Start()
    {
        // Set the main camera for calculations.
        UpdateCamera();

        radialMenuInputSystem.FindActionMap("PlayerControls").FindAction("Inventory").started += ctx => EnableMenus();

        radialMenuInputSystem.FindActionMap("InventoryControls").FindAction("Interact").performed += ctx => InteractPressed();
        radialMenuInputSystem.FindActionMap("InventoryControls").FindAction("Interact").canceled += ctx => InteractReleased();
    }

    private void OnEnable()
    {
        radialMenuInputSystem.Enable();
    }

    private void OnDisable()
    {
        radialMenuInputSystem.Disable();
    }

    void EnableMenus()
    {
        Debug.Log("ENABLE");
        if (!menuEnabled)
        {
            menuEnabled = true;
            enableMenu = true;
        }
        else
        {
            menuEnabled = false;
            disableMenu = true;
        }
    }

    void DisableMenus()
    {
        menuEnabled = false;
        disableMenu = true;
    }

    void InteractPressed()
    {
        inputDown = true;
    }

    void InteractReleased()
    {
        inputUp = true;
    }

    /// <summary>
    /// [INTERNAL] Called by each Ultimate Radial Menu.
    /// </summary>
    public void AddRadialMenuToList(UltimateRadialMenu radialMenu, ref UltimateRadialMenuInputManagerBackup inputManager)
    {
        // Add this radial menu to the list for calculations.
        UltimateRadialMenuInformations.Add(new UltimateRadialMenuInfomation() { radialMenu = radialMenu });

        inputManager = this;
    }

    /// <summary>
    /// Updates the current camera for calculations.
    /// </summary>
    void UpdateCamera()
    {
        // Find all the cameras in the scene.
        Camera[] sceneCameras = FindObjectsOfType<Camera>();

        // Loop through each camera.
        for (int i = 0; i < sceneCameras.Length; i++)
        {
            // If the camera gameobject is active and the camera component is enabled...
            if (sceneCameras[i].gameObject.activeInHierarchy && sceneCameras[i].enabled)
            {
                // Set this camera to the main camera.
                mainCamera = sceneCameras[i];

                // If this camera is tagged as MainCamera, then break the loop. Otherwise, keep looking for a MainCamera.
                if (sceneCameras[i].tag == "MainCamera")
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the camera to the provided camera parameter for calculations.
    /// </summary>
    /// <param name="newMainCamera">The new camera to use for calculations.</param>
    public void SetMainCamera(Camera newMainCamera)
    {
        mainCamera = newMainCamera;
    }

    /// <summary>
    /// Performs a physics raycast using the provided input information to see if the input collides with any world space menus.
    /// </summary>
    void RaycastWorldSpaceRadialMenu(ref Vector2 input, ref float distance, Vector2 rayOrigin, int radialMenuIndex)
    {
        // If the current radial menu is not used in world space, then just return.
        if (!UltimateRadialMenuInformations[radialMenuIndex].radialMenu.IsWorldSpaceRadialMenu)
            return;

        // If the main camera is null, not active, or the camera component is not enabled, then update the camera reference.
        if (mainCamera == null || !mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled)
            UpdateCamera();

        // Cast a ray from the mouse position.
        Ray ray = mainCamera.ScreenPointToRay(rayOrigin);

        // Temporary hit variable to store hit information.
        RaycastHit hit;

        // Raycast with the calculated information.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, worldSpaceMask))
        {
            // If the collider that was hit is this radial menu...
            if (hit.collider.gameObject == UltimateRadialMenuInformations[radialMenuIndex].radialMenu.gameObject)
            {
                // Configure the local 3D Position of hit.
                Vector3 localHitPosition = UltimateRadialMenuInformations[radialMenuIndex].radialMenu.ParentCanvas.transform.InverseTransformPoint(hit.point);

                // Configure the local position of the base.
                Vector3 localBasePosition = UltimateRadialMenuInformations[radialMenuIndex].radialMenu.ParentCanvas.transform.InverseTransformPoint(UltimateRadialMenuInformations[radialMenuIndex].radialMenu.BasePosition);

                // Configure the difference of the calculated vectors.
                input = (localHitPosition - localBasePosition);

                // Configure the distance of the mouse position from the Radial Menu's base position.
                distance = Vector3.Distance(UltimateRadialMenuInformations[radialMenuIndex].radialMenu.BasePosition, hit.point);
            }
        }
    }

    void Update()
    {
        // Loop through each of the radial menus.
        for (int i = 0; i < UltimateRadialMenuInformations.Count; i++)
        {
            // If the menu is null, then it must have been deleted...
            if (UltimateRadialMenuInformations[i].radialMenu == null)
            {
                // Update the list and break the loop to avoid errors.
                UltimateRadialMenuInformations.RemoveAt(i);
                break;
            }

            // If the user wants to use keyboard input then run the MouseAndKeyboardInput function.
            if (keyboardInput)
                MouseAndKeyboardInput(ref input, ref distance, i);

            // If the user wants to use controller input then run the ControllerInput function.
            if (controllerInput)
                ControllerInput(ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i);

            // If the user wants to use any VR devices then run the VirtualRealityInput function.
            if (virtualRealityInput)
                VirtualRealityInput(ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i);

            // If we want to activate the radial menu when we release the menu when hovering over a button...
            if (onMenuRelease)
            {
                // Check the last known radial menu state to see if it was active. If we are going to disable the menu on this frame and the last known state was true, then set interact to true.
                if (UltimateRadialMenuInformations[i].lastRadialMenuState == true && disableMenu == true)
                    inputDown = inputUp = true;
            }

            // Send all of the calculations to the Ultimate Radial Menu to process.
            UltimateRadialMenuInformations[i].radialMenu.ProcessInput(input, distance, inputDown, inputUp);

            // If we want to enable the radial menu on this frame then do that here.
            if (enableMenu)
                UltimateRadialMenuInformations[i].radialMenu.EnableRadialMenu();

            // Same this for the disable. Do that here.
            if (disableMenu)
                UltimateRadialMenuInformations[i].radialMenu.DisableRadialMenu();

            // Store the last known state for calculations.
            UltimateRadialMenuInformations[i].lastRadialMenuState = UltimateRadialMenuInformations[i].radialMenu.RadialMenuActive;

            // Reset all of the stored values.
            enableMenu = false;
            disableMenu = false;
            inputDown = false;
            inputUp = false;
            input = Vector2.zero;
            distance = 0.0f;
        }
    }

    /// <summary>
    /// This function will catch input from the Mouse and Keyboard and modify the information to send back to the Update function.
    /// </summary>
    /// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
    /// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
    /// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
    /// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
    /// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="radialMenuIndex">The current index of the selected radial button.</param>
    public virtual void MouseAndKeyboardInput(ref Vector2 input, ref float distance, int radialMenuIndex)
    {
        // If this radial menu is world space then send in the information to raycast from.
        if (UltimateRadialMenuInformations[radialMenuIndex].radialMenu.IsWorldSpaceRadialMenu)
            RaycastWorldSpaceRadialMenu(ref input, ref distance, radialMenuInputSystem.FindActionMap("InventoryControls").FindAction("DirectionalInput").ReadValue<Vector2>(), radialMenuIndex);
        // Else the radial menu is on the screen, so process mouse input.
        else
        {
            // Store the mouse position.
            Vector2 mPosition = radialMenuInputSystem.FindActionMap("InventoryControls").FindAction("DirectionalInput").ReadValue<Vector2>();
            Debug.Log("MPOSITION: " + mPosition);
            Debug.Log("BASEPOSITION: " + (Vector2)UltimateRadialMenuInformations[radialMenuIndex].radialMenu.BasePosition);
            // By subtracting the mouse position from the radial menu's position we get a relative number. Then we divide by the size of the base transform to give us an easier and more consistent number to work with.
            Vector2 modInput = (mPosition - (Vector2)UltimateRadialMenuInformations[radialMenuIndex].radialMenu.BasePosition) / UltimateRadialMenuInformations[radialMenuIndex].radialMenu.BaseTransform.sizeDelta.x;
            Debug.Log("MODINPUT: " + modInput);
            // Apply our new calculated input.
            input = modInput;

            // Configure the distance of the mouse position from the Radial Menu's base position.
            distance = Vector2.Distance(UltimateRadialMenuInformations[radialMenuIndex].radialMenu.BasePosition, mPosition);
        }
    }

    /// <summary>
    /// This function will catch input from the Controller and modify the information to send back to the Update function.
    /// </summary>
    /// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
    /// <param name="radialMenuIndex">The current index of the selected radial button.</param>
    public virtual void ControllerInput(ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex)
    {
        Vector2 userInput = radialMenuInputSystem.FindActionMap("InventoryControls").FindAction("DirectionalInput").ReadValue<Vector2>();

        // Store the horizontal and vertical axis of the targeted joystick axis.
        Vector2 modInput = new Vector2(invertHorizontal ? -userInput.x : userInput.x, invertVertical ? -userInput.y : userInput.y);

        // Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
        float tempDist = Vector2.Distance(Vector2.zero, modInput);

        // Set the input to what we have calculated.
        if (modInput != Vector2.zero)
            input = modInput;

        // If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
        if (tempDist >= UltimateRadialMenuInformations[radialMenuIndex].radialMenu.minRange)
            distance = Mathf.Lerp(UltimateRadialMenuInformations[radialMenuIndex].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[radialMenuIndex].radialMenu.CalculatedMaxRange, 0.5f);
    }

    /// <summary>
    /// This function will catch input from the center of the screen and VR device and modify the information to send back to the Update function.
    /// </summary>
    /// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
    /// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
    /// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
    /// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
    /// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
    /// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
    /// <param name="radialMenuIndex">The current index of the selected radial button.</param>
    public virtual void VirtualRealityInput(ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex)
    {
        // If the radial menu is not being used in world space, then just return.
        if (!UltimateRadialMenuInformations[radialMenuIndex].radialMenu.IsWorldSpaceRadialMenu)
            return;

        // Raycast from the center of the screen.
        RaycastWorldSpaceRadialMenu(ref input, ref distance, new Vector3(Screen.width / 2, Screen.height / 2, 0), radialMenuIndex);
    }
}