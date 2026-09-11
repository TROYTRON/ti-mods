# Adding a console command

Use [TiMods.Console](../../examples/code/TiMods.Console/Main.cs), which registers `timods_echo` and removes its own registration when disabled. [Build and install it](../../examples/code/README.md) with your local game references.

## Register at the right time

`TerminalController.RegisterCommand` accepts a command name, a handler taking `string[]`, and help text. Give your command a unique prefix and retain the controller needed for `Output` and `OutputError`.

The example handles both lifecycles: it checks for an existing terminal through `GlobalInstaller.container.TryResolve<Terminal>()`, and patches `Terminal.Initialize` for a terminal initialized later. Registration must be repeatable; the underlying dictionary rejects duplicate names.

## Validate arguments and clean up

The game's parser trims comma-separated arguments. A bare command can arrive as **one empty string**, so test for empty/whitespace content rather than relying only on `args.Length == 0`. The example prints usage for empty input and echoes the supplied text without changing campaign state.

In **1.0.53a**, there is no public unregister operation. The example accesses the private `TerminalController.commands` dictionary and remembers the exact `CommandRegistration` it added. On disable, it removes the entry only if that object is still the current registration, preserving a replacement made by another mod. Destroyed terminals can have a null dictionary; cleanup skips those safely. Recheck this private field after updates.

## Test

Open the game's debug console, run `help`, `timods_echo hello`, and bare `timods_echo`. Confirm the help entry, echoed text and usage message. Toggle the mod off and verify the command disappears, then enable it again and check that registration succeeds once. Restart and repeat with any other console mods you support.

The project compiles against the handbook's 1.0.53a baseline; console behavior still needs an in-game test on your installation.
