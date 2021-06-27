using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B2 RID: 1714
	public partial class TIRegionState : TIGameState
	{
		// Token: 0x06002CE9 RID: 11497 RVA: 0x000F2A4C File Offset: 0x000F0C4C
		public void CompleteOccupationofRegion(TINationState occupyingNation, TIArmyState army)
		{
			if (this.nation == occupyingNation || this.nation.allies.Contains(occupyingNation))
			{
				this.LiberateMyRegion();
			}
			else
			{
				this.occupier = occupyingNation;
				TINationState nation = this.nation;
				if (this == nation.capital)
				{
					List<TIRegionState> list = new List<TIRegionState>();
					List<TINationState> list2 = new List<TINationState>();
					foreach (TIRegionState item in nation.regions)
					{
						if (occupyingNation.claims.Contains(item))
						{
							list.Add(item);
						}
					}
					if (list.Count > 0)
					{
						list2.Add(occupyingNation);
					}
					foreach (TIRegionState region in list)
					{
						nation.TransferRegionControlTo(region, occupyingNation, true, false);
					}
					occupyingNation.SortAllianceList();
					foreach (TINationState tinationState in occupyingNation.allies)
					{
						List<TIRegionState> list3 = new List<TIRegionState>();
						foreach (TIRegionState item2 in nation.regions)
						{
							if ((tinationState.IsAtWarWith(nation) || tinationState.IsRivalWith(nation)) && tinationState.claims.Contains(item2))
							{
								list3.Add(item2);
							}
						}
						foreach (TIRegionState region2 in list3)
						{
							if (!list2.Contains(tinationState))
							{
								list2.Add(tinationState);
							}
							nation.TransferRegionControlTo(region2, tinationState, true, false);
						}
					}
					if (nation.extant)
					{
						nation.RegimeChange(army);
					}
					else
					{
						nation.AbsorbNation(nation);
						TINotificationQueueState.LogArmyConquersNation(army, nation, this, list2);
					}
				}
				else
				{
					List<TIRegionState> list4 = new List<TIRegionState>();
					list4.Add(this);
					List<TINationState> list5 = new List<TINationState>();
					list5 = this.NationsWithClaim(false, true, false, true);
					if (list5.Count > 0)
					{
						foreach (TINationState tinationState2 in list5)
						{
							occupyingNation.InitiateAlliance(tinationState2);
							this.nation.Secession(tinationState2, list4);
							break;
						}
					}
					TINotificationQueueState.LogArmyCompletesOccupationOfRegion(army);
				}
			}
			foreach (TIArmyState tiarmyState in this.armies)
			{
				army.SetArmyDataDirty();
			}
		}
	}
}
