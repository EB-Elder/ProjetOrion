using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
public class GrenadeMovementSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        float deltaTime = Time.DeltaTime;

        // Ces variable vont servire à faire bouger nos sphère en cercles


        // requête pour accéder à toutes les entities qui ont un component translation, rotation et MovementData
        // La différence entre le mot clef ref et le mot clef in concerne la manière dont on accède à ces component
        // Ref on a droit à la lecture et à l'écriture
        // In veut dire qu'on ne peut que le lire.
        // On fait bouger les sphères de haut en bas
        Entities.ForEach((ref Translation translation, ref Rotation rotation, ref MovementData movementData) => {


            if (movementData.goingUp)
            {
                translation.Value.y += movementData.movementSpeed * deltaTime;

                if ( translation.Value.y >= movementData.startPosition.y + 3)
                {
                    movementData.goingUp = false;

                    movementData.goingDown = true;
                }
                
            }

            if (movementData.goingDown)
            {
                translation.Value.y -= movementData.movementSpeed * deltaTime;

                if (translation.Value.y <= movementData.startPosition.y - 3)
                {
                    movementData.goingUp = true;

                    movementData.goingDown = false;
                }
            }
            




        }).Run();

        return default;
    }

}
