using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void Update()
    {
        Destroy(this.gameObject, .6f);
    }
}
