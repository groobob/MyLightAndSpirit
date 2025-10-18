using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PressurePlate : Switch
{
    private bool is_On = false;
    private bool was_On = false;
    private bool pressureFound = false;

    [Header("For Pressure Plate")]
    [SerializeField] private bool keyBlockRequired = false;
    [SerializeField] private GameObject[] keyBlocks;

    void Start()
    {
        base.init();
        fullDisabled = true;
        changeVisibility(false);
    }
    protected override void Update()
    {
        base.Update();
        checkBlockAbove();
        
    }

    private void checkBlockAbove()
    {

        Collider2D[] colliderList = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.5f, 0.5f), 0);
        pressureFound = false;
        foreach (Collider2D collider in colliderList)
        {
            InteractableBlock interactable = collider.gameObject.GetComponent<InteractableBlock>();
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            PlayerMove plrMove = collider.gameObject.GetComponent<PlayerMove>();
            if ((interactable || enemy || plrMove) && !keyBlockRequired)
            {
                plateOn();
            }
            else if (keyBlockRequired && keyBlocks.Contains(collider.gameObject))
            {
                //Debug.Log("keyblock found");
                plateOn();
            }
        }

        if (!pressureFound && was_On)
        {
            is_On = false;
            was_On = false;
            plateDeinteract();
        }
    }

    private void plateOn()
    {
        pressureFound = true;
        if (!is_On)
        {
            is_On = true;
            was_On = true;
            plateInteract();
        }
    }

    private void plateInteract()
    {
        switch (switchMode)
        {
            case SwitchMode.ToggleAppear:
                toggleAppear(); break;
            case SwitchMode.ToggleDisappear:
                toggleDisappear(); break;
            case SwitchMode.ToggleMovement:
                toggleLinkedBlockMovement(); break;
            case SwitchMode.ToggleShine:
                toggleLinkedBlockShine(); break;
        }
    }

    private void plateDeinteract()
    {
        switch (switchMode)
        {
            case SwitchMode.ToggleAppear:
                toggleAppearOff(); break;
            case SwitchMode.ToggleDisappear:
                toggleDisappearOff(); break;
            case SwitchMode.ToggleMovement:
                toggleLinkedBlockMovement(); break;
            case SwitchMode.ToggleShine:
                toggleLinkedBlockShine(); break;
        }
    }
}
