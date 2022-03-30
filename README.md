This is a small example of how I use my PrefabLibraryGenerator.cs script, which generates a PrefabLibrary class with properly typed references to the prefabs in your _Prefabs folder. 

The example project should run as is. In order to update the prefab library, go to Assets -> Generate Prefab Library. You only need to do this when the *structure* of the prefab folder changes, which is usually when you add new prefabs. You can also right click on the project window and choose Generate Prefab Library as well.

The script expects there to already be a folder named _Prefabs with a prefab named _PrefabLibrary in it. The _PrefabLibrary should have the PrefabLibrary component on it. You can create a placeholder PrefabLibrary.cs which the generator will then replace. You can change the paths for things to your own conventions by changing the static values in PrefabLibraryGenerator.cs

Any prefab with a name that starts with "_" will be ignored and not added to the prefab library. 

Any subfolders of the _Prefabs folder will be turned into arrays, where the type matches the items in the folder. I use this feature a lot in my own games, for example to populate a list of a bunch of textures. 

Let me know if you have any questions or run into any issues via my twitter @Mattrix. 

See also, a more advanced prefab library generator built upon this project by @BSChad: https://gist.github.com/BSChad/fb778623c2f5da9b470ae18a66489a3d
