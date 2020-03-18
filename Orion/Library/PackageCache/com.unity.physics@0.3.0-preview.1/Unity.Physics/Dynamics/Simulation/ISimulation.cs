﻿using System;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Physics
{
    // Implementations of ISimulation
    public enum SimulationType
    {
        NoPhysics,                    // A dummy implementation which does nothing
        UnityPhysics,                 // Default C# implementation
        HavokPhysics                  // Havok implementation (using C++ plugin)
    }

    // Parameters for a simulation step
    public struct SimulationStepInput
    {
        public PhysicsWorld World; // Physics world to be stepped
        public float TimeStep; // Portion of time to step the physics world for
        public float3 Gravity; // Gravity in the physics world
        public int NumSolverIterations; // Number of iterations to perform while solving constraints
        public bool SynchronizeCollisionWorld; // Whether to update the collision world after the step for more precise queries

        [Obsolete("ThreadCountHint has been deprecated. Provide it directly to the ScheduleStepJobs method. (RemovedAfter 2020-05-01)")]
        public int ThreadCountHint;
    }

    // Result of ISimulation.ScheduleStepJobs()
    public struct SimulationJobHandles
    {
        public JobHandle FinalExecutionHandle;
        public JobHandle FinalDisposeHandle;

        public SimulationJobHandles(JobHandle handle)
        {
            FinalExecutionHandle = handle;
            FinalDisposeHandle = handle;
        }
    }

    // Interface for simulations
    public interface ISimulation : IDisposable
    {
        // The implementation type.
        SimulationType Type { get; }

        // Step the simulation.
        void Step(SimulationStepInput input);

        // Schedule a set of jobs to step the simulation.
        SimulationJobHandles ScheduleStepJobs(SimulationStepInput input, SimulationCallbacks callbacks, JobHandle inputDeps, int threadCountHint = 0);

        // Schedule a set of jobs to step the simulation.
        [Obsolete("ScheduleStepJobs() has been deprecated. Use the new ScheduleStepJobs method that takes callbacks and threadCountHint as input and returns SimulationStepHandles. (RemovedAfter 2020-05-01)")]
        void ScheduleStepJobs(SimulationStepInput input, JobHandle inputDeps);

        // The final scheduled simulation job.
        // Jobs which use the simulation results should depend on this.
        JobHandle FinalSimulationJobHandle { get; }

        // The final scheduled job, including all simulation and cleanup.
        // The end of each step should depend on this.
        JobHandle FinalJobHandle { get; }
    }
}
