# Tayta's Mods

## Wars of Liberation

Functions changed:
- [TIRegionState.CompleteOccupationofRegion()](mods/tayta/code%20mods/liberation_wars.cs).

This modifies army occupation behaviour so that:
- Armies can conquer regions on behalf of allies who are not actively participating in the war.
  - This usually occurs when said allies are ineligible to participate due to not having armies of their own.
  - Allows LARPing and "vassal feeding".
- Armies can force regions to break away from war enemies, if there is a non-existent nation with a capital claim on that region.
  - This uses the regular Secession mechanic.
  - The breakaway will begin allied to the occupying army. This is necessary to avoid having the occupiers instantly evicted from the region.
  - The breakaway's CPs will go to unresting factions and _not_ the faction which controls the occupying army. Lore-wise, this is because the army is supporting local movements and not directly installing its own government. Gameplay-wise, this is fine, because the intent of the mechanic is to weaken the target nation, and not necessarily straight conquest. It also potentially leads to more intrigue by involving other factions in the war.

## Penalty for CPs in Rivals

Classes added:
- [TIMissionModifier_AttackerRivalControlPoints](mods/tayta/code%20mods/rivals_penalty.cs).

This adds a modifier for use in TIMissionTemplate.json. The effect is to penalize (although it could also give a bonus) missions targeting rivals of nations you already control, as it is otherwise not overly difficult to infiltrate two mortal enemies and have them make peace with the snap of a finger.

War enemies count as double for the purposes of calculation. This means that the maximum penalty can in fact go over 6.

## Secrecy Stat for Nations

Classes modified:
- [TINationState](mods/tayta/code%20mods/national_secrecy.cs).

The current formula for Secrecy is max((10 - Democracy) * 5 + Cohesion * 5).
