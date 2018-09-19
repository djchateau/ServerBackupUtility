
using Alphaleonis.Win32.Vss;
using ServerBackupUtility.Logging;
using System;

namespace ServerBackupUtility.VssService
{
    // Utility class to manage the snapshot's contents and Id.
    public class Snapshot : IDisposable
    {
        // A reference to the VSS context
        private IVssBackupComponents _backup;

        // Metadata about this object's snapshot
        private VssSnapshotProperties _properties;

        // Identifier for the overall shadow copy
        private Guid _snapshotSetId;

        // Identifier for our single snapshot volume
        private Guid _snapshotVolumeId;

        // Save the GUID of this snapshot in order to refer to it elsewhere in the class.
        public Snapshot(IVssBackupComponents backup)
        {
            _backup = backup;
            _snapshotSetId = backup.StartSnapshotSet();
        }

        // Gets the string that identifies the root of this snapshot
        public string Root
        {
            get
            {
                if (_properties == null)
                {
                    _properties = _backup.GetSnapshotProperties(_snapshotVolumeId);
                }

                return _properties.SnapshotDeviceObject;
            }
        }

        // Add a volume to the current snapshot.
        public void AddVolume(string volumeName)
        {
            if (_backup.IsVolumeSupported(volumeName))
            {
                _snapshotVolumeId = _backup.AddToSnapshotSet(volumeName);
            }
            else
            {
                LogService.LogEvent("Error: Snapshot.AddVolume - Vss Volume Not Supported");
            }
        }

        // Create the actual snapshot. This process can take around 10 seconds.
        public void Copy()
        {
            _backup.DoSnapshotSet();
        }

        // Remove all snapshots.
        public void DeleteSnapshots()
        {
            _backup.DeleteSnapshotSet(_snapshotSetId, false);
        }

        public void Dispose()
        {
            try
            {
                DeleteSnapshots();
            }
            catch (VssDeleteSnapshotsFailedException ex)
            {
                LogService.LogEvent("Error: Snapshot.Dispose - " + ex.Message);
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_backup != null)
                {
                    _backup.Dispose();
                }
            }
        }
    }
}
