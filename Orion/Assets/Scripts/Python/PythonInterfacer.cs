using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using IronPython.Modules;
using Microsoft.Scripting.Utils;
using Unity.Entities;
using UnityEngine;

namespace Python
{
    public class PythonInterfacer : MonoBehaviour
    {
        private dynamic IA;
        private float timeCounter;
        private EntityManager _eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        public Transform playerTransform;
        public Transform foodTransform;
        public Transform enemyTransform;
        public Vector3 startPlayer;
        public Vector3 startFood;
        public Vector3 startEnemy;
        public int lastStepX = 0;
        public int lastStepY = 0;
        
            void Start () {
                
                
            var engine = IronPython.Hosting.Python.CreateEngine();

            ICollection<string> searchPaths = engine.GetSearchPaths();

            
            searchPaths.Add(@"F:\Projet Orion\Python\QLearning\");
            //Path to the Python standard library
            searchPaths.Add(@"F:\Projet Orion\ProjetOrion\Orion\Assets\Plugins\Python\Lib");
            searchPaths.Add(@"F:\Projet Orion\ProjetOrion\Orion\Assets\Plugins\Python\Lib\site-packages");
            engine.SetSearchPaths(searchPaths);

            dynamic py = engine.ExecuteFile(@"F:\Projet Orion\Python\QLearning\IA.py");
            IA = py.IA();

            playerTransform.position = startPlayer;
            foodTransform.position = startFood;
            enemyTransform.position = startEnemy;

            }

        private void FixedUpdate()
        {
            timeCounter += Time.deltaTime;
            if(!(timeCounter >= 0.5f))
                return;
            timeCounter = .0f;
            Vector3 clampedPosition = playerTransform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0, 9);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0, 9);
            

            playerTransform.position = clampedPosition;
            Vector3 currentPlayerPosition = playerTransform.position;
            
            int playerX = (int) currentPlayerPosition.x;
            int playerY = (int) currentPlayerPosition.y;
            
            if (playerX == startFood.x && playerY == startFood.y)
                return;
            if (playerX >= startFood.x - 2f && playerY >= startFood.y - 2f &&
                playerX <= startFood.x && playerY <= startFood.y)
            {
                playerTransform.Translate(new Vector3(-1, -1, 0));
                return;
            }
            if(playerX <= startFood.x + 2f && playerY <= startFood.y + 2f &&
               playerX >= startFood.x && playerY >= startFood.y)
            {
                playerTransform.Translate(new Vector3(1, 1, 0));
                return;
            }
            if(playerX >= startFood.x - 2f && playerY <= startFood.y + 2f &&
               playerX <= startFood.x && playerY >= startFood.y)
            {
                playerTransform.Translate(new Vector3(-1, 1, 0));
                return;
            }
            if(playerX <= startFood.x + 2f && playerY >= startFood.y - 2f &&
               playerX >= startFood.x + 2f && playerY <= startFood.y - 2f)
            {
                playerTransform.Translate(new Vector3(1, -1, 0));
                return;
            }
            Debug.Log("Hi !");
            string resp = IA.getAction(startFood.x, startFood.y, playerX, playerY , startEnemy.x, startEnemy.y);
            string[] lines = Regex.Split(resp, ",");
            
            int movementX;
            int movementY;
            int.TryParse(lines[0], out movementX);
            int.TryParse(lines[1], out movementY);
                        
            
            playerTransform.Translate(new Vector3(movementX, movementY, 0));

            

        }

        
    }
}