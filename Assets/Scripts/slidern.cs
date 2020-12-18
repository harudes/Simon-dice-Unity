using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class slidern : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = 10;
        slider.value = 10;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (slider.value > 0)
        {     
            slider.value -= Time.deltaTime;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
