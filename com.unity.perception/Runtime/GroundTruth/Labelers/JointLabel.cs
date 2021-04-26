using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.Perception.GroundTruth
{
    public enum SelfOcclusionDistanceSource
    {
        JointLabel,
        KeypointLabeler
    }
    /// <summary>
    /// Label to designate a custom joint/keypoint. These are needed to add body
    /// parts to a humanoid model that are not contained in its <see cref="Animator"/> <see cref="Avatar"/>
    /// </summary>
    [AddComponentMenu("Perception/Labeling/Joint Label")]
    [Serializable]
    public class JointLabel : MonoBehaviour, ISerializationCallbackReceiver
    {
        private static PerceptionCamera singlePerceptionCamera = null;

        /// <summary>
        /// Maps this joint to a joint in a <see cref="KeypointTemplate"/>
        /// </summary>
        [Serializable]
        class TemplateData
        {
            /// <summary>
            /// The name of the joint.
            /// </summary>
            public string label;
        };

        /// <summary>
        /// List of all of the templates that this joint can be mapped to.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<TemplateData> templateInformation;

        /// <summary>
        /// List of all of the templates that this joint can be mapped to.
        /// </summary>
        [SerializeField]
        public List<string> labels = new List<string>();

        public SelfOcclusionDistanceSource selfOcclusionDistanceSource = SelfOcclusionDistanceSource.JointLabel;
        public float selfOcclusionDistance = .15f;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (templateInformation != null)
            {
                foreach (var data in templateInformation)
                {
                    labels.Add(data.label);
                }

                templateInformation = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Packages/com.unity.perception/Editor/Icons/Keypoint.png", false);
        }
        private void OnDrawGizmosSelected()
        {
            if (singlePerceptionCamera == null)
            {
                singlePerceptionCamera = FindObjectOfType<PerceptionCamera>();
            }
            float occlusionDistance;
            switch (selfOcclusionDistanceSource)
            {
                case SelfOcclusionDistanceSource.JointLabel:
                    occlusionDistance = selfOcclusionDistance;
                    break;
                case SelfOcclusionDistanceSource.KeypointLabeler:
                    if (singlePerceptionCamera == null)
                    {
                        occlusionDistance = KeypointLabeler.defaultSelfOcclusionDistance;
                    }
                    else
                    {
                        var keypointLabeler = (KeypointLabeler) singlePerceptionCamera.labelers.FirstOrDefault(l => l is KeypointLabeler);
                        if (keypointLabeler == null)
                            occlusionDistance = KeypointLabeler.defaultSelfOcclusionDistance;
                        else
                            occlusionDistance = keypointLabeler.selfOcclusionDistance;
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid SelfOcclusionDistanceSource");
            }

            Gizmos.color = /*Color.green;*/new Color(1, 1, 1, .5f);
            Gizmos.DrawWireSphere(transform.position, occlusionDistance);
        }
    }
}
