#if UNITY_EDITOR || RUNTIME_CSG

using System;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Sabresaurus.SabreCSG.Volumes
{
	public class ReflectionProbeVolume : Volume
	{
		public override Material BrushPreviewMaterial
		{
			get
			{
				return LoadMaterial( "Data/scsg_volume_reflection.mat", "ReflectionProbeVolume" );//(Material)SabreCSGResources.LoadObject( "Resources/Materials/scsg_volume_reverb.mat" );
			}
		}

#if UNITY_EDITOR


		public override bool OnInspectorGUI( Volume[] selectedVolumes )
		{
			var reverbVolumes = selectedVolumes.Cast<ReflectionProbeVolume>();
			bool invalidate = false;
			GUILayout.BeginVertical( "Box" );
			{
				GUILayout.Label( "Volume", EditorStyles.boldLabel );

				EditorGUI.indentLevel = 1;

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical( "Box" );
			{
				GUILayout.Label( "Reflection Probe", EditorStyles.boldLabel );

				EditorGUI.indentLevel = 1;

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();

			return invalidate;
		}

		private Material LoadMaterial( string path, string relativeToScriptName )
		{
			Material loadedObject = (Material)AssetDatabase.LoadMainAssetAtPath( Path.Combine( GetLocalResourcePath(relativeToScriptName), path ) );
			
			if( loadedObject != null )
			{
				return loadedObject;
			}
			return new Material( Shader.Find( "Unlit/Colored" ) );
		}


		private string GetLocalResourcePath(string scriptName)
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
		public override void OnCreateVolume( GameObject volume )
		{
			ReflectionProbeVolumeComponent rpc = volume.AddComponent<ReflectionProbeVolumeComponent>();
		}
}
}

#endif
