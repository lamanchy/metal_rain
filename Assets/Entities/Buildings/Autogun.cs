using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Wreckage;
using Meteor;
using UnityEngine;

namespace Entities.Buildings {
    public class Autogun : StaticEntity {
        [Header("Autogun specific")]
        public int FireCooldown;
        public int FireCost;

        private GameObject gun;
        private Transform barrelEnd;
        private LineRenderer lineRenderer;
        private bool isReadyToFire = true;
        private const float minEnergyToAttack = 3000;

        private SortedSet<FallingWreckage> wreckageInRange;

        protected override void Start() {
            gun = transform.Find("Gun").gameObject;
            barrelEnd = gun.transform.Find("BarrelEnd");
            lineRenderer = GetComponent<LineRenderer>();
            wreckageInRange = new SortedSet<FallingWreckage>(new WreckageSizeComparer());
            base.Start();
            lineRenderer.enabled = false;
            FallingWreckage.OnWreckageFallen += OnWreckageFallen;
        }

        private void OnDestroy() {
            FallingWreckage.OnWreckageFallen -= OnWreckageFallen;
        }

        private void OnTriggerEnter(Collider other) {
            var fallingWreckage = other.GetComponent<FallingWreckage>();
            if (fallingWreckage == null) {
                return;
            }

            wreckageInRange.Add(fallingWreckage);
        }

        private void OnTriggerExit(Collider other) {
            var fallingWreckage = other.GetComponent<FallingWreckage>();
            if (fallingWreckage == null) {
                return;
            }

            wreckageInRange.Remove(fallingWreckage);
        }
            
        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (!IsPowered) {
                return;
            }

            // Clear falling wreckage
            while (wreckageInRange.Count > 0 && (wreckageInRange.Min == null ||
                !Physics.Linecast(gun.transform.position, wreckageInRange.Min.transform.position))) {
                wreckageInRange.Remove(wreckageInRange.Min);
            }

            // Extract from nearest wreckage
            if (wreckageInRange.Count > 0
                && wreckageInRange.Min != null
                && wreckageInRange.Min.Energy >= minEnergyToAttack) {
                gun.transform.LookAt(wreckageInRange.Min.transform);
                Fire();
            }
        }

        private void Fire() {
            if (!isReadyToFire || Energy < FireCost) {
                return;
            }
            Energy -= FireCost;

            var nearestWreckage = wreckageInRange.Min;
            foreach (var newWreckage in nearestWreckage.Split()) {
                wreckageInRange.Add(newWreckage);
            }

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, barrelEnd.position);
            lineRenderer.SetPosition(1, nearestWreckage.transform.position);

            wreckageInRange.Remove(nearestWreckage);

            isReadyToFire = false;
            StartCoroutine(FireEffectTimer());
            StartCoroutine(FireCooldownTimer());
        }

        private IEnumerator FireEffectTimer() {
            for (var alpha = 1f; alpha > 0; alpha -= 0.01f) {
                lineRenderer.startColor = new Color(1, 1, 1, alpha);
                yield return null;
            }
            lineRenderer.enabled = false;
        }

        private IEnumerator FireCooldownTimer() {
            yield return new WaitForSeconds(FireCooldown);
            isReadyToFire = true;
        }

        public void OnWreckageFallen(FallenWreckage _, FallingWreckage wreckage) {
            wreckageInRange.Remove(wreckage);
        }
    }
}