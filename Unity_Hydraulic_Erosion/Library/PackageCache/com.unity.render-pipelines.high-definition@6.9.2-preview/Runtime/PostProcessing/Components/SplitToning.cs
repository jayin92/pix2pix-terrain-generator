using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [Serializable, VolumeComponentMenu("Post-processing/Split Toning")]
    public sealed class SplitToning : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Specifies the color to use for shadows.")]
        public ColorParameter shadows = new ColorParameter(Color.grey, false, false, true);
        
        [Tooltip("Specifies the color to use for highlights.")]
        public ColorParameter highlights = new ColorParameter(Color.grey, false, false, true);

        [Tooltip("Controls the balance between the colors in the highlights and shadows.")]
        public ClampedFloatParameter balance = new ClampedFloatParameter(0f, -100f, 100f);

        public bool IsActive()
        {
            return shadows != Color.grey
                || highlights != Color.grey;
        }
    }
}
