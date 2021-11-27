
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class PrefabLibraryGenerator
{
    static string SOURCE_PATH = "Assets/_Prefabs";
    static string LIBRARY_CLASS_PATH = "Assets/Code/PrefabLibrary.cs";
    static string LIBRARY_PATH = "Assets/_Prefabs/_PrefabLibrary.prefab";

    [MenuItem("Assets/Generate Prefab Library", priority = 1000000)] //high priority number puts it at the bottom of the list 
    public static void GeneratePrefabLibrary()
    {
        var guids = AssetDatabase.FindAssets("", new string[] { SOURCE_PATH });

        var infos = new List<PrefabInfo>();
        var folders = new List<FolderInfo>();

        foreach(var guid in guids)
        {
            var info = new PrefabInfo();
            info.guid = guid;
            info.path = AssetDatabase.GUIDToAssetPath(guid);

            var fileName = Path.GetFileName(info.path);

            if (fileName[0] == '_') continue; //don't import any prefab starting with _
            if (info.path == LIBRARY_PATH) continue; //don't import the actual library as a prefab!

            var folderPath = Path.GetDirectoryName(info.path);
            var folderName = folderPath.Substring(SOURCE_PATH.Length);

            if(folderName != "")
            {
                folderName = folderName.Substring(1); //remove the leading slash

                var folder = folders.FirstOrDefault(f=>f.name == folderName);

                var thing = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(info.path);

                if(folder == null) //create folder for the first time
                {
                    folder = new FolderInfo();
                    folders.Add(folder);

                    folder.name = folderName;
                    folder.path = folderPath;

                    folder.mainType = thing.GetType();

                    folder.declaration = "\tpublic " + folder.mainType.Name + "[] " + folder.name +";\n";
                    folder.yaml += $"  {folder.name}:\n";

                    Debug.Log("found folder! " + folder.name);
                }

                if(thing.GetType() != folder.mainType) //if there are multiple types, we just use UnityEngine.Object
                {
                    folder.declaration = "\tpublic UnityEngine.Object[] " + folder.name +";\n";
                }

                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(thing, out _, out info.fileID)) continue;  //this seems to be the only way to get a file ID?

                folder.yaml += $"  - {{fileID: {info.fileID}, guid: {info.guid}, type: 3}}\n";
            }
            else
            { 
                info.go = AssetDatabase.LoadAssetAtPath<GameObject>(info.path);

                if (info.go == null) continue;

                var comps = info.go.GetComponents<Component>();

                if (comps.Length == 1) info.mainIndex = 0;
                else info.mainIndex = 1;

                info.mainComp = comps[info.mainIndex];
                info.mainType = info.mainComp.GetType();

                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(info.mainComp, out _, out info.fileID)) continue;  //this seems to be the only way to get a file ID?

                //remove root path from the start and '.prefab' from the end
                info.fullName = info.path.Replace('/', '_').Substring(SOURCE_PATH.Length+1, info.path.Length-7-(SOURCE_PATH.Length + 1)); 

                info.declaration = "\tpublic " + info.mainType.Name + " " + info.fullName +";\n";

                //yaml format:  Blasts_BasicBlast: {fileID: 5991340264641166209, guid: 22351f65547537a409ab91ed83ac7e9f, type: 3}
                info.yaml = $"  {info.fullName}: {{fileID: {info.fileID}, guid: {info.guid}, type: 3}}\n";

                infos.Add(info);
            }
        }

        var declarationString = string.Join("",infos.Select(info => info.declaration).ToArray());
        var yamlString = string.Join("", infos.Select(info => info.yaml).ToArray());

        foreach(var folder in folders)
        {
            declarationString += folder.declaration;
            yamlString += folder.yaml;
        }

        string managerPath = LIBRARY_PATH;

        var managerGO = AssetDatabase.LoadAssetAtPath<GameObject>(managerPath);

        if(managerGO != null)
        {
            var prefabText = File.ReadAllText(managerPath);

            string textToFind = "m_EditorClassIdentifier:";

            var clearIndex = prefabText.LastIndexOf(textToFind) + textToFind.Length;

            if (clearIndex != -1)
            {
                prefabText = prefabText.Substring(0, clearIndex);
                prefabText += "\n" + yamlString;
            }

            File.WriteAllText(managerPath,prefabText);
        }
        else
        {
            Debug.Log("PrefabLibrary not found! Create a prefab with the PrefabLibrary component at " + LIBRARY_PATH);
        }

        WriteClass(LIBRARY_CLASS_PATH, declarationString);
    }

    public class PrefabInfo
    {
        public string guid;
        public string path;
        public long fileID;

        public string fullName;

        public GameObject go;
        public int mainIndex;
        public Component mainComp;
        public Type mainType;

        public string declaration;
        public string yaml;
    }

    public class FolderInfo
    {
        public string path;

        public string name;

        public Type mainType;

        public string declaration = "";
        public string yaml = "";
    }

    public static void WriteClass(string scriptPath, string declarationsString)
    {
        var scriptCode = "//GENERATED CLASS FROM PrefabLibraryGenerator.cs\n\n" +
            "using UnityEngine;\n\n[DefaultExecutionOrder(-1000)]//make it run first!\npublic class PrefabLibrary : MonoBehaviour\n{\n" +
            declarationsString + "\n" +
            "\tpublic static PrefabLibrary instance;\n\n\tpublic void Awake()\n\t{\n\t\tinstance = this;\n\t}\n\n}";

        File.WriteAllText(scriptPath, scriptCode);

        MonoScript script = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(MonoScript)) as MonoScript;

        if (script != null)
        {
            AssetDatabase.ImportAsset(scriptPath);
        }
    }

}
