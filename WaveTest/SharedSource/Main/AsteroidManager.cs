using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace WaveTest {
    [DataContract]
    class AsteroidManager : Behavior {
        [DataMember]
        public int NumberOfAsteroids { get; set; }

        [DataMember]
        public float AsteroidInterval { get; set; }

        [DataMember]
        [RenderPropertyAsEntity(new string[] {"WaveEngine.Framework.Physics3D.SphereCollider3D"})]
        public string ShipPath { get; set; }

        [DataMember]
        [RenderPropertyAsEntity(new string[] {"WaveEngine.Components.Particles.ParticleSystem3D"})]
        public string ExplosionPath { get; set; }

        [DataMember]
        public float AsteroidDistance { get; set; }

        [DataMember]
        public float AsteroidSpread { get; set; }

        private bool isSpawned = false;
        private int asteroidIndex = 0;
        private const float GameOverTime = 3;
        private const float appearDistance = 1000;

        private bool isGameOver;
        private float remainingAsteroidTime;
        private float remainingGameOverTime;
        private List<Entity> asteroids;
        private Entity shipEntity;
        private Entity explosionEntity;

        protected override void Initialize() {
            base.Initialize();

            this.remainingAsteroidTime = this.AsteroidInterval;

            if (!string.IsNullOrEmpty(this.ShipPath)) {
                this.shipEntity = this.EntityManager.Find(this.ShipPath);
            }

            if (!string.IsNullOrEmpty(this.ExplosionPath)) {
                this.explosionEntity = this.EntityManager.Find(this.ExplosionPath);
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!this.isSpawned) {
                this.CreateAsteroid();
                this.isSpawned = true;
                return;
            }

            if ((this.shipEntity == null) && (this.NumberOfAsteroids == 0)) {
                return;
            }

            if (this.isGameOver) {
                this.remainingGameOverTime -= (float)gameTime.TotalSeconds;
                if (this.remainingGameOverTime <= 0) {
                    this.Reset();
                }
            }

            else {
                var shipCollider = this.shipEntity.FindComponent<SphereCollider3D>().BoundingSphere;
                foreach (var asteroid in this.asteroids)
                {
                    var asteroidCollider = asteroid.FindComponent<SphereCollider3D>();
                    if (asteroidCollider.Intersects(ref shipCollider))
                    {
                        this.GameOver();
                    }
                }
                this.remainingAsteroidTime -= (float)gameTime.TotalSeconds;
                if (this.remainingAsteroidTime < 0) {
                    this.ShowAsteroid();
                    this.remainingAsteroidTime += this.AsteroidInterval;
                }
            }   
        }

        public AsteroidManager() { }

        private void CreateAsteroid() {
            this.asteroids = new List<Entity>();
            for(int i = 0; i < this.NumberOfAsteroids; i++) {
                var asteroid = this.CreateAsteroid(i);
                this.asteroids.Add(asteroid);
                this.EntityManager.Add(asteroid);
            }
            this.asteroidIndex = 0;
        }

        private Entity CreateAsteroid(int i) {
            string model;

            switch (i % this.NumberOfAsteroids) {
                case 0:
                    model = WaveContent.Assets.Models.asteroid_1_0_fbx;
                    break;
                case 1:
                    model = WaveContent.Assets.Models.asteroid_2_0_fbx;
                    break;
                case 2:
                    model = WaveContent.Assets.Models.asteroid_3_0_fbx;
                    break;
                default:
                    model = WaveContent.Assets.Models.asteroid_4_0_fbx;
                    break;
            }

            var asteroid = new Entity("asteroid" + i)
                .AddComponent(new Transform3D())
                .AddComponent(new Model(model))
                .AddComponent(new MaterialsMap() { DefaultMaterialPath = WaveContent.Assets.Materials.AsteroidMat })
                .AddComponent(new Spinner())
                .AddComponent(new SphereCollider3D())
                .AddComponent(new ModelRenderer())
                .AddComponent(new SpawnBehaviour());

            asteroid.IsVisible = false;
            return asteroid;
        }

        private void ShowAsteroid() {
            var asteroid = this.asteroids[this.asteroidIndex];
            asteroid.IsVisible = true;

            var shipTransform = this.shipEntity.FindComponent<Transform3D>();

            var spawnPosition = shipTransform.Position + (shipTransform.WorldTransform.Forward * this.AsteroidDistance);
            spawnPosition.X += WaveServices.Random.Next((int)-this.AsteroidSpread, (int)this.AsteroidSpread);
            spawnPosition.Y += WaveServices.Random.Next((int)-this.AsteroidSpread, (int)this.AsteroidSpread);

            var transform = asteroid.FindComponent<Transform3D>();
            transform.Position = spawnPosition;
            transform.Scale = new Vector3(WaveServices.Random.Next(30, 80));

            var spinner = asteroid.FindComponent<Spinner>();
            spinner.IncreaseX = WaveServices.Random.Next(-100, 100);
            spinner.IncreaseY = WaveServices.Random.Next(-100, 100);
            spinner.IncreaseZ = WaveServices.Random.Next(-100, 100);

            asteroid.FindComponent<SpawnBehaviour>().Spawn();

            this.asteroidIndex = (this.asteroidIndex + 1) % this.NumberOfAsteroids;
        }

        private void GameOver() {
            this.isGameOver = true;
            this.shipEntity.FindComponent<ShipBehaviour>().GameOver();
            this.explosionEntity.FindComponent<Transform3D>().Position = this.shipEntity.FindComponent<Transform3D>().Position;
            this.explosionEntity.FindComponent<ParticleSystem3D>().Emit = true;
            this.explosionEntity.FindChild("ExplosionSound").FindComponent<SoundEmitter3D>().Play();
            this.remainingGameOverTime = 3f;
        }

        private void Reset() {
            foreach (var asteroid in this.asteroids) {
                asteroid.IsVisible = false;
            }
            this.shipEntity.FindComponent<ShipBehaviour>().Reset();
            this.explosionEntity.FindComponent<ParticleSystem3D>().Emit = false;
            this.isGameOver = false;
        }
    }
}
