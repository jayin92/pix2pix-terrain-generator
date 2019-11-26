using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEditor.Experimental.VFX;

namespace UnityEditor.VFX
{
    class VFXBlockSubgraphContext : VFXContext
    {
        public enum ContextType
        {
            Spawner = VFXContextType.Spawner,
            Init = VFXContextType.Init,
            Update = VFXContextType.Update,
            Output = VFXContextType.Output,

            InitAndUpdate = Init | Update,
            InitAndUpdateAndOutput = Init | Update | Output,
            UpdateAndOutput = Update | Output
        }

        public VFXBlockSubgraphContext():base(VFXContextType.None, VFXDataType.None, VFXDataType.None)
        {
        }
        protected override int inputFlowCount { get { return 0; } }

        public sealed override string name { get { return "Block Subgraph"; } }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties
        {
            get {
                yield break;
            }
        }

        [VFXSetting]
        ContextType m_SuitableContexts = ContextType.InitAndUpdateAndOutput;

        public VFXContextType compatibleContextType
        {
            get
            {
                return (VFXContextType)m_SuitableContexts;
            }
        }
        public override VFXDataType ownedType
        {
            get
            {
                return (m_SuitableContexts == ContextType.Spawner) ? VFXDataType.SpawnEvent : VFXDataType.Particle;
            }
        }
        public override bool spaceable
        {
            get
            {
                return false;
            }
        }

        protected override void OnInvalidate(VFXModel model, InvalidationCause cause)
        {
            base.OnInvalidate(model, cause);

            if (cause == InvalidationCause.kSettingChanged)
            {
                //Delete incompatible blocks

                foreach (var block in children.ToList())
                {
                    if (!Accept(block))
                        RemoveChild(block);
                }
            }
        }
        public override bool Accept(VFXBlock block, int index = -1)
        {
            return ((block.compatibleContexts & compatibleContextType) == compatibleContextType);
        }

        public override bool CanBeCompiled()
        {
            return false;
        }
    }
}
