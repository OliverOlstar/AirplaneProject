using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    //public GameObject target;
    private PlayerCamera _camera;
    public float offSetUp = 0.6f;

    void Start()
    {
        //Getting Reference to the camera
        _camera = GetComponentInChildren<PlayerCamera>();
    }

    public void tick(Vector3 pPosition)
    {
        //Position the camera pivot on the player
        transform.position = pPosition + (Vector3.up * offSetUp);
    }


    
    // Camera Transition //////////////

    public void ChangePlayerCamera(float pOffSetUp, float pOffSetLeft, float pMouseSensitivity, float pTurnDampening, float pCameraDistance, float pCameraMinHeight, float pCameraMaxHeight, float pTransitionSpeed)
    {
        StopCoroutine("CameraOffSetTransition");
        StartCoroutine(CameraOffSetTransition(pOffSetUp, pTransitionSpeed));
        _camera.ChangePlayerCamera(pOffSetLeft, pMouseSensitivity, pTurnDampening, pCameraDistance, pCameraMinHeight, pCameraMaxHeight, pTransitionSpeed);
    }

    private IEnumerator CameraOffSetTransition(float pOffSetUp, float pTransitionSpeed)
    {
        while (offSetUp != pOffSetUp)
        {
            offSetUp = Mathf.Lerp(offSetUp, pOffSetUp, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(offSetUp - pOffSetUp) <= 0.01f)
                offSetUp = pOffSetUp;

            yield return null;
        }
    }
}
