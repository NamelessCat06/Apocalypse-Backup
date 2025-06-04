using UnityEngine;
using FMODUnity;
using Cinemachine;
using System.Collections.Generic;

public class FMODCameraListenerManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraListenerPair
    {
        public CinemachineVirtualCamera virtualCamera;
        public StudioListener fmodListener;
    }

    [Header("Camera - FMOD Listener Pairs")]
    [SerializeField] private List<CameraListenerPair> cameraListenerPairs = new List<CameraListenerPair>();

    private void Start()
    {
        UpdateActiveListener();
    }

    private void Update()
    {
        UpdateActiveListener();
    }

    private void UpdateActiveListener()
    {
        CameraListenerPair activePair = null;
        float highestPriority = float.NegativeInfinity;

        // Find the highest priority active camera
        foreach (var pair in cameraListenerPairs)
        {
            if (pair.virtualCamera.isActiveAndEnabled && pair.virtualCamera.Priority > highestPriority)
            {
                activePair = pair;
                highestPriority = pair.virtualCamera.Priority;
            }
        }

        // Enable only the listener that matches the active camera
        foreach (var pair in cameraListenerPairs)
        {
            if (pair.fmodListener != null)
            {
                pair.fmodListener.enabled = (pair == activePair);
            }
        }
    }
}