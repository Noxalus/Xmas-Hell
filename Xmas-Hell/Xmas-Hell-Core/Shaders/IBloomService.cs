using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Shaders
{
    public interface IBloomService
    {

        /// <summary>
        /// The render target the game should draw itself on to be blurred.
        /// </summary>
        RenderTarget2D RenderTarget { get; }

        /// <summary>
        /// The render target the bloom filter should use (null if it should render to the backbuffer).
        /// </summary>
        RenderTarget2D FinalRenderTarget { get; set; }

        bool BloomEnabled { get; set; }

        /// <summary>
        /// Power and range of the gaussian blur filter. Use small fractions in order to avoid artifacts.
        /// </summary>
        float BlurPower { get; set; }

        /// <summary>
        /// Intensity multiplier of the original render.
        /// </summary>
        float BaseIntensity { get; set; }

        /// <summary>
        /// Intensity multiplier of the bloomed image.
        /// </summary>
        float BloomIntensity { get; set; }

        /// <summary>
        /// Saturation multiplier of the original render.
        /// </summary>
        float BaseSaturation { get; set; }

        /// <summary>
        /// Saturation multiplier of the bloomed image.
        /// </summary>
        float BloomSaturation { get; set; }

        /// <summary>
        /// Intensity threshold for pixels that should be copied to the bloomed image. 0 takes all pixels, 1 none.
        /// </summary>
        float BloomThreshold { get; set; }
    }
}
