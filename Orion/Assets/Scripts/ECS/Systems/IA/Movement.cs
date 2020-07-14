using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class Movement : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Entities.ForEach((ref Translation pos, in PlayerStatsData playerStatsData) =>
        {
            int toDebug = (int) pos.Value.x;

            

            if (pos.Value.x < 0)
            {
                float threshold = -0.5f;
                if (toDebug + threshold < pos.Value.x)
                {
                    Debug.Log(toDebug + "With Treshold = " + threshold);
                }
                else if (toDebug + threshold > pos.Value.x)
                {
                    Debug.Log(toDebug + threshold + "With Treshold = " + threshold);
                }
            }
            else
            {
                float threshold = 0.5f;
                if (toDebug + threshold > pos.Value.x)
                {
                    Debug.Log(toDebug + "With Treshold = " + threshold);
                }
                else if (toDebug + threshold < pos.Value.x)
                {
                    Debug.Log(toDebug + threshold + "With Treshold = " + threshold);
                }
            }
            
            


        }).Run();
        return default;
    }
}
