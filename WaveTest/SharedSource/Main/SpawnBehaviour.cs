using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveTest {

    [DataContract]
    class SpawnBehaviour : Behavior{
        [RequiredComponent]
        public Transform3D transform;

        private Vector3 endScale;
        private bool isSpawning;

        protected override void Update(TimeSpan gameTime) {
            if (this.isSpawning) {
                var scale = this.transform.Scale;
                scale = Vector3.Lerp(this.transform.Scale, this.endScale, 0.005f);
                this.transform.Scale = scale;
                if (scale.X > this.endScale.X) {
                    this.isSpawning = false;
                }
            }
        }

        public void Spawn() {
            this.endScale = this.transform.Scale;
            this.transform.Scale = Vector3.Zero;
            this.isSpawning = true;
        }
    }
}
