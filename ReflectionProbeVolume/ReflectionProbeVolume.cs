#if UNITY_EDITOR || RUNTIME_CSG

using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Sabresaurus.SabreCSG.Volumes
{
    public class ReflectionProbeVolume : Volume
    {
#if UNITY_EDITOR

        public override Material BrushPreviewMaterial
        {
            get
            {
                return LoadMaterial( "Data/scsg_volume_reflection.mat", "ReflectionProbeVolume" );
            }
        }

        public ReflectionProbeMode type = ReflectionProbeMode.Baked;
        public ReflectionProbeTimeSlicingMode timeMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
        public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.OnAwake;
        public int importance = 1;
        public float intensity = 1;
        public bool boxProjection = false;
        public float blendDistance = 1;
        public ReflectionCubeResolution resolution = ReflectionCubeResolution._128;
        public bool hdr = true;
        public float shadowDist = 100;
        public ReflectionProbeClearFlags clearFlags = ReflectionProbeClearFlags.Skybox;
        public Color background = new Color32( 49, 77, 128, 0 );
        public LayerMask cullingMask = ~0;
        public Vector2 clippingPlanes = new Vector2( 0.3f, 1000.0f ); //x = near, y = far
        //public bool dynamicObjects = false;
        //public Cubemap customCube = null;

        private ReflectionProbe p;

        public override bool OnInspectorGUI( Volume[] selectedVolumes )
        {
            System.Collections.Generic.IEnumerable<ReflectionProbeVolume> rpVolumes = selectedVolumes.Cast<ReflectionProbeVolume>();
            bool invalidate = false;

            GUILayout.Label( "Reflection Probe", "OL Title" );

            GUILayout.BeginVertical();
            {
                EditorGUI.indentLevel = 1;

                GUILayout.BeginVertical( "grey_border" );
                {
                    GUILayout.Space( 4 );

                    EditorGUILayout.LabelField( "General Settings", EditorStyles.boldLabel );
                    ReflectionProbeMode oldType;
                    type = (ReflectionProbeMode)EditorGUILayout.EnumPopup( "Type", oldType = type );
                    if( oldType != type )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.type = type;
                        }

                        invalidate = true;
                    }

                    if( type == ReflectionProbeMode.Custom )
                    {
                        EditorGUILayout.HelpBox( "Editing custom probes is currently not supported by volumes. This can be set manually on the volume component.", MessageType.Info );

                        /*
                            bool oldDynamicObjects;
                            dynamicObjects = EditorGUILayout.Toggle( new GUIContent( "Dynamic Objects" ), oldDynamicObjects = dynamicObjects );
                            if( oldDynamicObjects != dynamicObjects )
                            {
                                foreach( ReflectionProbeVolume volume in rpVolumes )
                                    volume.dynamicObjects = dynamicObjects;

                                invalidate = true;
                            }

                            Cubemap oldCustomCube;
                            customCube = (Cubemap)EditorGUILayout.ObjectField( "Custom Cube", oldCustomCube = customCube, typeof( Cubemap ), false );
                            if( oldCustomCube != customCube )
                            {
                                foreach( ReflectionProbeVolume volume in rpVolumes )
                                    volume.customCube = customCube;

                                invalidate = true;
                            }
                        */
                    }

                    if( type == ReflectionProbeMode.Realtime )
                    {
                        EditorGUI.indentLevel = 2;
                        ReflectionProbeRefreshMode oldRefreshMode;
                        refreshMode = (ReflectionProbeRefreshMode)EditorGUILayout.EnumPopup( "Refresh Mode", oldRefreshMode = refreshMode );
                        if( oldRefreshMode != refreshMode )
                        {
                            foreach( ReflectionProbeVolume volume in rpVolumes )
                            {
                                volume.refreshMode = refreshMode;
                            }

                            invalidate = true;
                        }

                        ReflectionProbeTimeSlicingMode oldTimeMode;
                        timeMode = (ReflectionProbeTimeSlicingMode)EditorGUILayout.EnumPopup( "Time Slicing", oldTimeMode = timeMode );
                        if( oldTimeMode != timeMode )
                        {
                            foreach( ReflectionProbeVolume volume in rpVolumes )
                            {
                                volume.timeMode = timeMode;
                            }

                            invalidate = true;
                        }
                        EditorGUI.indentLevel = 1;
                    }

                    GUILayout.Space( 4 );
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical( "grey_border" );
                {
                    GUILayout.Space( 4 );
                    EditorGUILayout.LabelField( "Runtime Settings", EditorStyles.boldLabel );

                    int oldImportance;
                    importance = EditorGUILayout.IntField( "Importance", oldImportance = importance );
                    if( oldImportance != importance )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.importance = importance;
                        }

                        invalidate = true;
                    }

                    float oldIntensity;
                    intensity = EditorGUILayout.FloatField( "Intensity", oldIntensity = intensity );
                    if( oldIntensity != intensity )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.intensity = intensity;
                        }

                        invalidate = true;
                    }

                    bool oldBoxProj;
                    boxProjection = EditorGUILayout.Toggle( "Box Projection", oldBoxProj = boxProjection );
                    if( oldBoxProj != boxProjection )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.boxProjection = boxProjection;
                        }

                        invalidate = true;
                    }

                    float oldBlendDist;
                    blendDistance = EditorGUILayout.FloatField( "Blend Distance", oldBlendDist = blendDistance );
                    if( oldBlendDist != blendDistance )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.blendDistance = blendDistance;
                        }

                        invalidate = true;
                    }

                    EditorGUILayout.HelpBox( "Size and origin is set by the volume", MessageType.Info );

                    GUILayout.Space( 4 );
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical( "grey_border" );
                {
                    GUILayout.Space( 4 );
                    EditorGUILayout.LabelField( "Cubemap Capture Settings", EditorStyles.boldLabel );

                    ReflectionCubeResolution oldRes;
                    resolution = (ReflectionCubeResolution)EditorGUILayout.EnumPopup( "Resolution", oldRes = resolution );
                    if( oldRes != resolution )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.resolution = resolution;
                        }

                        invalidate = true;
                    }

                    bool oldHDR;
                    hdr = EditorGUILayout.Toggle( "HDR", oldHDR = hdr );
                    if( oldHDR != hdr )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.hdr = hdr;
                        }

                        invalidate = true;
                    }

                    float oldShadowDist;
                    shadowDist = EditorGUILayout.FloatField( "Shadow Distance", oldShadowDist = shadowDist );
                    if( oldShadowDist != shadowDist )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.shadowDist = shadowDist;
                        }

                        invalidate = true;
                    }

                    ReflectionProbeClearFlags oldCFlags;
                    clearFlags = (ReflectionProbeClearFlags)EditorGUILayout.EnumPopup( "Clear Flags", oldCFlags = clearFlags );
                    if( oldCFlags != clearFlags )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.clearFlags = clearFlags;
                        }

                        invalidate = true;
                    }

                    Color oldColor;
                    background = EditorGUILayout.ColorField( "Background", oldColor = background );
                    if( oldColor != background )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.background = background;
                        }

                        invalidate = true;
                    }

                    LayerMask oldMask;
                    cullingMask = SabreGUILayout.LayerMaskField( new GUIContent( "Culling Mask" ), oldMask = cullingMask );//EditorGUILayout.MaskField( "Culling Mask", oldMask = cullingMask );
                    if( oldMask != cullingMask )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.cullingMask = cullingMask;
                        }

                        invalidate = true;
                    }

                    Vector2 oldClip;
                    clippingPlanes = EditorGUILayout.Vector2Field( new GUIContent( "Clipping Planes", "X = Near, Y = Far" ), oldClip = clippingPlanes );
                    if( oldClip != clippingPlanes )
                    {
                        foreach( ReflectionProbeVolume volume in rpVolumes )
                        {
                            volume.clippingPlanes = clippingPlanes;
                        }

                        invalidate = true;
                    }

                    EditorGUILayout.HelpBox( "Occlusion is currently not supported by volumes. This can be set on the volume component.", MessageType.Info );

                    GUILayout.Space( 4 );
                }
                GUILayout.EndVertical();

                if( type == ReflectionProbeMode.Custom )
                {
                    EditorGUILayout.HelpBox( "This probe is set to Custom, and is not handled by volumes.", MessageType.Info );
                }
                else if( type == ReflectionProbeMode.Realtime )
                {
                    EditorGUILayout.HelpBox( "This probe is set to Realtime, and its baking is handled by unity. The results are stored in the GI Cache.", MessageType.Info );
                }

                EditorGUI.indentLevel = 0;
            }
            GUILayout.EndVertical();

            return invalidate;
        }

        public override void OnCreateVolume( GameObject volume )
        {
            ReflectionProbeVolumeComponent rpc = volume.AddOrGetComponent<ReflectionProbeVolumeComponent>();
            p = volume.GetComponent<ReflectionProbe>();

            rpc.Setup(); // init bounds and set them up on the probe

            rpc.type = type;
            rpc.timeMode = timeMode;
            rpc.refreshMode = refreshMode;
            rpc.importance = importance;
            rpc.intensity = intensity;
            rpc.boxProjection = boxProjection;
            rpc.blendDistance = blendDistance;
            rpc.resolution = resolution;
            rpc.hdr = hdr;
            rpc.shadowDist = shadowDist;
            rpc.clearFlags = clearFlags;
            rpc.background = background;
            rpc.cullingMask = cullingMask;
            rpc.clippingPlanes = clippingPlanes;
            //rpc.dynamicObjects = dynamicObjects;
            //rpc.customCube = customCube;

            rpc.Apply(); // make sure the probe knows the new settings.
        }

        private Material LoadMaterial( string path, string relativeToScriptName )
        {
            Material loadedObject = (Material)AssetDatabase.LoadMainAssetAtPath( Path.Combine( GetLocalResourcePath( relativeToScriptName ), path ) );

            if( loadedObject != null )
            {
                return loadedObject;
            }
            return new Material( Shader.Find( "Unlit/Colored" ) );
        }

        private string GetLocalResourcePath( string scriptName )
        {
            string[] guids = AssetDatabase.FindAssets( scriptName + " t:Script" );

            foreach( string guid in guids )
            {
                string path = AssetDatabase.GUIDToAssetPath( guid );
                string suffix = scriptName + ".cs";

                //Debug.Log( path );

                if( path.EndsWith( suffix ) )
                {
                    path = path.Remove( path.Length - suffix.Length, suffix.Length );
                    return path;
                }
            }
            return string.Empty;
        }

#endif
    }
}

#endif
