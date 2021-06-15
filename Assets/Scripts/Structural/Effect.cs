using System;
using UnityEngine;

namespace Structural {
    public class Effect {
        private float initTime;
        private readonly float maxDuration;
        private readonly Action enactFnc;
        private readonly Action desistFnc;


        public Effect(float maxDuration, Action enactFnc, Action desistFnc) {
            this.maxDuration = maxDuration;
            this.enactFnc = enactFnc;
            this.desistFnc = desistFnc;
        }

        public void StartTime() {
            initTime = Time.time;
        }

        public float Progress() {
            return (Time.time - initTime) / maxDuration;
        }

        public void Enact() {
            enactFnc.Invoke();
        }

        public void Desist() {
            desistFnc.Invoke();
        }
    }
}