# Cookbook

Cookbook is intended as a collection of various code snippets and tricks for
common modding operations.

## Adding new recipe

* Create a new directory under cookbook for you recipe. The name of the
  directory should be descriptive of the recipe.
* Create `index.md` under the directory you created above. That is the main
  entry point to your recipe. Small recipes should be able to fit entirely into
  `index.md`. You can split you recipe onto multiple files.
  If you find yourself inclined to do so, consider making a tutorial instead.
* The content of the recipe entry should include `Version Compatibility`
  section mentioned first, but otherwise is free-form.
* The recipe dikrectory should include a compilable example referenced from the
  document. Put source files into `src` sub-directory of your recipe.
* Update the recipe list bellow to reference your recipe. The entry name should
  match the title of the recipe in the `index.md`

## Recipes

* [Patching data templates in the code](patching_data_templates_in_code/index.md)
* [Preserving mod state over save/load](save_mod_state_to_save_file/index.md)
* [Adding a console command](add_console_command/index.md)
