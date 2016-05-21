using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveTest
{
    [DataContract]
    class FollowBehaviour : Behavior
    {
        [RequiredComponent]
        public Transform3D _transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Tranform3D" })]
        public string EntityPath { get; set; }

        protected override void Update(TimeSpan gameTime)
        {

        }
    }
}
