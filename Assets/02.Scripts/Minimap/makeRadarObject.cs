using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class makeRadarObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RadarMap.instance.RegisterMapObject(this.gameObject);
    }
    void OnDisable()
    {
        RadarMap.instance.RemoveMapObject(this.gameObject);
    }


}
