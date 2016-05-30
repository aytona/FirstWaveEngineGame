using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveTest {
    [DataContract]
    class FollowBehaviour : Behavior {
        [RequiredComponent]
        public Transform3D _transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Tranform3D" })]
        public string EntityPath { get; set; }

        [DataMember]
        public bool SmoothFollow { get; set; }

        [DataMember]
        public bool RotatingFollow { get; set; }

        private Transform3D _target;

        protected override void Initialize() {
            base.Initialize();

            if (string.IsNullOrEmpty(this.EntityPath)) {
                return;
            }

            var entity = this.EntityManager.Find(this.EntityPath);
            this._target = entity.FindComponent<Transform3D>();
        }

        protected override void Update(TimeSpan gameTime) {
            if (this._target == null) {
                return;
            }

            if (this.SmoothFollow) {
                var lerp = Math.Min(1, 10 * (float)gameTime.TotalSeconds);
                this._transform.Position = Vector3.Lerp(this._transform.Position, this._target.Position, lerp);

                if (this.RotatingFollow) {
                    this._transform.Rotation = Vector3.Lerp(this._transform.Rotation, this._target.Rotation, lerp);
                }
            }

            else {
                this._transform.Position = this._target.Position;
                if (this.RotatingFollow) {
                    this._transform.Rotation = this._target.Rotation;
                }
            }
        }
    }
}
