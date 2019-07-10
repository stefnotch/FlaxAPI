using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlaxEngine
{
    /// <summary>
    /// My custom graph
    /// Does *not* inherit from JsonAsset!
    /// </summary>
    [Serializable]
    public sealed class NumberGraph
    {
        private float _accumulatedTime = 0;
        private const float UpdatesPerSecond = 3;
        private const float UpdateDuration = 1f / UpdatesPerSecond;

        [Serialize]
        private NumberGraphDefinition _numberGraphDefinition;

        public static NumberGraph New()
        {
            return new NumberGraph();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NumberGraph()
        {

        }

        [NoSerialize]
        internal NumberGraphDefinition NumberGraphDefinition
        {
            get { return _numberGraphDefinition; }
            set { _numberGraphDefinition = value; }
        }

        [Serialize]
        public string GraphName { get; set; } = "Neko 42";

        [NoSerialize]
        public float OutputFloat => NumberGraphDefinition?.OutputFloat ?? default(float);

        [NoSerialize]
        public Vector2 OutputVector2 => NumberGraphDefinition?.OutputVector2 ?? default(Vector2);

        [NoSerialize]
        public Vector3 OutputVector3 => NumberGraphDefinition?.OutputVector3 ?? default(Vector3);

        public void Update(float deltaTime)
        {
            _accumulatedTime += deltaTime;
            if (_accumulatedTime < UpdateDuration) return;

            _accumulatedTime = 0;
            NumberGraphDefinition?.Update();
        }
    }
}
