﻿using Domain.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Model.Creature
{
    public class CreatureGenerator : MonoBehaviour, IGenerator
    {
        private const int MAX_CREATURE = 2;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject creaturePrefab;

        private List<GameObject> creatures;

        private GameObject GetGameZone(List<GameObject> gameObjects)
        {
            foreach (var item in gameObjects)
                if (item.transform.tag == "GameZone") return item;
            return null;
        }

        // иниц. существ (распределение ролей)
        private void InitCreatures()
        {
            creatures[0].tag = "Player";
            creatures[0].name = "Player";
            creatures[0].layer = 8; // FireDirection
            var playerBehavior = creatures[0].AddComponent<PlayerBehavior>();
            playerBehavior.mainCamera = Camera.main;

            for (int i = 1; i < creatures.Count; i++)
            {
                creatures[i].name = "Creature_" + i;
                creatures[i].tag = "Bot";
                var botBehavior = creatures[i].AddComponent<BotBehavior>();
                if (botBehavior != null) BotBehavior.player = creatures[0].transform;
            }
        }

        public void Generation() => Generation(null);

        public void Generation(List<GameObject> gameObjects)
        {
            if (creaturePrefab != null && bulletPrefab != null && Camera.main != null && gameObjects.Count > 0)
            {
                var zone = GetGameZone(gameObjects).transform;
                creatures = new List<GameObject>();
                if (zone != null)
                {
                    var xMin = new Vector3(zone.lossyScale.x, zone.lossyScale.y) / -2.1f;
                    var xMax = new Vector3(-zone.lossyScale.x, zone.lossyScale.y) / -2.1f;
                    var yMin = new Vector3(zone.lossyScale.x, zone.lossyScale.y) / -2.1f;
                    var yMax = new Vector3(zone.lossyScale.x, -zone.lossyScale.y) / -2.1f;
                    // спавн сущностей
                    for (int i = 0; i < MAX_CREATURE; i++)
                    {
                        creatures.Add(Instantiate(creaturePrefab, new Vector3(Random.Range(xMin.x, xMax.x), Random.Range(yMin.y, yMax.y), -1), creaturePrefab.transform.rotation));
                        var creatureController = creatures[creatures.Count - 1].AddComponent<CreatureController>();
                        creatureController.StartPosition = creatures[creatures.Count - 1].transform.position;
                        creatureController.SetBullet(bulletPrefab);
                    }
                    InitCreatures();
                }
            }
        }

        public void DestroyGenerator() => Destroy(this);

        public List<GameObject> GetCreatedObjects()
        {
            return creatures;
        }
    }
}