using UnityEditor;

[CustomEditor(typeof(Spread_ShootPattern))]
public class ScripableObjectGizmos : Editor
{
    private void OnEnable()
    {
        SceneView.duringSceneGui += DrawGizmos;

    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DrawGizmos;
    }

    private void DrawGizmos(SceneView sceneView)
    {
        // draw some Gizmos-like gizmo
        Spread_ShootPattern pattern = (Spread_ShootPattern)target;
        foreach(var emitter in pattern.emitters)
        {
            Handles.DrawWireDisc(emitter.position, UnityEngine.Vector3.forward,0.2f);
            Handles.DrawLine(emitter.position, emitter.position+emitter.direction*99);
        }
    }
}