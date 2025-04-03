using System.Data;
using UnityEngine;


/**
 * Created by krmacdonald
 * Setup: Place the script onto an empty gameobject, interact with the inspector fields to modify
 * Prerequisite: A point for the script to start and end at as well as a gameobject for the stone list
 * Output: A cobblestone road generated from point A to point B on a curve
 */

public class CobblestonePath : MonoBehaviour
{
    public GameObject[] stones;
    public GameObject pointA;
    public GameObject pointB;
    public int stoneCount = 20; //Number of stones along the path
    public float deviationAmount = 0.5f; //Max deviation from the main path
    public float minScale; //Min size it can scale to, rec ~0.8
    public float maxScale; //Max size it can scale to, rec ~1.2
    public float randomStoneCount; //Number of stones the script will ATTEMPT to randomly generate in a cluster
    private void Start()
    {
        GeneratePath();
    }

    //Function that generates the stones along the path
    void GeneratePath()
    {
        if (stones.Length == 0 || pointA == null || pointB == null)
        {
            Debug.LogError("ERROR: Check inspector for missing fields");
            return;
        }

        Vector3 controlPoint = (pointA.transform.position + pointB.transform.position) / 2 + Vector3.up * 2; //Adjusts the height to create a curve, modify to change curve

        for (int i = 0; i <= stoneCount; i++)
        {
            float t = i / (float)stoneCount;
            Vector3 basePosition = CalculateBezierPoint(t, pointA.transform.position, controlPoint, pointB.transform.position);

            //Apply slight deviation to the left or right
            Vector3 deviation = Vector3.Cross(Vector3.up, (pointB.transform.position - pointA.transform.position).normalized) * Random.Range(-deviationAmount, deviationAmount);
            Vector3 position = basePosition + deviation;

            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            GameObject stone = Instantiate(stones[Random.Range(0, stones.Length)], position, rotation);
            stone.transform.localScale = new Vector3(25, 25, 25);
            stone.transform.localScale *= Random.Range(minScale, maxScale); //Slight variation in scale for realism
            
            //Occasionally spawn another stone at the same path point for a more natural road
            for(int j = 0; j < randomStoneCount; j++)
            {
                if (Random.value > 0.7f)
                {
                    Vector3 secondPosition = basePosition + Vector3.Cross(Vector3.up, deviation).normalized * Random.Range(0.1f, deviationAmount);
                    GameObject secondStone = Instantiate(stones[Random.Range(0, stones.Length)], secondPosition, rotation);
                    secondStone.transform.localScale *= Random.Range(minScale, maxScale);
                }
            }
        }
    }

    //Function that calculates the point alongside a curve between the two positions, 
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}
