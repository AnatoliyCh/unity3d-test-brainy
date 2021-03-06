﻿using Business.ServiceMethods;
using Domain.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Model.LevelGeneration
{
    [DisallowMultipleComponent]
    public class LevelGenerator : MonoBehaviour, IGenerator
    {
        [SerializeField] private GameObject squarePrefab; // базовый блок для генерации
        [SerializeField] private GameObject triggerPrefab; // триггер для генерации препятствий

        [SerializeField] private Vector2Int sizeZone; // размер игрового поля
        [SerializeField] private Color colorZone;

        [SerializeField] private int maxNumberObstacles; // макс. кол-во препятствий
        [SerializeField] private Color[] colorsObstacles; // цвета препятствий

        private List<GameObject> createdObjects = new List<GameObject>();

        // триггер для ограничения зоны
        private void TriggerZone(Transform zone)
        {
            var trigger = Instantiate(triggerPrefab, squarePrefab.transform.position, squarePrefab.transform.rotation);
            var collider2D = trigger.GetComponent<Collider2D>();
            if (collider2D != null)
            {
                collider2D.isTrigger = true;
                createdObjects.Add(trigger);
                trigger.transform.localScale = zone.lossyScale;
                trigger.name = "Trigger";
                trigger.transform.tag = "EndZone";
                trigger.layer = 2; // ignoreRay
                trigger.transform.SetParent(zone);
            }
            else Destroy(trigger);
        }

        // генерация препятствий
        private void GenerationObstacles(Transform parent)
        {
            var trigger = Instantiate(triggerPrefab);
            var collision = false; // обнаружение столкновений
            for (int i = 0; i < maxNumberObstacles; i++)
            {
                // относительно размера игрового поля, +1 для проходов
                trigger.transform.localScale = new Vector2(Random.Range(1.0f, ServiceMethods.PercentNumber(10, sizeZone.x)) + 1, Random.Range(1.0f, ServiceMethods.PercentNumber(20, sizeZone.y)) + 1);
                trigger.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 90));
                // размер относительно игрового поля с отступами т.е: (+-ширинаХ / 2) +- (ширинаБлокаХ / 2 (т.к pivot в центре)) +- ширинаБлокаХ / 4 (чтобы при вращении не выходить за края)
                trigger.transform.localPosition = new Vector3(
                    Random.Range(-sizeZone.x / 2 + trigger.transform.localScale.x / 2 + trigger.transform.localScale.x / 4, sizeZone.x / 2 - trigger.transform.localScale.x / 2 - trigger.transform.localScale.x / 4),
                    Random.Range(-sizeZone.y / 2 + trigger.transform.localScale.y / 2 + trigger.transform.localScale.y / 4, sizeZone.y / 2 - trigger.transform.localScale.y / 2 - trigger.transform.localScale.y / 4), -1);
                // расчет столкновения
                for (int j = 0; j < createdObjects.Count; j++)
                    if (ServiceMethods.IsСollision2D(createdObjects[j].transform, trigger.transform))
                    {
                        collision = true;
                        break;
                    }
                // создание объекта
                if (!collision)
                {
                    var newObstacle = Instantiate(squarePrefab, trigger.transform.position, trigger.transform.rotation);
                    newObstacle.transform.localScale = new Vector2(trigger.transform.localScale.x - 1, trigger.transform.localScale.y - 1);
                    if (colorsObstacles.Length > 0) newObstacle.GetComponent<SpriteRenderer>().color = colorsObstacles[Random.Range(0, colorsObstacles.Length)];
                    createdObjects.Add(newObstacle);
                    newObstacle.tag = "Obstacle";
                    newObstacle.name = "Obstacle_" + createdObjects.Count;
                    newObstacle.layer = 8; // FireDirection
                }
                collision = false;
            }
            // если есть препятствия
            if (createdObjects.Count > 0)
                foreach (var item in createdObjects)
                    item.transform.SetParent(parent);
            Destroy(trigger); // удаляем лишний триггер
        }

        public void Generation()
        {
            if (squarePrefab != null && triggerPrefab != null)
            {
                var zone = Instantiate(squarePrefab);
                zone.transform.position = Vector2.zero;
                zone.transform.localScale = new Vector3(sizeZone.x, sizeZone.y);
                zone.name = "Zone";
                zone.transform.tag = "GameZone";
                var spriteRenderer = zone.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) spriteRenderer.color = colorZone;
                Destroy(zone.GetComponent<Collider2D>());

                if (sizeZone.x > 0 && sizeZone.y > 0)
                {
                    GenerationObstacles(zone.transform);
                    TriggerZone(zone.transform);
                }

                createdObjects.Add(zone);
            }
        }

        public void Generation(List<GameObject> gameObjects) => Generation();

        public void DestroyGenerator() => Destroy(this);

        public List<GameObject> GetCreatedObjects()
        {
            return createdObjects;
        }
    }
}