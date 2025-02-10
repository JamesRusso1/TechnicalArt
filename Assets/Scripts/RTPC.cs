using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class RTPC : MonoBehaviour
{
    // Initialize needed variables
    private Camera myCamera; // main camera

    public float farThreshold = 10f; // distance at which effects begin
    public float closeThreshold = 2f; // distance at which effects max out
    public float rotationSpeed = 15f; // speed at which table will rotate when floating
    public float minSpawnRate = 0f; // minimum possible particle spawns per second
    public float maxSpawnRate = 64f; // maximum possible particle spawns per second

    private VisualEffect visualEffect;
    private VisualEffect visualEffect2;

    Vector3 originalPosition = new Vector3();
    Vector3 tempPos = new Vector3();

    private Vector3 velocity = Vector3.zero;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCamera = FindFirstObjectByType<Camera>(); // Finds main camera
        visualEffect = transform.Find("SmokeVFX").GetComponent<VisualEffect>(); // Finds VFX child
        visualEffect2 = transform.Find("SmokeVFX 1").GetComponent<VisualEffect>(); // Finds VFX child

        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Interpolates between the minimum and maximum particle spawn amounts based on proximity to object
        float parameterValue = Mathf.InverseLerp(farThreshold, closeThreshold, DistanceChecker(myCamera.transform, this.gameObject.transform));
        parameterValue = Mathf.Lerp(minSpawnRate, maxSpawnRate, parameterValue);
        visualEffect.SetFloat("SpawnRate", Mathf.Clamp(parameterValue, minSpawnRate, maxSpawnRate));
        visualEffect2.SetFloat("SpawnRate", Mathf.Clamp(parameterValue, minSpawnRate, maxSpawnRate));

        if (DistanceChecker(myCamera.transform, this.gameObject.transform) <= farThreshold) { // if within distance threshold, float
            print("true!");
            tempPos = originalPosition;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI) + 1;
            // transform.position = tempPos;
            transform.position = Vector3.SmoothDamp(transform.position, tempPos, ref velocity, 1f);
            transform.Rotate(new Vector3(0f, Time.deltaTime * rotationSpeed, 0f), Space.World);
        } else { // once out of distance threshold, smoothly return to original position
            print("not true!");
            transform.position = Vector3.SmoothDamp(transform.position, originalPosition, ref velocity, 1f);
        }
    }

    // Returns Vector3 distance between two given objects
    float DistanceChecker(Transform obj1, Transform obj2)
    {
        return Vector3.Distance(obj1.position, obj2.position);
    }
}
