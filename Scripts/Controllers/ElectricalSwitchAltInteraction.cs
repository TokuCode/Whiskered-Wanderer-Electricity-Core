using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalAltSwitchInteraction : MonoBehaviour, IInteractableAlternate
{
    [Header("Electrical Settings")]
    [SerializeField] private SwitchElectrode switchElectrode;

    [Header("Interactable Settings")]
    [SerializeField, Range(1f, 100f)] private float horizontalInteractionRange;
    [SerializeField, Range(1f, 100f)] private float verticalInteractionRange;
    [Space]
    [SerializeField] private bool canBeSelected;
    [SerializeField] private bool isInteractable;
    [SerializeField] private bool hasAlreadyBeenInteracted;
    [Space]
    [SerializeField] private bool grabPetAttention;
    [SerializeField] private bool grabPlayerAttention;
    [SerializeField] private Transform interactionAttentionTransform;
    [SerializeField] private Transform interactionPositionTransform;

    #region IHoldInteractable Properties
    public float HorizontalInteractionRange => horizontalInteractionRange;
    public float VerticalInteractionRange => verticalInteractionRange;
    public bool IsSelectableAlternate => canBeSelected;
    public bool IsInteractableAlternate => isInteractable;
    public bool HasAlreadyBeenInteractedAlternate => hasAlreadyBeenInteracted;
    public string TooltipMessageAlternate => $"{(!switchElectrode.SwitchOn ? "Encender Switch" : "Apagar Switch")}";
    public bool GrabPetAttention => grabPetAttention;
    public bool GrabPlayerAttention => grabPlayerAttention;
    #endregion

    #region IHoldInteractable Events
    public event EventHandler OnObjectSelectedAlternate;
    public event EventHandler OnObjectDeselectedAlternate;
    public event EventHandler OnObjectInteractedAlternate;
    public event EventHandler OnObjectFailInteractedAlternate;
    public event EventHandler OnObjectHasAlreadyBeenInteractedAlternate;
    public event EventHandler OnUpdatedInteractableAlternateState;
    #endregion

    #region IHoldInteractable Methods
    public void SelectAlternate()
    {
        OnObjectSelectedAlternate?.Invoke(this, EventArgs.Empty);
        Debug.Log("Electrical Switch Selected");
    }
    public void DeselectAlternate()
    {
        OnObjectDeselectedAlternate?.Invoke(this, EventArgs.Empty);
        Debug.Log("Electrical Switch Deselected");
    }
    public void TryInteractAlternate()
    {
        if (!IsInteractableAlternate)
        {
            FailInteractAlternate();
            return;
        }

        if (HasAlreadyBeenInteractedAlternate)
        {
            AlreadyInteractedAlternate();
            return;
        }

        InteractAlternate();
    }
    public void InteractAlternate()
    {
        SwitchComponent();

        Debug.Log("Electrical Switch Interacted");
        OnObjectInteractedAlternate?.Invoke(this, EventArgs.Empty);
    }
    public void FailInteractAlternate()
    {
        Debug.Log("Cant Interact with Electrical Switch");
        OnObjectFailInteractedAlternate?.Invoke(this, EventArgs.Empty);
    }
    public void AlreadyInteractedAlternate()
    {
        Debug.Log("Electrical Switch has Already Been Interacted");
        OnObjectHasAlreadyBeenInteractedAlternate?.Invoke(this, EventArgs.Empty);
    }

    public Transform GetTransform() => transform;

    public Transform GetInteractionAlternateAttentionTransform() => interactionAttentionTransform;
    public Transform GetInteractionAlternatePositionTransform() => interactionPositionTransform;
    #endregion

    private void SwitchComponent()
    {
        switchElectrode.SetSwitch(!switchElectrode.SwitchOn);
        OnUpdatedInteractableAlternateState?.Invoke(this, EventArgs.Empty);
    }
}
