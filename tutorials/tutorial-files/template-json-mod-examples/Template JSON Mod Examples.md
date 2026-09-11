# Native data examples

These are independent examples. **Do not install this entire directory recursively**: the game discovers nested JSON, so that would also enable the other examples.

| Example | Copy into this directory under `Mods/Disabled` | Files to copy |
| --- | --- | --- |
| Alaska starts in Canada, 2022 scenario | `Example Mod` | This directory's `ModInfo.json` and `TIBilateralTemplate.json` only |
| Notification input delay | `Handbook Config Example` | Contents of `global-config-example` |
| One new Research org | `Handbook Org Example` | Contents of `org-example` |
| Ad Astra research cost | `Handbook Tech Example` | Contents of `tech-example` |

The examples use strict JSON and target **1.0.53a**. Follow the relevant guide's test steps in a disposable campaign; check [version notes](../../../docs/compatibility.md) when targeting **1.0.57**.

See [the first-mod tutorial](../../Create_Template_JSON_mod.md), [global config](../../../docs/global-config.md), [org authoring](../../Custom%20Orgs.md), and [tech/content authoring](../../../docs/content-authoring.md). Enable the selected mod and Use Mods, restart, and use a disposable campaign for testing. An existing save may retain already-created game state.
