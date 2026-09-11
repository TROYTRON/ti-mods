# Uploading and updating Steam Workshop mods

Use the game's publishing interface to create or update an item. The **1.0.53a** upload helper uses `WorkshopItemInfo.xml` to retain item identity. Interface labels and the older screenshots below may differ in your build; the upload sequence has not been retested here.

## Prepare the exact package

Test the deployed mod before staging an upload. Record game version, branch, DLC requirements, and any code-loader dependency. A Workshop subscription does not establish that a third-party loader is installed; state the required installation steps in the mod description.

Prepare a clean copy containing the native `ModInfo.json` and template/localization files, each required AssetBundle with its `.manifest`, and any FMOD bank files. Code mods also need their loader-specific metadata and compiled DLL as described in [the code-mod guide](code-mods-with-umm.md). Do not ship the whole `Mods/Enabled` directory, game assemblies, unrelated mod files, or an entire Unity/FMOD authoring project by accident.

Use a file list to check the package. Copying new files over old staging contents leaves deleted files behind, so explicitly compare the staged contents with the intended release and remove only confirmed obsolete mod files. Preserve the Workshop identity file described below.

## Upload a new item

1. Open Terra Invicta's mod publishing interface and select the new-mod upload action.
2. Select **OPEN MOD FOLDER**. Use the exact folder the game opens; do not guess a directory from a screenshot or rename the generated folder.
3. Copy the contents of your prepared release into that folder, preserving the mod's internal structure. Avoid an accidental extra wrapper directory.
4. Review the title, description, preview image, dependencies, and intended visibility shown by the interface. Select **UPLOAD MOD** when the staged package is ready.
5. Wait for completion and open the resulting Workshop page. Check item identity, description, visibility, and any outstanding Steam Workshop agreement prompt. Steam's upload flow can require agreement acceptance before an item is visible. [Steamworks Workshop documentation](https://partner.steamgames.com/doc/features/workshop/implementation)

![Historical upload interface](https://user-images.githubusercontent.com/11687023/194989961-bf3bb725-b94f-43e1-aab5-33d858530adc.png)

After creation, back up **`WorkshopItemInfo.xml`** and record the Workshop page URL/item ID. The helper stores `PublishedFileId` there to identify the existing item for later updates. Keep that backup outside the staging folder as well.

## Update the existing item

1. In the game's publishing interface, select **UPDATE EXISTING MOD**, then choose the intended existing item. Verify its title and Workshop ID.
2. Select **OPEN MOD FOLDER**. Back up its `WorkshopItemInfo.xml` before changing staged contents.
3. Replace the staged release files with the tested new version. Compare file lists so renamed bundles, removed JSON, or obsolete DLLs do not survive. Preserve the existing item metadata; do not substitute another mod's XML file.
4. Update release notes/version information and review the package. Select **UPLOAD MOD** and wait for a successful result.
5. Check the same Workshop URL after upload. Confirm its update time and content; creating a second item is not an update of the original subscription.

![Historical update selection](https://user-images.githubusercontent.com/11687023/194990246-731c681a-fcab-4567-90be-55833a0f959a.png)

If the item cannot be found, confirm the active Steam account and ownership, retain the metadata backup, and inspect the error log before creating a replacement listing. A local XML file does not transfer ownership of somebody else's Workshop item.

## Troubleshoot an upload failure

If the log shows `k_EResultLimitExceeded`, check the preview image's file size. Reducing it to **below 500 KB** resolved one upload failure reported by Long, following Stallion's suggestion; that is a useful troubleshooting size, not an official limit. [Confirmed result](https://discord.com/channels/462769550841348126/780213497028018207/1508068274422808777).

Keep the log, inspect the failed upload stage, and verify the existing item's ID before retrying. A failed upload can leave a blank item; creating another listing immediately can duplicate it. If reducing the preview does not help, continue from the actual error rather than assuming every failure has the same cause.

## Check the subscriber installation

The staging folder, your local development mod, and Steam's downloaded Workshop copy are different locations. Confirm the subscriber copy receives the update, enable it through the game's mod controls, and restart. Test with your separate development copy disabled so it cannot mask a missing Workshop file or create duplicate template/bundle conflicts.

Repeat the mod's actual feature test, then save/reload when relevant. Keep the published package with its source revision. See [assets](../docs/assets.md) for bundle requirements and [debugging](../docs/debugging.md) for logs.
