using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000691 RID: 1681
	public class TINationState : TIPolityState
	{
		public float secrecy
		{
			get
			{
				return Mathf.Max((10f - this.democracy) * 5f + this.cohesion * 5f, 100f);
			}
		}

		public bool topSecret
		{
			get
			{
				return this.percentWeighttoPriority(PriorityType.Knowledge) >= this.secrecy;
			}
		}
	}