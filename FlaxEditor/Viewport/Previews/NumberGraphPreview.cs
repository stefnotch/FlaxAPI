using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

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
            _numberGraph = new NumberGraph();
        }

        public JsonAsset Asset
        {
            get { return _asset; }
            set
            {
                if (_asset != value)
                {
                    _asset = value;
                    // TODO: 
                    //_system.Init(value, _playbackDuration);
                    //_numberGraph.ResetSimulation();
                }
            }
        }

    }
}
