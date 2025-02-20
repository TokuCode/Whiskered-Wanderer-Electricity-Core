using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElectricalSwitchToggle : MonoBehaviour, IInteractable
{
    [Header("Electrical Settings")]
    [SerializeField] private ElectricalSwitch electricalSwitch;
    [SerializeField] private SwitchElectrode switchElectrode;

    [Header("Interactable Settings")]
    [SerializeField, Range(1f, 100f)] private float horizontalInteractionRange;
    [SerializeField, Range(1f, 100f)] private float verticalInteractionRange;
    [Space]
    [SerializeField] private bool canBeSelected;
    [SerializeField] private bool isInteractable;
    [SerializeField] private bool hasAlreadyBeenInteracted;
    [SerializeField] private string tooltipMessageOff;
    [SerializeField] private string tooltipMessageOn;
    [Space]
    [SerializeField] private bool grabPetAttention;
    [SerializeField] private bool grabPlayerAttention;
    [SerializeField] private Transform interactionAttentionTransform;
    [SerializeField] private Transform interactionPositionTransform;

    public static event EventHandler<OnSwitchToggleEventArgs> OnSwitchToggle;

    public class OnSwitchToggleEventArgs : EventArgs
    {
        public bool switchOn;
        public int id;
    }

    #region IHoldInteractable Properties
    public float HorizontalInteractionRange => horizontalInteractionRange;
    public float VerticalInteractionRange => verticalInteractionRange;
    public bool IsSelectable => canBeSelected;
    public bool IsInteractable => isInteractable;
    public bool HasAlreadyBeenInteracted => hasAlreadyBeenInteracted;
    public string TooltipMessage => $"{(!switchElectrode.SwitchOn ? tooltipMessageOff : tooltipMessageOn)}";
    public bool GrabPetAttention => grabPetAttention;
    public bool GrabPlayerAttention => grabPlayerAttention;
    #endregion

    #region IInteractable Events
    public event EventHandler OnObjectSelected;
    public event EventHandler OnObjectDeselected;
    public event EventHandler OnObjectInteracted;
    public event EventHandler OnObjectFailInteracted;
    public event EventHandler OnObjectHasAlreadyBeenInteracted;
    public event EventHandler OnUpdatedInteractableState;
    #endregion

    #region IInteractable Methods
    public void Select()
    {
        OnObjectSelected?.Invoke(this, EventArgs.Empty);
        //Debug.Log("Electrical Switch Selected");
    }
    public void Deselect()
    {
        OnObjectDeselected?.Invoke(this, EventArgs.Empty);
        //Debug.Log("Electrical Switch Deselected");
    }
    public void TryInteract()
    {
        if (!isInteractable)
        {
            FailInteract();
            return;
        }

        if (hasAlreadyBeenInteracted)
        {
            AlreadyInteracted();
            return;
        }

        Interact();
    }
    public void Interact()
    {
        SwitchComponent();

        //Debug.Log("Electrical Switch Interacted");
        OnObjectInteracted?.Invoke(this, EventArgs.Empty);
    }
    public void FailInteract()
    {
        Debug.Log("Cant Interact with Electrical Switch");
        OnObjectFailInteracted?.Invoke(this, EventArgs.Empty);
    }
    public void AlreadyInteracted()
    {
        Debug.Log("Electrical Switch has Already Been Interacted");
        OnObjectHasAlreadyBeenInteracted?.Invoke(this, EventArgs.Empty);
    }

    public Transform GetTransform() => transform;
    public Transform GetInteractionAttentionTransform() => interactionAttentionTransform;
    public Transform GetInteractionPositionTransform() => interactionPositionTransform;
    #endregion

    private void SwitchComponent()
    {
        switchElectrode.SetSwitch(!switchElectrode.SwitchOn);

        OnSwitchToggle?.Invoke(this, new OnSwitchToggleEventArgs { switchOn = switchElectrode.SwitchOn, id = electricalSwitch.ID });

        OnUpdatedInteractableState?.Invoke(this, EventArgs.Empty);
    }
}