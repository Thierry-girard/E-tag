using UnityEngine;
using System.Collections;
using HoloToolkit.Unity.InputModule;

public class ParticlesOnClick : MonoBehaviour, IManipulationHandler, IHoldHandler, IInputHandler
{

	public float distance = 0;

	public ParticleSystem playerParticle;

    private bool fireParticle;

    private InputEventData holdEvent;

    public void OnHoldCanceled(HoldEventData eventData)
    {
        Debug.Log("Hold canceled");
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        Debug.Log("Hold completed");
        fireParticle = false;
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        Debug.Log("Hold started");
        fireParticle = true;
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("Input down");
        holdEvent = eventData;
    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("Input up");
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation canceled");
        fireParticle = false;
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation completed");
        fireParticle = false;
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation started");
        fireParticle = true;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation updated");
        fireParticle = true;
    }



    // Use this for initialization
    void Start () 
	{
        ParticleSystem.EmissionModule playerEmission = playerParticle.emission;
        playerEmission.enabled = false;
        fireParticle = false;
        holdEvent = null;
	}

	// Update is called once per frame
	void Update () 
	{
		if (fireParticle) {
            Vector3 systemPosition = Camera.main.transform.position;
            Quaternion systemRotation = Camera.main.transform.rotation;
            if (holdEvent != null)
            {
               // Debug.Log("HoldEvent not null");
                IInputSource currentInputSource = holdEvent.InputSource;
                uint currentInputSourceId = holdEvent.SourceId;

                Vector3 handPosition;
                if (currentInputSource.TryGetGripPosition(currentInputSourceId, out handPosition))
                {
                    handPosition += new Vector3(0, 0.03f, 0) ;
                    systemPosition = handPosition;

                    Vector3 heading = handPosition - Camera.main.transform.position;
                    float distance = heading.magnitude;
                    Vector3 direction = new Vector3(0, 0, 0);

                    if(distance != 0)
                        direction = heading / distance;

                    systemRotation = Quaternion.LookRotation(direction);
                }
            }
            ParticleSystem.EmissionModule playerEmission = playerParticle.emission;
            playerEmission.enabled = true;
            playerParticle.transform.position = systemPosition;
			playerParticle.transform.rotation = systemRotation;
		} else {
            ParticleSystem.EmissionModule playerEmission = playerParticle.emission;
            playerEmission.enabled = false;
		}
	}
}
