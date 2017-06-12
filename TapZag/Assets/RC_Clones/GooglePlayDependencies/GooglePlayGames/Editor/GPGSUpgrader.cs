// <copyright file="GPGSUpgrader.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))

namespace GooglePlayGames.Editor
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// GPGS upgrader handles performing and upgrade tasks.
    /// </summary>
    [InitializeOnLoad]
    public class GPGSUpgrader
    {
        /// <summary>
        /// Initializes static members of the <see cref="GooglePlayGames.GPGSUpgrader"/> class.
        /// </summary>
        static GPGSUpgrader()
        {
            string prevVer = GPGSProjectSettings.Instance.Get(GPGSUtil.LASTUPGRADEKEY, "00000");
            if (!prevVer.Equals(PluginVersion.VersionKey))
            {
                // if this is a really old version, upgrade to 911 first, then 915
                if (!prevVer.Equals(PluginVersion.VersionKeyCPP))
                {
                    prevVer = Upgrade911(prevVer);
                }

                prevVer = Upgrade915(prevVer);

                prevVer = Upgrade927Patch(prevVer);

                // Upgrade to remove gpg version of jar resolver
                prevVer = Upgrade928(prevVer);

                prevVer = Upgrade930(prevVer);

                prevVer = Upgrade931(prevVer);

                prevVer = Upgrade935(prevVer);

                // there is no migration needed to 930+
                if (!prevVer.Equals(PluginVersion.VersionKey))
                {
                    Debug.Log("Upgrading from format version " + prevVer + " to " + PluginVersion.VersionKey);
                    prevVer = PluginVersion.VersionKey;
                }

                string msg = GPGSStrings.PostInstall.Text.Replace(
                                 "$VERSION",
                                 PluginVersion.VersionString);
                EditorUtility.DisplayDialog(GPGSStrings.PostInstall.Title, msg, "OK");
            }

            GPGSProjectSettings.Instance.Set(GPGSUtil.LASTUPGRADEKEY, prevVer);
            GPGSProjectSettings.Instance.Set(GPGSUtil.PLUGINVERSIONKEY,
                PluginVersion.VersionString);
            GPGSProjectSettings.Instance.Save();

            // clean up duplicate scripts if Unity 5+
            int ver = GPGSUtil.GetUnityMajorVersion();

            if (ver >= 5)
            {
                string[] paths =
                    {
                        GPGSUtil.modifiedDir+"/GooglePlayGames",
                        GPGSUtil.modifiedDir+"/Plugins/Android",
                        GPGSUtil.modifiedDir+"/PlayServicesResolver"
                    };
                foreach (string p in paths)
                {
                    CleanDuplicates(p);
                }

                // remove support lib from old location.
                string jarFile = GPGSUtil.modifiedDir+"/Plugins/Android/libs/android-support-v4.jar";
                if (File.Exists(jarFile))
                {
                    File.Delete(jarFile);
                }

                // remove the massive play services client lib
                string clientDir = GPGSUtil.modifiedDir+"/Plugins/Android/google-play-services_lib";
                GPGSUtil.DeleteDirIfExists(clientDir);
            }

            // Check that there is a AndroidManifest.xml file
            if (!GPGSUtil.AndroidManifestExists())
            {
                GPGSUtil.GenerateAndroidManifest();
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Cleans the duplicate files.  There should not be any since
        /// we are keeping track of the .meta files.
        /// </summary>
        /// <param name="root">Root of the directory to clean.</param>
        private static void CleanDuplicates(string root)
        {
            string[] subDirs = Directory.GetDirectories(root);

            // look for .1 and .2
            string[] dups = Directory.GetFiles(root, "* 1.*");
            foreach (string d in dups)
            {
                Debug.Log("Deleting duplicate file: " + d);
                File.Delete(d);
            }

            dups = Directory.GetFiles(root, "* 2.*");
            foreach (string d in dups)
            {
                Debug.Log("Deleting duplicate file: " + d);
                File.Delete(d);
            }

            // recurse
            foreach (string s in subDirs)
            {
                CleanDuplicates(s);
            }
        }

        /// <summary>
        /// Upgrade to 0.9.35
        /// </summary>
        /// <remarks>
        /// This cleans up some unused files mostly related to the improved jar resolver.
        /// </remarks>
        /// <param name="prevVer">Previous ver.</param>
        private static string Upgrade935(string prevVer)
        {
            string[] obsoleteFiles =
                {
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/CocoaPodHelper.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/CocoaPodHelper.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/GPGSInstructionWindow.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/GPGSInstructionWindow.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/Podfile.txt",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/Podfile.txt.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/cocoapod_instructions",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/cocoapod_instructions.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/ios_instructions",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/ios_instructions.meta",

                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/DefaultResolver.cs",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/DefaultResolver.cs.meta",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/IResolver.cs",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/IResolver.cs.meta",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/JarResolverLib.dll",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/JarResolverLib.dll.meta",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/PlayServicesResolver.cs",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/PlayServicesResolver.cs.meta",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/ResolverVer1_1.cs",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/ResolverVer1_1.cs.meta",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/SampleDependencies.cs",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/SampleDependencies.cs.meta",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/SettingsDialog.cs",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/SettingsDialog.cs.meta",

                GPGSUtil.modifiedDir+"/Plugins/Android/play-services-plus-8.4.0.aar",
                GPGSUtil.modifiedDir+"/PlayServicesResolver/Editor/play-services-plus-8.4.0.aar.meta",

                // not an obsolete file, but delete the cache since the schema changed.
                "ProjectSettings/GoogleDependencyGooglePlayGames.xml"
            };
            foreach (string file in obsoleteFiles)
            {
                if (File.Exists(file))
                {
                    Debug.Log("Deleting obsolete file: " + file);
                    File.Delete(file);
                }
            }

            return PluginVersion.VersionKey;
        }

        /// <summary>
        /// Upgrade to 0.9.31
        /// </summary>
        /// <remarks>
        /// This cleans up some unused files.
        /// </remarks>
        /// <param name="prevVer">Previous ver.</param>
        private static string Upgrade931(string prevVer)
        {
            string[] obsoleteFiles =
                {
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/GPGSExportPackageUI.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/GPGSExportPackageUI.cs.meta"
                };
            foreach (string file in obsoleteFiles)
            {
                if (File.Exists(file))
                {
                    Debug.Log("Deleting obsolete file: " + file);
                    File.Delete(file);
                }
            }

            return PluginVersion.VersionKey;
        }

        /// <summary>
        /// Upgrade to 930 from the specified prevVer.
        /// </summary>
        /// <param name="prevVer">Previous ver.</param>
        /// <returns>the version string upgraded to.</returns>
        private static string Upgrade930(string prevVer)
        {
            Debug.Log("Upgrading from format version " + prevVer + " to " + PluginVersion.VersionKeyNativeCRM);

            // As of 930, the CRM API is handled by the Native SDK, not GmsCore.
            string[] obsoleteFiles =
            {
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Games.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Games.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/LoadPlayerStatsResultObject.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/LoadPlayerStatsResultObject.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/PlayerStats.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/PlayerStats.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/PlayerStatsObject.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/PlayerStatsObject.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/Stats.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/Stats.cs.meta",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/StatsObject.cs",
                GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Android/Gms/Games/Stats/StatsObject.cs.meta"
            };

            // only delete these if we are not version 0.9.34
            if (string.Compare(PluginVersion.VersionKey, PluginVersion.VersionKeyJNIStats,
                               System.StringComparison.Ordinal) <= 0)
            {
                foreach (string file in obsoleteFiles)
                {
                    if (File.Exists(file))
                    {
                        Debug.Log("Deleting obsolete file: " + file);
                        File.Delete(file);
                    }
                }
            }

            return PluginVersion.VersionKeyNativeCRM;
        }

        private static string Upgrade928(string prevVer)
        {
            //remove the jar resolver and if found, then
            // warn the user that restarting the editor is required.
            string[] obsoleteFiles =
                {
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/JarResolverLib.dll",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/JarResolverLib.dll.meta",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/BackgroundResolution.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/BackgroundResolution.cs.meta"
                };

            bool found = File.Exists(obsoleteFiles[0]);

            foreach (string file in obsoleteFiles)
            {
                if (File.Exists(file))
                {
                    Debug.Log("Deleting obsolete file: " + file);
                    File.Delete(file);
                }
            }

            if (found)
            {
                GPGSUtil.Alert("This update made changes that requires that you restart the editor");
            }

            Debug.Log("Upgrading from version " + prevVer + " to " + PluginVersion.VersionKeyJarResolver);
            return PluginVersion.VersionKeyJarResolver;
        }

        /// <summary>
        /// Upgrade to 0.9.27a.
        /// </summary>
        /// <remarks>This removes the GPGGizmo class, which broke the editor</remarks>
        /// <returns>The patched version</returns>
        /// <param name="prevVer">Previous version</param>
        private static string Upgrade927Patch(string prevVer)
        {
            string[] obsoleteFiles =
                {
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/GPGGizmo.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Editor/GPGGizmo.cs.meta",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/BasicApi/OnStateLoadedListener.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/BasicApi/OnStateLoadedListener.cs.meta",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Native/AndroidAppStateClient.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Native/AndroidAppStateClient.cs.meta",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Native/UnsupportedAppStateClient.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/Platforms/Native/UnsupportedAppStateClient.cs.meta"
                };
            foreach (string file in obsoleteFiles)
            {
                if (File.Exists(file))
                {
                    Debug.Log("Deleting obsolete file: " + file);
                    File.Delete(file);
                }
            }

            return PluginVersion.VersionKey27Patch;
        }

        /// <summary>
        /// Upgrade to 915 from the specified prevVer.
        /// </summary>
        /// <param name="prevVer">Previous ver.</param>
        /// <returns>the version string upgraded to.</returns>
        private static string Upgrade915(string prevVer)
        {
            Debug.Log("Upgrading from format version " + prevVer + " to " + PluginVersion.VersionKeyU5);

            // all that was done was moving the Editor files to be in GooglePlayGames/Editor
            string[] obsoleteFiles =
                {
                    GPGSUtil.modifiedDir+"/Editor/GPGSAndroidSetupUI.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSAndroidSetupUI.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSDocsUI.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSDocsUI.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSIOSSetupUI.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSIOSSetupUI.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSInstructionWindow.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSInstructionWindow.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSPostBuild.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSPostBuild.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSProjectSettings.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSProjectSettings.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSStrings.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSStrings.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSUpgrader.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSUpgrader.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GPGSUtil.cs",
                    GPGSUtil.modifiedDir+"/Editor/GPGSUtil.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/GameInfo.template",
                    GPGSUtil.modifiedDir+"/Editor/GameInfo.template.meta",
                    GPGSUtil.modifiedDir+"/Editor/PlistBuddyHelper.cs",
                    GPGSUtil.modifiedDir+"/Editor/PlistBuddyHelper.cs.meta",
                    GPGSUtil.modifiedDir+"/Editor/PostprocessBuildPlayer",
                    GPGSUtil.modifiedDir+"/Editor/PostprocessBuildPlayer.meta",
                    GPGSUtil.modifiedDir+"/Editor/ios_instructions",
                    GPGSUtil.modifiedDir+"/Editor/ios_instructions.meta",
                    GPGSUtil.modifiedDir+"/Editor/projsettings.txt",
                    GPGSUtil.modifiedDir+"/Editor/projsettings.txt.meta",
                    GPGSUtil.modifiedDir+"/Editor/template-AndroidManifest.txt",
                    GPGSUtil.modifiedDir+"/Editor/template-AndroidManifest.txt.meta",
                    GPGSUtil.modifiedDir+"/Plugins/Android/libs/armeabi/libgpg.so",
                    GPGSUtil.modifiedDir+"/Plugins/Android/libs/armeabi/libgpg.so.meta",
                    GPGSUtil.modifiedDir+"/Plugins/iOS/GPGSAppController 1.h",
                    GPGSUtil.modifiedDir+"/Plugins/iOS/GPGSAppController 1.h.meta",
                    GPGSUtil.modifiedDir+"/Plugins/iOS/GPGSAppController 1.mm",
                    GPGSUtil.modifiedDir+"/Plugins/iOS/GPGSAppController 1.mm.meta"
                };

            foreach (string file in obsoleteFiles)
            {
                if (File.Exists(file))
                {
                    Debug.Log("Deleting obsolete file: " + file);
                    File.Delete(file);
                }
            }

            return PluginVersion.VersionKeyU5;
        }

        /// <summary>
        /// Upgrade to 911 from the specified prevVer.
        /// </summary>
        /// <param name="prevVer">Previous ver.</param>
        /// <returns>the version string upgraded to.</returns>
        private static string Upgrade911(string prevVer)
        {
            Debug.Log("Upgrading from format version " + prevVer + " to " + PluginVersion.VersionKeyCPP);

            // delete obsolete files, if they are there
            string[] obsoleteFiles =
                {
                    GPGSUtil.modifiedDir+"/GooglePlayGames/OurUtils/Utils.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/OurUtils/Utils.cs.meta",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/OurUtils/MyClass.cs",
                    GPGSUtil.modifiedDir+"/GooglePlayGames/OurUtils/MyClass.cs.meta",
                    GPGSUtil.modifiedDir+"/Plugins/GPGSUtils.dll",
                    GPGSUtil.modifiedDir+"/Plugins/GPGSUtils.dll.meta",
                };

            foreach (string file in obsoleteFiles)
            {
                if (File.Exists(file))
                {
                    Debug.Log("Deleting obsolete file: " + file);
                    File.Delete(file);
                }
            }

            // delete obsolete directories, if they are there
            string[] obsoleteDirectories =
                {
                    GPGSUtil.modifiedDir+"/Plugins/Android/BaseGameUtils"
                };

            foreach (string directory in obsoleteDirectories)
            {
                if (Directory.Exists(directory))
                {
                    Debug.Log("Deleting obsolete directory: " + directory);
                    Directory.Delete(directory, true);
                }
            }

            Debug.Log("Done upgrading from format version " + prevVer + " to " + PluginVersion.VersionKeyCPP);
            return PluginVersion.VersionKeyCPP;
        }
    }
}
#endif
