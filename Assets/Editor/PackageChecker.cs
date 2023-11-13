#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class PackageChecker
    {
        private static readonly ListRequest Request;
    
        static PackageChecker()
        {
            Request = Client.List(); // List packages installed in the project
        
            EditorApplication.update += Process;
        }

        private static void Process()
        {
            if(!Request.IsCompleted)
                return;

            switch (Request.Status)
            {
                case StatusCode.Success when Request.Result.Any(package => package.name == "com.unity.xr.oculus"):
                    // Oculus package is installed, set the define symbol
                    SetDefineSymbol("OVR_IMPLEMENTED", true);
                    break;
                case StatusCode.Success:
                    // Oculus package is not installed, remove the define symbol
                    SetDefineSymbol("OVR_IMPLEMENTED", false);
                    break;
                case >= StatusCode.Failure:
                    Debug.LogError("Failed to list packages.");
                    break;
            }
            
            EditorApplication.update -= Process;
        }

        private static void SetDefineSymbol(string symbol, bool define)
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> definesList = definesString.Split(';').ToList();
        
            if (define)
            {
                if (definesList.Contains(symbol))
                    return;
                definesList.Add(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", definesList.ToArray()));
            }
            else
            {
                if (!definesList.Contains(symbol))
                    return;
                definesList.Remove(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", definesList.ToArray()));
            }
        }
    }
}
#endif