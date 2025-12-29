using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARMultiTrackedImageController : MonoBehaviour
{
    public ARTrackedImageManager arTrackedImageManager;

    public GameObject[] prefabs;

    private GameObject markerObj1, markerObj2, markerObj3, markerObj4, markerObj5, markerObj6, markerObj7;

    void OnEnable()
    {
        arTrackedImageManager.trackablesChanged.AddListener(OnChangeTrackingState);
    }

    void OnDisable()
    {
        arTrackedImageManager.trackablesChanged.RemoveListener(OnChangeTrackingState);
    }

    private void OnChangeTrackingState(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.name == "Marker1")
            {
                markerObj1 = Instantiate(prefabs[0]);
                markerObj1.SetActive(false);
            }

            if (trackedImage.referenceImage.name == "Marker2")
            {
                markerObj2 = Instantiate(prefabs[1]);
                markerObj2.SetActive(false);
            }

            if (trackedImage.referenceImage.name == "Marker3")
            {
                markerObj3 = Instantiate(prefabs[2]);
                markerObj3.SetActive(false);
            }

            if (trackedImage.referenceImage.name == "Marker4")
            {
                markerObj4 = Instantiate(prefabs[3]);
                markerObj4.SetActive(false);
            }

            if (trackedImage.referenceImage.name == "Marker5")
            {
                markerObj5 = Instantiate(prefabs[4]);
                markerObj5.SetActive(false);
            }

            if (trackedImage.referenceImage.name == "Marker6")
            {
                markerObj6 = Instantiate(prefabs[5]);
                markerObj6.SetActive(false);
            }

            if (trackedImage.referenceImage.name == "Marker7")
            {
                markerObj7 = Instantiate(prefabs[6]);
                markerObj7.SetActive(false);
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name == "Marker1")
            {
                markerObj1.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj1.SetActive(true);
            }

            if (trackedImage.referenceImage.name == "Marker2")
            {
                markerObj2.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj2.SetActive(true);
            }

            if (trackedImage.referenceImage.name == "Marker3")
            {
                markerObj3.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj3.SetActive(true);
            }

            if (trackedImage.referenceImage.name == "Marker4")
            {
                markerObj4.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj4.SetActive(true);
            }

            if (trackedImage.referenceImage.name == "Marker5")
            {
                markerObj5.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj5.SetActive(true);
            }

            if (trackedImage.referenceImage.name == "Marker6")
            {
                markerObj6.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj6.SetActive(true);
            }

            if (trackedImage.referenceImage.name == "Marker7")
            {
                markerObj7.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
                markerObj7.SetActive(true);
            }
        }
    }
}