using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Movement> images = new List<Movement> { };

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
        images[colorIndex].StartTracking(direction, isSimon, players, orderTime); ;
    }
}
