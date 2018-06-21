using UnityEngine;

namespace Sabresaurus.SabreCSG.Volumes

{
	public class UsableTriggerVolumeComponent : MonoBehaviour
	{
		/// <summary>
		/// Whether to use a filter tag.
		/// </summary>
		public bool useFilterTag = false;

		/// <summary>
		/// The filter tag to limit the colliders that can invoke the trigger.
		/// </summary>
		public string filterTag = "Untagged";

		/// <summary>
		/// The layer mask to limit the colliders that can invoke the trigger.
		/// </summary>
		public LayerMask layer = -1;

		/// <summary>
		/// Whether the trigger can only be instigated once.
		/// </summary>
		public bool triggerOnceOnly = false;

		/// <summary>
		/// If we are using the input manager, then which input event do we use?
		/// </summary>
		public string inputEventName = "Submit";

		/// <summary>
		/// If we are using <see cref="KeyCode"/>, then which key do we use?
		/// </summary>
		public KeyCode inputKey = KeyCode.E;

		/// <summary>
		/// Which input type do we use?
		/// </summary>
		[SerializeField]
		public UseInputType useInputType = UseInputType.KeyCode;

		/// <summary>
		/// The event called when a collider enters the trigger volume.
		/// </summary>
		public TriggerVolumeEvent onUseEvent;

		/// <summary>
		/// Whether the trigger can still be triggered (used with <see cref="triggerOnceOnly"/>).
		/// </summary>
		private bool canTrigger = true;

		/// <summary>
		/// Called every frame while a collider stays inside the volume.
		/// </summary>
		/// <param name="other">The collider that is inside of the volume.</param>
		private void OnTriggerStay( Collider other )
		{
			// ignore empty events.
			if( onUseEvent.GetPersistentEventCount() == 0 )
				return;

			// tag filter:
			if( useFilterTag && other.tag != filterTag )
				return;

			// layer filter:
			if( !layer.Contains( other.gameObject.layer ) )
				return;

			// trigger once only:
			if( !triggerOnceOnly )
				canTrigger = true;

			if( !canTrigger )
				return;

			switch( useInputType )
			{
				case UseInputType.InputManager:
					if( Input.GetButtonDown( inputEventName ) )
					{
						onUseEvent.Invoke();
						canTrigger = false;
					}
					else if( Input.GetButtonDown( inputEventName ) )
						onUseEvent.Invoke();

					break;

				case UseInputType.KeyCode:
					if( Input.GetKeyDown( inputKey ) )
					{
						onUseEvent.Invoke();
						canTrigger = false;
					}
					else if( Input.GetKeyDown( inputKey ) )
						onUseEvent.Invoke();

					break;
			}
		}
	}
}
