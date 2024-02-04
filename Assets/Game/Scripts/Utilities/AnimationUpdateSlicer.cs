using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUpdateSlicer : MonoBehaviour
{
    public static AnimationUpdateSlicer Instance;
    private readonly List<List<AnimationHandler>> slicedUpdateBehavioursBucket = new List<List<AnimationHandler>>();
    public int totalBucket = 2;
    public int frameSkip = 1;
    private int registerIndex = 0;
    int count = 0;
    [SerializeField]
    private int[] skips = { 0, 1, 2, 3, 4 },threshold= {6,10,15,20,25 };
    public void RegisterSlicedUpdate(AnimationHandler slicedUpdateBehaviour)
    {
        if(slicedUpdateBehavioursBucket[(registerIndex % slicedUpdateBehavioursBucket.Count)].Contains(slicedUpdateBehaviour))
        {
            return;
        }
        count++;
        slicedUpdateBehavioursBucket[(registerIndex++ % slicedUpdateBehavioursBucket.Count)].Add(slicedUpdateBehaviour);
        CheckFrameSkip();
    }
    void CheckFrameSkip()
    {
        for (int i = 0; i < threshold.Length; i++)
        {
            if (count < threshold[i])
            {
                frameSkip = skips[i];
                break;
            }
        }
    }
    public void DeregisterSlicedUpdate(AnimationHandler slicedUpdateBehaviour)
    {
        bool exist = false;
        for (int i = 0; i < slicedUpdateBehavioursBucket.Count; i++)
        {
            if (slicedUpdateBehavioursBucket[i].Remove(slicedUpdateBehaviour))
            {
                exist = true;
            }
        }
        if (exist)
        {
            count--;
            CheckFrameSkip();
        }
    }
    private void OnDestroy()
    {
        slicedUpdateBehavioursBucket.Clear();
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            lastUpdate = new float[totalBucket];
            for (int i = 0; i < totalBucket; i++)
            {
                slicedUpdateBehavioursBucket.Add(new List<AnimationHandler>());
            }
        }
    }
    int current = 0;
    float [] lastUpdate;
    int frame = 0;
    void LateUpdate()
    {
        var targetUpdateFunctions = slicedUpdateBehavioursBucket[current];
        if (frame >= frameSkip)
        {
            frame = 0;
            for(int i=0;i<targetUpdateFunctions.Count;i++)
            {
                var slicedUpdateBehaviour = targetUpdateFunctions[i];
                if (slicedUpdateBehaviour == null) continue;

                slicedUpdateBehaviour.OnUpdate((Time.time - lastUpdate[current])*GameTime.Controller.TIME_SCALE);
            }
            lastUpdate[current] = Time.time;
        }
        else
        {
            frame++;
        }
       

        current++;
        if (current >= slicedUpdateBehavioursBucket.Count)
        {
            current = 0;
        }
    }
}