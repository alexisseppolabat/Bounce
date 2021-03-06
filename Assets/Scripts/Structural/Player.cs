using System;
using System.Collections.Generic;
using UnityEngine;
using static Managers.PlayerMovementManager;
using static Structural.Effects;

namespace Structural {
    public static class Player {
        private const float FlyTime = 12f;
        private const float SpeedTime = 12f; // TODO CHANGE THIS
        private const int FastSpeed = 1000;

    
        public static int score;
        public static int lives = 3;
        public const int Big = 3, Small = 2;
        public static int speed;
        public const int NormalSpeed = 600;
        public static Vector3 spawnPoint;
        public static bool limitedLives = true;
        public static readonly Dictionary<Effects, Effect> Effects = new Dictionary<Effects, Effect>();
        public static readonly FixedSizeSet<Effects> ActiveEffects = new FixedSizeSet<Effects>(Enum.GetNames(typeof(Effects)).Length);

        public static void Init() {
            lives = 3;
            score = 0;
            speed = NormalSpeed;

            InitEffects();
        }

        /**
         * Initialise the activeEffects hashmap by mapping each effect from the Effects enum
         * to an Effect object which encapsulates the functionality of an effect. This functionality
         * includes the enacting function (to apply the effect to the player), the desist function
         * (to remove the effect from the player) and a flag to indicate whether that effect is active
         */
        private static void InitEffects() {
            if (Effects.Count != 0) return;
            // TODO Change the fly mechanic from changing gravity to just adding a vertical force of 2*GRAVITY
            Effects.Add(Fly, new Effect(FlyTime,
                () => {
                    if (Physics.gravity.y == -Gravity) {
                        Physics.gravity = new Vector3(0, Gravity, 0);
                    }
                },
                () => {
                    if (Physics.gravity.y == Gravity) {
                        Physics.gravity = new Vector3(0, -Gravity, 0);
                    }
                }
            ));
            Effects.Add(FlyPress, new Effect(FlyTime,
                () => {
                    if (Input.GetKey("space")) {
                        if (Physics.gravity.y == -Gravity) {
                            Physics.gravity = new Vector3(0, Gravity, 0);
                        }
                    } else if (Physics.gravity.y == Gravity && !ActiveEffects.Contains(Fly)) {
                        // Only stop the ball from flying if it doesn't have the flying effect active
                        Physics.gravity = new Vector3(0, -Gravity, 0);
                    }
                },
                () => {
                    if (Physics.gravity.y == Gravity) {
                        Physics.gravity = new Vector3(0, -Gravity, 0);
                    }
                }
            ));
            Effects.Add(Speed, new Effect(SpeedTime,
                () => speed = FastSpeed,
                () => speed = NormalSpeed
            ));
        }

        public static void RemoveActiveEffects() {
            foreach (Effects effect in ActiveEffects) {
                Effects[effect].Desist();
            }

            ActiveEffects.ExceptWith(ActiveEffects);
        }
    }
}