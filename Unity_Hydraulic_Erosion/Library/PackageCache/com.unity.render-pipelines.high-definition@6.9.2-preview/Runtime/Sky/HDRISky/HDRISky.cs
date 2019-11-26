using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [VolumeComponentMenu("Sky/HDRI Sky")]
    [SkyUniqueID((int)SkyType.HDRISky)]
    public class HDRISky : SkySettings
    {
        [Tooltip("Specify the cubemap HDRP uses to render the sky.")]
        public CubemapParameter hdriSky = new CubemapParameter(null);

        public override SkyRenderer CreateRenderer()
        {
            return new HDRISkyRenderer(this);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();

            unchecked
            {
                hash = hdriSky.value != null ? hash * 23 + hdriSky.GetHashCode() : hash;
            }

            return hash;
        }
    }
}
