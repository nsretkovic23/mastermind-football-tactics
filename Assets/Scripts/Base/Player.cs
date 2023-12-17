using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public Field CurrentField { get; set; }
        [field: SerializeField] public Base.Manager Manager { get; set; }

    }
}