using UnityEngine;
using UnityEngine.Rendering;

namespace Sabresaurus.SabreCSG.Volumes
{
    [RequireComponent( typeof( ReflectionProbe ) )]
    public partial class ReflectionProbeVolumeComponent : MonoBehaviour
    {
        [HideInInspector] public ReflectionProbeMode type = ReflectionProbeMode.Baked;
        [HideInInspector] public ReflectionProbeTimeSlicingMode timeMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
        [HideInInspector] public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.OnAwake;
        [HideInInspector] public int importance = 1;
        [HideInInspector] public float intensity = 1;
        [HideInInspector] public bool boxProjection = false;
        [HideInInspector] public float blendDistance = 1;
        [HideInInspector] public ReflectionCubeResolution resolution = ReflectionCubeResolution._128;
        [HideInInspector] public bool hdr = true;
        [HideInInspector] public float shadowDist = 100;
        [HideInInspector] public ReflectionProbeClearFlags clearFlags = ReflectionProbeClearFlags.Skybox;
        [HideInInspector] public Color background = new Color32( 49, 77, 128, 0 );
        [HideInInspector] public LayerMask cullingMask = ~0;
        [HideInInspector] public Vector2 clippingPlanes = new Vector2( 0.3f, 1000.0f ); //x = near, y = far
       // [HideInInspector] public bool dynamicObjects = false;
       // [HideInInspector] public Cubemap customCube = null;

        private ReflectionProbe probe = null;

        public void Setup()
        {
            Bounds bounds = GetComponent<MeshCollider>().bounds;
            probe = this.AddOrGetComponent<ReflectionProbe>();

            probe.size = bounds.size;
            probe.center = Vector3.zero;
        }

        public void Apply()
        {
            probe.mode = type;
            probe.timeSlicingMode = timeMode;
            probe.refreshMode = refreshMode;
            probe.importance = importance;
            probe.intensity = intensity;
            probe.boxProjection = boxProjection;
            probe.blendDistance = blendDistance;
            probe.resolution = (int)resolution;
            probe.hdr = hdr;
            probe.shadowDistance = shadowDist;
            probe.clearFlags = clearFlags;
            probe.backgroundColor = background;
            probe.cullingMask = cullingMask;
            probe.farClipPlane = clippingPlanes.y;
            probe.nearClipPlane = clippingPlanes.x;
            //Reflection.SetValue( probe, "m_RenderDynamicObjects", dynamicObjects );
            //probe.customBakedTexture = customCube;
        }
    }
}
