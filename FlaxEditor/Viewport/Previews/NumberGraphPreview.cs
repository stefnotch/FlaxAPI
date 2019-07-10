using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.Viewport.Previews
{
    public class NumberGraphPreview : AssetPreview
    {
        private NumberGraph _numberGraph;
        private JsonAsset _asset;

        /// <summary>
        /// Check out <see cref="ParticleSystemPreview"/> and <see cref="ParticleEmitterPreview"/>
        /// </summary>
        /// <param name="useWidgets"></param>
        public NumberGraphPreview(bool useWidgets) : base(useWidgets)
        {
            // FlaxEngine.Content.CreateVirtualAsset
            _numberGraph = NumberGraph.New();
        }

        public JsonAsset Asset
        {
            get { return _asset; }
            set
            {
                if (_asset != value)
                {
                    _asset = value;
                    RefreshAsset();
                    // TODO: 
                    //_system.Init(value, _playbackDuration);
                    //_numberGraph.ResetSimulation();
                }
            }
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Manually update simulation
            _numberGraph?.Update(deltaTime);
        }

        /// <inheritdoc />
        public override void Draw()
        {
            base.Draw();

            if (_numberGraph == null) return;

            Render2D.DrawText(
                Style.Current.FontLarge,
                $"Float: {_numberGraph.OutputFloat}\nVector2: {_numberGraph.OutputVector2}\nVector3: {_numberGraph.OutputVector3}\n",
                new Rectangle(Vector2.Zero, Size),
                Color.Wheat,
                TextAlignment.Near,
                TextAlignment.Far);

        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            _numberGraph = null;
            base.OnDestroy();
        }

        public void RefreshAsset()
        {
            if (_asset)
            {
                _numberGraph = _asset.CreateInstance<NumberGraph>();
            }
            else
            {
                _numberGraph = null;
            }
        }
    }
}
