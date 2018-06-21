#if UNITY_EDITOR || RUNTIME_CSG

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;
using System.Linq;

#endif

namespace Sabresaurus.SabreCSG.Volumes
{
	public class UsableTriggerVolume : Volume
	{
		public override Material BrushPreviewMaterial
		{
			get
			{
				return LoadMaterial( "Data/scsg_volume_usable_trigger.mat", "UsableTriggerVolume" );
			}
		}

		/// <summary>
		/// Whether to use a filter tag.
		/// </summary>
		[SerializeField]
		public bool useFilterTag = false;

		/// <summary>
		/// The filter tag to limit the colliders that can invoke the trigger.
		/// </summary>
		[SerializeField]
		public string filterTag = "Untagged";

		/// <summary>
		/// The layer mask to limit the colliders that can invoke the trigger.
		/// </summary>
		[SerializeField]
		public LayerMask layer = -1;

		/// <summary>
		/// Whether the trigger can only be instigated once.
		/// </summary>
		[SerializeField]
		public bool triggerOnceOnly = false;

		/// <summary>
		/// If we are using the input manager, then which input event do we use?
		/// </summary>
		[SerializeField]
		public string inputEventName = "Submit";

		/// <summary>
		/// If we are using <see cref="KeyCode"/>, then which key do we use?
		/// </summary>
		[SerializeField]
		public KeyCode inputKey = KeyCode.E;

		/// <summary>
		/// Which input type do we use?
		/// </summary>
		[SerializeField]
		public UseInputType useInputType = UseInputType.KeyCode;

		/// <summary>
		/// The event called when a collider enters the trigger volume.
		/// </summary>
		[SerializeField]
		public TriggerVolumeEvent onUseEvent;

#if UNITY_EDITOR
		private Dictionary<string, Material> loadedObjects = new Dictionary<string, Material>();

		public override bool OnInspectorGUI( Volume[] selectedVolumes )
		{
			var volumes = selectedVolumes.Cast<UsableTriggerVolume>();
			bool invalidate = false;

			GUILayout.BeginVertical( "Box" );
			{
				EditorGUILayout.LabelField( "Trigger Options", EditorStyles.boldLabel );
				GUILayout.Space( 4 );
				EditorGUI.indentLevel = 1;

				GUILayout.BeginVertical();
				{
					LayerMask previousLayerMask;
					layer = SabreGUILayout.LayerMaskField( new GUIContent( "Layer Mask", "The layer mask to limit the colliders that can invoke the trigger." ), ( previousLayerMask = layer ).value );
					if( previousLayerMask != layer )
					{
						foreach( UsableTriggerVolume volume in volumes )
							volume.layer = layer;
						invalidate = true;
					}

					bool previousUseFilterTag;
					useFilterTag = EditorGUILayout.Toggle( new GUIContent( "Use Filter Tag", "Whether to use a filter tag." ), previousUseFilterTag = useFilterTag );
					if( useFilterTag != previousUseFilterTag )
					{
						foreach( UsableTriggerVolume volume in volumes )
							volume.useFilterTag = useFilterTag;
						invalidate = true;
					}

					if( useFilterTag )
					{
						string previousFilterTag;
						filterTag = EditorGUILayout.TagField( new GUIContent( "Filter Tag", "The filter tag to limit the colliders that can invoke the trigger." ), previousFilterTag = filterTag );
						if( filterTag != previousFilterTag )
						{
							foreach( UsableTriggerVolume volume in volumes )
								volume.filterTag = filterTag;
							invalidate = true;
						}
					}

					bool previousTriggerOnce;
					triggerOnceOnly = EditorGUILayout.Toggle( new GUIContent( "Trigger Once Only", "Whether the trigger can only be instigated once." ), previousTriggerOnce = triggerOnceOnly );
					if( triggerOnceOnly != previousTriggerOnce )
					{
						foreach( UsableTriggerVolume volume in volumes )
							volume.triggerOnceOnly = triggerOnceOnly;
						invalidate = true;
					}
				}
				GUILayout.EndVertical();

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical( "Box" );
			{
				EditorGUILayout.LabelField( "Use Settings", EditorStyles.boldLabel );
				GUILayout.Space( 4 );

				EditorGUI.indentLevel = 1;

				UseInputType oldUseInputType;
				useInputType = (UseInputType)EditorGUILayout.EnumPopup( new GUIContent( "Input Type" ), oldUseInputType = useInputType );
				if( useInputType != oldUseInputType )
				{
					foreach( UsableTriggerVolume volume in volumes )
						volume.useInputType = useInputType;
					invalidate = true;
				}

				switch( useInputType )
				{
					case UseInputType.InputManager:
						string oldInputEventName;
						inputEventName = EditorGUILayout.TextField( new GUIContent( "Input Button", "The name of the input button set up in the input manager." ), oldInputEventName = inputEventName );
						if( inputEventName != oldInputEventName )
						{
							foreach( UsableTriggerVolume volume in volumes )
								volume.inputEventName = inputEventName;
							invalidate = true;
						}

						break;

					case UseInputType.KeyCode:
						KeyCode oldInputKey;
						inputKey = (KeyCode)EditorGUILayout.EnumPopup( new GUIContent( "Input Key" ), oldInputKey = inputKey );
						if( inputKey != oldInputKey )
						{
							foreach( UsableTriggerVolume volume in volumes )
								volume.inputKey = inputKey;
							invalidate = true;
						}

						break;
				}

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical( "Box" );
			{
				EditorGUILayout.LabelField( "Trigger Events", EditorStyles.boldLabel );
				GUILayout.Space( 4 );

				EditorGUI.indentLevel = 1;

				GUILayout.BeginVertical();
				{
					SerializedObject tv = new SerializedObject( this );
					SerializedProperty prop1 = tv.FindProperty( "onUseEvent" );

					EditorGUI.BeginChangeCheck();

					EditorGUILayout.PropertyField( prop1, new GUIContent( "On Use Event" ) );

					if( EditorGUI.EndChangeCheck() )
					{
						tv.ApplyModifiedProperties();
						foreach( UsableTriggerVolume volume in volumes )
						{
							volume.onUseEvent = onUseEvent;
						}
						invalidate = true;
					}
				}
				GUILayout.EndVertical();

				EditorGUI.indentLevel = 0;
			}
			GUILayout.EndVertical();

			return invalidate;
		}

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
			loadedObject = (Material)AssetDatabase.LoadMainAssetAtPath( Path.Combine( GetLocalResourcePath( relativeToScriptName ), path ) );
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

		public override void OnCreateVolume( GameObject volume )
		{
			UsableTriggerVolumeComponent tvc = volume.AddComponent<UsableTriggerVolumeComponent>();
			tvc.useFilterTag = useFilterTag;
			tvc.filterTag = filterTag;
			tvc.layer = layer;
			tvc.triggerOnceOnly = triggerOnceOnly;
			tvc.inputEventName = inputEventName;
			tvc.inputKey = inputKey;
			tvc.useInputType = useInputType;
			tvc.onUseEvent = onUseEvent;
		}
	}
}

#endif
