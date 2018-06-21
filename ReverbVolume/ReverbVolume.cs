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
	public class ReverbVolume : Volume
	{
		public class GUILabels
		{
			public static GUIContent Layer { get { return new GUIContent( "Layer Mask", "The layer mask to limit the colliders that can invoke the trigger." ); } }
			public static GUIContent Room { get { return new GUIContent( "Room", "" ); } }
			public static GUIContent RoomHF { get { return new GUIContent( "Room HF", "" ); } }
			public static GUIContent RoomLF { get { return new GUIContent( "Room LF", "" ); } }
			public static GUIContent DecayTime { get { return new GUIContent( "Decay Time", "" ); } }
			public static GUIContent DecayHFRatio { get { return new GUIContent( "Decay HF Ratio", "" ); } }
			public static GUIContent Reflections { get { return new GUIContent( "Reflections", "" ); } }
			public static GUIContent ReflectionsDelay { get { return new GUIContent( "Reflections Delay", "" ); } }
			public static GUIContent Reverb { get { return new GUIContent( "Reverb", "" ); } }
			public static GUIContent ReverbDelay { get { return new GUIContent( "Reverb Delay", "" ); } }
			public static GUIContent HFReference { get { return new GUIContent( "HF Reference", "" ); } }
			public static GUIContent LFReference { get { return new GUIContent( "LF Reference", "" ); } }
			public static GUIContent Diffusion { get { return new GUIContent( "Diffusion", "" ); } }
			public static GUIContent Density { get { return new GUIContent( "Density", "" ); } }
		}

		public override Material BrushPreviewMaterial
		{
			get
			{
				return LoadMaterial( "Data/scsg_volume_reverb.mat", "ReverbVolume" );//(Material)SabreCSGResources.LoadObject( "Resources/Materials/scsg_volume_reverb.mat" );
			}
		}

		[SerializeField] public LayerMask layer = -1;
		[SerializeField] public int room = -1000;
		[SerializeField] public int roomHF = -100;
		[SerializeField] public int roomLF = 0;
		[SerializeField] public float decayTime = 1.49f;
		[SerializeField] public float decayHFRatio = 0.83f;
		[SerializeField] public int reflections = -2602;
		[SerializeField] public float reflectionsDelay = 0.007f;
		[SerializeField] public int reverb = 200;
		[SerializeField] public float reverbDelay = 0.011f;
		[SerializeField] public float hfReference = 5000;
		[SerializeField] public float lfReference = 250;
		[SerializeField] public float diffusion = 100;
		[SerializeField] public float density = 100;

#if UNITY_EDITOR


		public override bool OnInspectorGUI( Volume[] selectedVolumes )
		{
			var reverbVolumes = selectedVolumes.Cast<ReverbVolume>();
			bool invalidate = false;
			GUILayout.BeginVertical( "Box" );
			{
				GUILayout.Label( "Volume", EditorStyles.boldLabel );

				EditorGUI.indentLevel = 1;

				LayerMask previousLayerMask;
				layer = SabreGUILayout.LayerMaskField( GUILabels.Layer, ( previousLayerMask = layer ).value );
				if( previousLayerMask != layer )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.layer = layer;
					invalidate = true;
				}

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical( "Box" );
			{
				GUILayout.Label( "Reverb Zone", EditorStyles.boldLabel );

				EditorGUI.indentLevel = 1;

				int oldRoom;
				room = EditorGUILayout.IntSlider( GUILabels.Room, oldRoom = room, -10000, 0 );
				if( oldRoom != room )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.room = room;

					invalidate = true;
				}

				int oldRoomHF;
				roomHF = EditorGUILayout.IntSlider( GUILabels.RoomHF, oldRoomHF = roomHF, -10000, 0 );
				if( oldRoomHF != roomHF )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.roomHF = roomHF;

					invalidate = true;
				}

				int oldRoomLF;
				roomLF = EditorGUILayout.IntSlider( GUILabels.RoomLF, oldRoomLF = roomLF, -10000, 0 );
				if( oldRoomLF != roomLF )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.roomLF = roomLF;

					invalidate = true;
				}

				float oldDecayTime;
				decayTime = EditorGUILayout.Slider( GUILabels.DecayTime, oldDecayTime = decayTime, 0.1f, 20f );
				if( oldDecayTime != decayTime )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.decayTime = decayTime;

					invalidate = true;
				}

				float oldDecayHFRatio;
				decayHFRatio = EditorGUILayout.Slider( GUILabels.DecayHFRatio, oldDecayHFRatio = decayHFRatio, 0.1f, 2f );
				if( oldDecayHFRatio != decayHFRatio )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.decayHFRatio = decayHFRatio;

					invalidate = true;
				}

				int oldReflections;
				reflections = EditorGUILayout.IntSlider( GUILabels.Reflections, oldReflections = reflections, -10000, 1000 );
				if( oldReflections != reflections )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.reflections = reflections;

					invalidate = true;
				}

				float oldReflectionsDelay;
				reflectionsDelay = EditorGUILayout.Slider( GUILabels.ReflectionsDelay, oldReflectionsDelay = reflectionsDelay, 0f, 0.3f );
				if( oldReflectionsDelay != reflectionsDelay )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.reflectionsDelay = reflectionsDelay;

					invalidate = true;
				}

				int oldReverb;
				reverb = EditorGUILayout.IntSlider( GUILabels.Reverb, oldReverb = reverb, -10000, 2000 );
				if( oldReverb != reverb )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.reverb = reverb;

					invalidate = true;
				}

				float oldReverbDelay;
				reverbDelay = EditorGUILayout.Slider( GUILabels.ReverbDelay, oldReverbDelay = reverbDelay, 0f, 0.1f );
				if( oldReverbDelay != reverbDelay )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.room = room;

					invalidate = true;
				}

				float oldHFReference;
				hfReference = EditorGUILayout.Slider( GUILabels.HFReference, oldHFReference = hfReference, 1000f, 20000f );
				if( oldHFReference != hfReference )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.hfReference = hfReference;

					invalidate = true;
				}

				float oldLFReference;
				lfReference = EditorGUILayout.Slider( GUILabels.LFReference, oldLFReference = lfReference, 20f, 1000f );
				if( oldLFReference != lfReference )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.lfReference = lfReference;

					invalidate = true;
				}

				float oldDiffusion;
				diffusion = EditorGUILayout.Slider( GUILabels.Diffusion, oldDiffusion = diffusion, 0f, 100f );
				if( oldDiffusion != diffusion )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.diffusion = diffusion;

					invalidate = true;
				}

				float oldDensity;
				density = EditorGUILayout.Slider( GUILabels.Density, oldDensity = density, 0f, 100f );
				if( oldDensity != density )
				{
					foreach( ReverbVolume volume in reverbVolumes )
						volume.density = density;

					invalidate = true;
				}

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();

			return invalidate;
		}


		private Dictionary<string, Material> loadedObjects = new Dictionary<string, Material>();

		private Material LoadMaterial( string path, string relativeToScriptName )
		{
			bool found = false;

			Material loadedObject = null;

			// First of all see if there's a cached record
			if( loadedObjects.ContainsKey( path ) )
			{
				found = true;
				loadedObject = loadedObjects[path];

				// Now make sure the cached record actually points to something
				if( loadedObject != null )
				{
					return loadedObject;
				}
			}

			// Failed to load from cache, so load it from the Asset Database
			loadedObject = (Material)AssetDatabase.LoadMainAssetAtPath( Path.Combine( GetLocalResourcePath(relativeToScriptName), path ) );
			if( loadedObject != null )
			{
				if( found )
				{
					// A cache record was found but empty, so set the existing record to the newly loaded object
					loadedObjects[path] = loadedObject;
				}
				else
				{
					// We know that it's not already in the cache, so add it to the end
					loadedObjects.Add( path, loadedObject );
				}
			}
			return loadedObject;
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
			AudioReverbZone arz = volume.AddComponent<AudioReverbZone>();
			ReverbVolumeComponent rvc = volume.AddComponent<ReverbVolumeComponent>();

			arz.maxDistance = volume.GetComponent<Collider>().bounds.extents.magnitude;
			arz.minDistance = volume.GetComponent<Collider>().bounds.extents.magnitude* 0.95f;

			arz.room = room;
			arz.roomHF = roomHF;
			arz.roomLF = roomLF;
			arz.decayTime = decayTime;
			arz.decayHFRatio = decayHFRatio;
			arz.reflections = reflections;
			arz.reflectionsDelay = reflectionsDelay;
			arz.reverb = reverb;
			arz.reverbDelay = reverbDelay;
			arz.HFReference = hfReference;
			arz.LFReference = lfReference;
			arz.diffusion = diffusion;
			arz.density = density;

			rvc.layer = layer;
		}
}
}

#endif
