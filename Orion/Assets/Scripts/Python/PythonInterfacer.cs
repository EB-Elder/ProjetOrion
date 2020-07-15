using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using IronPython.Modules;
using Microsoft.Scripting.Utils;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Python
{
    public class PythonInterfacer : MonoBehaviour
    {
        private dynamic IA;
        private float timeCounter;
        private EntityManager _eManager;
        private Translation _playerTranslation;
        private Translation _bossTranslation;
        private Vector3 startEnemy;
        
            void Start () 
            {
                
                var engine = IronPython.Hosting.Python.CreateEngine();

                ICollection<string> searchPaths = engine.GetSearchPaths();

                
                searchPaths.Add(@".\Assets\Scripts\Python\");
                //Path to the Python standard library
                searchPaths.Add(@".\Assets\Plugins\Python\Lib");
                searchPaths.Add(@".\Assets\Plugins\Python\Lib\site-packages");
                engine.SetSearchPaths(searchPaths);

                dynamic py = engine.ExecuteFile(@".\Assets\Scripts\Python\QLearning\IA.py");
                IA = py.IA();
                

            }

        private void FixedUpdate()
        {
            _eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = _eManager.CreateEntityQuery(ComponentType.ReadOnly<BossStats>());
            var resBoss = query.ToEntityArray(Unity.Collections.Allocator.TempJob);
            if (resBoss.Length > 0)
            {
                _bossTranslation = _eManager.GetComponentData<Translation>(resBoss[0]);
                
            }

            
            
            float bossPosX = _bossTranslation.Value.x;
            float bossPosZ = _bossTranslation.Value.z;
            
            
            query = _eManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerStatsData>());
            var resPlayer = query.ToEntityArray(Unity.Collections.Allocator.TempJob);
            
            if (resPlayer.Length > 0)
            {
                _playerTranslation = _eManager.GetComponentData<Translation>(resPlayer[0]);

            }
            
            float playerPosX = _playerTranslation.Value.x;
            float playerPosZ = _playerTranslation.Value.z;


            timeCounter += Time.deltaTime;
            if (!(timeCounter >= 0.25f))
            {
                resPlayer.Dispose();
                resBoss.Dispose();
                return;
            }
                
            timeCounter = .0f;
            Vector3 clampedPosition = new Vector3(bossPosX, 0, bossPosZ);
            bossPosX = Mathf.Clamp(bossPosX, 0, 9);
            bossPosZ = Mathf.Clamp(bossPosZ, 0, 9);
            
            
            int bossIntPosX = (int) bossPosX;
            int bossIntPosZ = (int) bossPosZ;

            int playerIntPosX = (int) playerPosX;
            int playerIntPosZ = (int) playerPosZ;
            
            
            Debug.Log("Player : " + playerIntPosX + "_____" +playerIntPosZ);
            Debug.Log("Boss : " + bossIntPosX + "_____" +bossIntPosZ);
            
            string resp = IA.getAction(playerIntPosX, playerIntPosZ, bossIntPosX, bossIntPosZ , startEnemy.x, startEnemy.y);
            string[] lines = Regex.Split(resp, ",");
            
            int movementX;
            int movementZ;
            int.TryParse(lines[0], out movementX);
            int.TryParse(lines[1], out movementZ);
                        
            
            _eManager.SetComponentData<Translation>(resBoss[0], new Translation{Value = new Unity.Mathematics.float3(_bossTranslation.Value.x + (movementX),
                _bossTranslation.Value.y,
                _bossTranslation.Value.z + (movementZ))});

            resPlayer.Dispose();
            resBoss.Dispose();
            


        }

        
    }
}