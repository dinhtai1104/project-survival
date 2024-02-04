using UnityEditor;
using UnityEngine;


namespace GameTime
{
    [CustomEditor(typeof(Controller))]
    public class TimeEditor : Editor 
    {
        public string scale = "1";
        private void OnEnable()
        {
            scale = Controller.TIME_SCALE.ToString();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            scale = GUILayout.TextArea(scale);

            if (GUILayout.Button("SET TIME"))
            {
                Controller.TIME_SCALE = float.Parse(scale);
            }
        }
    }
   
}