using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    /// <summary>
    /// Display an item (Ex: exclamation mark) to show the user this object is interactable
    /// </summary>
    [SerializeField]
    GameObject m_InteractableSignPrefab;

    private GameObject m_InteractableSign;

    bool m_Interacting = false;
    float m_TimeOfInteraction = 0f;

    /// <summary>
    /// Time until automatic confirmation of action
    /// </summary>
    [SerializeField]
    float m_TimeUntilConfirmation;

    bool m_CanConfirmAutomatically = true;

    public System.Action m_Action;

    public virtual void StartInteraction()
    {

    }

    private void Confirmed()
    {
        if(m_Action != null)
        {
            m_Action.Invoke();
        }
    }

    public void OnPointerEnter()
    {
        Debug.Log("Pointer enter");
        m_Interacting = true;
    }

    public void OnPointerExit()
    {
        StopAndResetTimer();
    }

    private void StopAndResetTimer()
    {
        m_Interacting = false;
        m_TimeOfInteraction = 0f;
    }

    private void Update()
    {
        if (m_Interacting)
        {
            TimerUntilConfirmation();
            if(m_TimeOfInteraction > m_TimeUntilConfirmation)
            {
                Confirmed();
                StopAndResetTimer();
            }
        }
    }

    public virtual void TimerUntilConfirmation()
    {
        m_TimeOfInteraction += Time.deltaTime;
    }
}
