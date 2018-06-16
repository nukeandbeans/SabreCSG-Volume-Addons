using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sabresaurus.SabreCSG.Volumes
{
	public class ReverbVolumeComponent : MonoBehaviour
	{
		public LayerMask layer = -1;

		private AudioReverbZone arz;

		private void Awake()
		{
			arz = GetComponent<AudioReverbZone>();
		}

		private void OnTriggerStay( Collider other )
		{
			if( !layer.Contains( other.gameObject.layer ) )
				return;

			arz.enabled = true;
		}

		private void OnTriggerExit( Collider other )
		{
			if( !layer.Contains( other.gameObject.layer ) )
				return;

			arz.enabled = false;
		}
	}
}
