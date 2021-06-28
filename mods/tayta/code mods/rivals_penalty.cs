using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012D RID: 301
public class TIMissionModifier_AttackerRivalControlPoints : TIMissionModifier
{
	// Token: 0x0600042D RID: 1069 RVA: 0x000394F0 File Offset: 0x000376F0
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIUtilities.ObjectToNation(target, true);
		bool flag = tinationState == null;
		float result;
		if (flag)
		{
			result = 0f;
		}
		else
		{
			float num = 0f;
			float num2 = 0f;
			TIFactionState faction = attackingCouncilor.faction;
			foreach (TINationState tinationState2 in tinationState.rivals)
			{
				foreach (TIControlPoint ticontrolPoint in tinationState2.controlPoints)
				{
					num += 1f;
					bool flag2 = ticontrolPoint.faction == faction && !ticontrolPoint.benefitsDisabled;
					if (flag2)
					{
						num2 += 1f;
					}
				}
			}
			foreach (TINationState tinationState2 in tinationState.wars)
			{
				foreach (TIControlPoint ticontrolPoint in tinationState2.controlPoints)
				{
					num += 1f;
					bool flag2 = ticontrolPoint.faction == faction && !ticontrolPoint.benefitsDisabled;
					if (flag2)
					{
						num2 += 2f;
					}
				}
			}
			result = ((num > 0f) ? (num2 / num * this.maxValueFromAttackerAllyControlPoints) : 0f);
		}
		return result;
	}

	// Token: 0x0400040A RID: 1034
	public float maxValueFromAttackerAllyControlPoints = 6f;
}