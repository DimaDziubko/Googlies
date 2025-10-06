#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using AppLovinMax.Scripts.IntegrationManager.Editor;

namespace Assets.EditorIOS
{
    public class PostBuildStep
    {
        [PostProcessBuild(999)] // The execution order can be adjusted as needed
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            // Path to the Info.plist file
            string plistPath = Path.Combine(pathToBuildProject, "Info.plist");

            // Read the Info.plist file
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Get the root dictionary
            PlistElementDict rootDict = plist.root;

            // Get the user tracking usage description from AppLovin settings
            string userTrackingUsageDescription = "This uses device info for more personalized ads and content";

            // Check if the description is set
            if (string.IsNullOrEmpty(userTrackingUsageDescription))
            {
                // Provide a default description or handle the case appropriately
                userTrackingUsageDescription = "This identifier will be used to deliver personalized ads to you.";
            }

            // Set the NSUserTrackingUsageDescription key with the description
            rootDict.SetString("NSUserTrackingUsageDescription", userTrackingUsageDescription);

            // Save the updated Info.plist
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif