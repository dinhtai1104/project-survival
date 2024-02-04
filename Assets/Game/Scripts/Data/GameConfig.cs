using System;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

[CreateAssetMenu (menuName ="GameConfig/Config")]
public partial class GameConfig:ScriptableObject
{
    public bool editMode = true;

    
    
    [Serializable]
    public class Vector
    {
        public float x, y, z;

        public Vector()
        {
        }

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3 Vector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class CustomColor
    {
        public float r, g, b,a;

        public CustomColor()
        {
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }
  
}
