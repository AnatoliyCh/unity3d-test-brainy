﻿using Domain.Interfaces;
using UnityEngine;

namespace Domain.Model.Creature
{
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        private const int SPEED = 2;

        private GameObject bullet;
        private Transform spawnBullet;

        public Vector3 StartPosition { get; set; }
        public bool Debug { get; set; } = false;

        private void Awake() => spawnBullet = transform.Find("SpawnBullet");

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.tag == "Bullet")
            {
                Destroy(collision.gameObject);
                GameController.Instance?.GameReset(transform.tag);
            }
        }

        public void ResetPosition()
        {
            transform.position = StartPosition;
            var botBehavior = gameObject.GetComponent<BotBehavior>();
            if (botBehavior != null) botBehavior.GetPath();
        }

        public void Movement(Vector3 moveDirection) => transform.position += moveDirection * SPEED * Time.deltaTime;

        public void Rotation(Vector3 rotateDirection) => transform.eulerAngles = rotateDirection;

        public void Shot() => Instantiate(bullet, spawnBullet.position, transform.rotation).GetComponent<Rigidbody2D>().velocity += (Vector2)(transform.right * 30f);

        public void SetBullet(GameObject bullet) => this.bullet = bullet;
    }
}