using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

public class ObservableValueExample : MonoBehaviour
{
    private ObservableValue<int> intAsObservable;
    public int intValue = 0;


    void Start()
    {
        intAsObservable = new ObservableValue<int>(intValue);
        intAsObservable.OnValueChange = (i) => {
            Debug.Log("change value:" + i.ToString());
        };
    }

    void Update()
    {
        intAsObservable.Value = intValue;
    }
}
