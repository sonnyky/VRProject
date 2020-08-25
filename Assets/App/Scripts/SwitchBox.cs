using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwitchBox : Interactable
{

    [SerializeField]
    GameObject m_Lights;

    private void Start()
    {
        m_Action += SwitchLights;
    }

    public void SwitchLights()
    {
        Debug.Log("Lights switched");
    }
 
}
