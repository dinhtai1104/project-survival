using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletMovement
{
    Stat Speed { get; set; }
    void Move();
    void Reset();
}