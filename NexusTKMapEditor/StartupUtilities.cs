﻿using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NexusTKMapEditor
{
    public static class StartupUtilities
    {
        public static ToolStripLabel lblStatus { get; set; }

        /// <summary>
        /// Creates map editor registry key (if it does not already exist).
        /// </summary>
        public static void CreateMapEditorRegistryKey()
        {
            const string mapEditorKeyString = "Software\\NexusTKMapEditor";

            RegistryKey mapEditorKey = Registry.CurrentUser.OpenSubKey(mapEditorKeyString);
            if (mapEditorKey != null && mapEditorKey.GetValue("NexusTKPath") != null && Directory.Exists(mapEditorKey.GetValue("NexusTKPath").ToString()))
            {
                mapEditorKey.Close();
                return;
            }

            mapEditorKey = Registry.CurrentUser.CreateSubKey(mapEditorKeyString);
            if (mapEditorKey != null)
            {
                mapEditorKey.SetValue("Checksum", 0);
                mapEditorKey.SetValue("NexusTKPath", GetGameInstallFolder());
                mapEditorKey.Close();
            }
        }

        /// <summary>
        /// Get location where game is installed using game registry key.
        /// </summary>
        /// <returns></returns>
        public static string GetGameInstallFolder()
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "NexusTK|NexusTK.exe|OtherTK|*.exe" };
            fileDialog.Title = "Select a NexusTK executable";
            fileDialog.InitialDirectory = "C:\\";
            DialogResult result = fileDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                CloseMapEditor();
                return null;
            }

            if (Directory.Exists(Path.GetDirectoryName(fileDialog.FileName)))
            {
                return Path.GetDirectoryName(fileDialog.FileName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts files from the folder where game is installed to the <paramref name="mapEditorFolder"/>.
        /// Does extraction only if 'NexusTK.exe' has changed, or 'Data\\SObj.tbl' is missing in <paramref name="mapEditorFolder"/>.
        /// </summary>
        /// <param name="mapEditorFolder">Folder where map editor is located.</param>
        /// <returns>True if files were already extracted or if extraction executed succesfully. False otherwise.</returns>
        public static bool ExtractFiles(string mapEditorFolder)
        {
            RegistryKey tKey = Registry.CurrentUser;
            
            tKey = tKey.OpenSubKey("Software\\NexusTKMapEditor", true);
            System.Diagnostics.Debug.Assert(tKey != null);
            
            if (string.IsNullOrEmpty(tKey.GetValue("NexusTKPath").ToString()) || !Directory.Exists(tKey.GetValue("NexusTKPath").ToString()))
            {
                CloseMapEditor();
                return false;
            }

            string exePath = tKey.GetValue("NexusTKPath").ToString() + "\\NexusTK.exe";

            // If Exe is modified, re-extract files
            if (!IsExeModified(exePath)) ExtractFilesCore(mapEditorFolder);

            // Also extract files if SObj.tbl is missing
            if (!File.Exists(mapEditorFolder + "\\Data\\SObj.tbl")) ExtractFilesCore(mapEditorFolder);

            return true;
        }

        private static void ExtractFilesCore(string mapEditorFolder)
        {
            RegistryKey tKey = Registry.CurrentUser;

            tKey = tKey.OpenSubKey("Software\\NexusTKMapEditor", true);
            System.Diagnostics.Debug.Assert(tKey != null);

            string gameInstallDataFolder = tKey.GetValue("NexusTKPath").ToString() + "\\Data";
            string mapEditorDataFolder = mapEditorFolder + "\\Data";

            if (!Directory.Exists(mapEditorDataFolder))
                Directory.CreateDirectory(mapEditorDataFolder);

            string[] files = Directory.GetFiles(gameInstallDataFolder, "tile*.dat");
            foreach (string filem in files)
            {
                Application.DoEvents();
                Stream a = new FileStream(filem, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                ArchiveInfo dat = new ArchiveInfo(a);
                foreach (string fileName in dat.FileNames)
                {
                    lblStatus.Text = @"Extracting " + fileName;
                    ArchiveInfo.File file = dat.GetFile(fileName);
                    a.Seek(file.Offset, SeekOrigin.Begin);
                    FileStream outputFileStream = new FileStream(Path.Combine(mapEditorDataFolder, fileName), FileMode.Create);
                    byte[] data = new byte[file.Size];
                    a.Read(data, 0, file.Size);
                    //for(int count=0; count<file.Size;++count) {
                    //int data=a.ReadByte();
                    //if(DataBindings==-1) break;
                    outputFileStream.Write(data, 0, file.Size);

                    //}
                    outputFileStream.Dispose();
                    //ArchiveInfo dat = new ArchiveInfo(
                }
            }
            lblStatus.Text = "";
        }

        /// <summary>
        /// Checks if exe file has been modified since the last time editor was started.
        /// </summary>
        /// <param name="exePath">Path to executable file ('NexusTK.exe').</param>
        /// <returns>Returns true if file has been modified since, false otherwise.</returns>
        private static bool IsExeModified(string exePath)
        {
            Crc32 t = new Crc32();
            RegistryKey tKey = Registry.CurrentUser;
            byte[] bytes = File.ReadAllBytes(exePath);
            uint checksum = t.ComputeChecksum(bytes);

            tKey = tKey.OpenSubKey("Software\\NexusTKMapEditor", true);
            System.Diagnostics.Debug.Assert(tKey != null);
            uint c_crc = Convert.ToUInt32(tKey.GetValue("Checksum"));
            if (c_crc != checksum)
            {
                tKey.SetValue("Checksum", checksum);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Closes map editor.
        /// Called if startup preconditions are not satisfied.
        /// </summary>
        public static void CloseMapEditor()
        {
            MessageBox.Show(@"You must install NexusTK to use this map editor!", @"Startup error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }
    }
}
