using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using IronPython.Modules;
using Microsoft.Scripting.Utils;
using UnityEngine;

public class BossMovement : JobComponentSystem
{
    private bool done = false;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        if (!done)
        {
            done = true;
        }
            
        return default;
    }
}
