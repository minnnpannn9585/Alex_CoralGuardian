using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeaUrchin : MonoBehaviour
{
    public Transform[] curchinTrans;

    // Start is called before the first frame update
    void Start()
    {
        // Get all child transforms but exclude this GameObject's own transform
        curchinTrans = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
    }


}
