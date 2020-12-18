using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkersController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<MoveDetection> markers = new List<MoveDetection> { };

    void Start()
    {
        /*markers.Add(GameObject.Find("Red Marker").GetComponent<MoveDetection>());
        markers.Add(GameObject.Find("Blue Marker").GetComponent<MoveDetection>());*/
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MovementOrder(int colorIndex, bool isSimon, List<string> players, int direction, float orderTime)
    {
        markers[colorIndex].StartTracking(direction, isSimon, players, orderTime);
        
    }
}
