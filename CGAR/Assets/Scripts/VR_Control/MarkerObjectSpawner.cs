using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerObjectSpawner : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject prefab;

    private GameObject spawnedObject;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.added)
        {
            Spawn(image);
        }

        foreach (var image in args.updated)
        {
            UpdatePose(image);
        }
    }

    void Spawn(ARTrackedImage image)
    {
        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(prefab);
        }

        UpdatePose(image);
        spawnedObject.SetActive(true);
    }

    void UpdatePose(ARTrackedImage image)
    {
        spawnedObject.transform.SetPositionAndRotation(
            image.transform.position,
            image.transform.rotation
        );
    }
}
