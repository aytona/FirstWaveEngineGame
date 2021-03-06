﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Sound;

namespace WaveTest {
    [DataContract]
    public class ShipBehaviour : Behavior {
        [RequiredComponent]
        public Transform3D _transform;

        [DataMember]
        public float Speed { get; set; }

        private float currentSpeed;

        protected override void Initialize() {
            base.Initialize();
            this.currentSpeed = this.Speed;
        }

        protected override void Update(TimeSpan gameTime) {
            var rotation = Vector3.Zero;
            var input = WaveServices.Input.KeyboardState;
            var localPosition = this._transform.LocalPosition;

            if (input.W == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y -= (float)gameTime.TotalSeconds;
            }

            if (input.S == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y += (float)gameTime.TotalSeconds;
            }

            if (input.A == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X += (float)gameTime.TotalSeconds;
            }

            if (input.D == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X -= (float)gameTime.TotalSeconds;
            }

            this._transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
            localPosition.Z -= (float)(this.currentSpeed * gameTime.TotalSeconds);
            this._transform.LocalPosition = localPosition;
        }

        public void Reset() {
            this.Owner.IsVisible = true;
            this._transform.Position = Vector3.Zero;
            this._transform.Rotation = Vector3.Zero;
            this.currentSpeed = this.Speed;

            this.Owner.FindChild("EngineSound").FindComponent<SoundEmitter3D>().Volume = 1;
        }

        public void GameOver() {
            this.Owner.IsVisible = false;
            this.currentSpeed = 0;


            this.Owner.FindChild("EngineSound").FindComponent<SoundEmitter3D>().Volume = 0;
        }
    }
}
