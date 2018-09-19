
using Alphaleonis.Win32.Vss;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace ServerBackupUtility.VssService
{
    public class VssBackup : IDisposable
    {
        // This code quiesces all VSS-compatible components before making its shadow copy.
        private bool ComponentMode = false;

        // A reference to the VSS context
        private IVssBackupComponents _backup;

        // Some persistent context for the current snapshot
        private Snapshot _snapshot;

        // Constructs a VssBackup object and initializes some of the necessary VSS structures
        public VssBackup()
        {
            InitializeBackup();
        }

        // Sets up a shadow copy against the specified volume
        public void Setup(string volumeName)
        {
            Discovery(volumeName);
            PreBackup();
        }

        // This stage initializes both the requester and any writers on the system in
        // preparation for a backup, and sets up a communcation channel between the two.
        private void InitializeBackup()
        {
            // Here we are retrieving an OS-dependent object that encapsulates all of the VSS functionality.
            IVssImplementation vss = VssUtils.LoadImplementation();

            // Now we create a BackupComponents object to manage the backup.
            _backup = vss.CreateVssBackupComponents();

            // Now we must initialize the components. We can either start a fresh backup by passing null here,
            // or we could resume a previous backup operation through an earlier use of the SaveXML method.
            _backup.InitializeForBackup(null);

            // At this point, we're supposed to establish communication with the writers on the system. It is possible 
            // before this step to enable or disable specific writers via the BackupComponents' Enable* and Disable* methods.
            _backup.GatherWriterMetadata();
        }

        private void Discovery(string fullPath)
        {
            if (ComponentMode)
            {
                // In component mode, we would need to enumerate through each component
                // and decide whether it should be added to our backup document.
                ExamineComponents(fullPath);
            }
            else
            {
                // Once we are finished with the writer metadata, we can dispose of it. If we were in component mode, we would
                // want to keep it around so that we could notify the writers of our success or failure when we finish the backup.
                _backup.FreeWriterMetadata();
            }

            // Now we use our helper class to add the appropriate volume to the shadow copy set.
            _snapshot = new Snapshot(_backup);
            _snapshot.AddVolume(Path.GetPathRoot(fullPath));
        }

        private void ExamineComponents(string fullPath)
        {
            IList<IVssExamineWriterMetadata> writerMds = _backup.WriterMetadata;

            foreach (IVssExamineWriterMetadata metadata in writerMds)
            {
                Trace.WriteLine("Examining Metadata for " + metadata.WriterName);

                foreach (IVssWMComponent component in metadata.Components)
                {
                    Trace.WriteLine("  Component: " + component.ComponentName);
                    Trace.WriteLine("  Component Info: " + component.Caption);

                    foreach (VssWMFileDescriptor file in component.Files)
                    {
                        Trace.WriteLine("    Path: " + file.Path);
                        Trace.WriteLine("       Spec: " + file.FileSpecification);
                    }
                }
            }
        }

        // This phase of the backup is focused around creating the shadow copy.
        private void PreBackup()
        {
            Debug.Assert(_snapshot != null);

            // This a way to tell writers just what sort of backup they should be preparing for.
            _backup.SetBackupState(ComponentMode, true, VssBackupType.Full, false);

            // From here we just need to send messages to each writer that our snapshot is imminent.
            // We simply block while the writers to complete their background preparations.
            _backup.PrepareForBackup();

            // Each writer will have to freeze its I/O to the selected volumes for up to 10 seconds.
            _snapshot.Copy();
        }

        public string GetSnapshotPath(string localPath)
        {
            Trace.WriteLine("New Volume: " + _snapshot.Root);

            // This replaces the file's normal root information with root info from our new shadow copy.
            if (Path.IsPathRooted(localPath))
            {
                string root = Path.GetPathRoot(localPath);
                localPath = localPath.Replace(root, String.Empty);
            }

            string slash = Path.DirectorySeparatorChar.ToString();

            if (!_snapshot.Root.EndsWith(slash) && !localPath.StartsWith(slash))
            {
                localPath = localPath.Insert(0, slash);
            }

            localPath = localPath.Insert(0, _snapshot.Root);
            Trace.WriteLine("Converted Path: " + localPath);

            return localPath;
        }

        public Stream GetStream(string localPath)
        {
            return File.OpenRead(GetSnapshotPath(localPath));
        }

        // The final phase of the backup involves some cleanup steps.
        private void Complete(bool succeeded)
        {
            if (ComponentMode)
            {
                // We iterate through all of the writers on the system.
                IList<IVssExamineWriterMetadata> writers = _backup.WriterMetadata;

                foreach (IVssExamineWriterMetadata metadata in writers)
                {
                    foreach (IVssWMComponent component in metadata.Components)
                    {
                        // The BackupSucceeded call should mirror the AddComponent call that was called during the discovery phase.
                        _backup.SetBackupSucceeded(metadata.InstanceId, metadata.WriterId, component.Type, component.LogicalPath, component.ComponentName, succeeded);
                    }
                }

                // Finally, we can dispose of the writer metadata.
                _backup.FreeWriterMetadata();
            }

            try
            {
                // The BackupComplete event must be sent to all of the writers.
                _backup.BackupComplete();
            }
            catch (VssBadStateException)
            {
                // Empty
            }
        }

        private string FileToPathSpecification(VssWMFileDescriptor file)
        {
            // Environment variables (eg. "%windir%") are common.
            string path = Environment.ExpandEnvironmentVariables(file.Path);

            // Use the alternate location if it's present.
            if (!String.IsNullOrEmpty(file.AlternateLocation))
            {
                path = Environment.ExpandEnvironmentVariables(file.AlternateLocation);
            }

            // Normalize wildcard usage.
            string spec = file.FileSpecification.Replace("*.*", "*");

            // Combine the file specification and the directory name.
            return Path.Combine(path, file.FileSpecification);
        }

        // The disposal of this object involves sending completion notices to the writers, removing the shadow copies from the
        // system and finally releasing the BackupComponents object. This method must be called when this class is no longer used.
        public void Dispose()
        {
            try
            {
                Complete(true);
            }
            catch
            {
                // Empty
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_snapshot != null)
                {
                    _snapshot.Dispose();
                    _snapshot = null;
                }

                if (_backup != null)
                {
                    _backup.Dispose();
                }
            }
        }
    }
}
