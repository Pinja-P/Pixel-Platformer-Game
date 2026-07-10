using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Aitem
{
    public void Collect()
    {
        Destroy(gameObject);
    }

}
