using System;
using System.IO;
using System.Linq;
using Common.Configuration;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Common.Managers
{
    /// <summary>
    /// Work with SFTP import server
    /// </summary>
    public class FtpManager : IDisposable
    {
        private SftpClient _client;

        public FtpManager()
        {
            _client = new SftpClient(TestConfig.SftpServer, TestConfig.SftpUser, TestConfig.SftpPassword);
        }

        /// <summary>
        /// Uploads file to a specified SFTP directory
        /// </summary>
        /// <param name="sftpDirectory">SFTP directory path</param>
        /// <param name="pathFile">File name including path</param>
        public void UploadFile(string sftpDirectory, string pathFile)
        {
            try
            {
                if (!_client.IsConnected)
                {
                    _client.Connect();
                }
                try
                {
                    using (var stream = File.Open(pathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        _client.UploadFile(stream, Path.Combine(sftpDirectory + Path.GetFileName(pathFile)));
                    }
                }
                catch (Exception ex)
                {
                    throw new FileLoadException(
                        $@"Cannot upload file {pathFile} to SFTP directory '{TestConfig.SftpServer}:" + 
                        $@"{TestConfig.SftpPort}{sftpDirectory}': {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new SftpPathNotFoundException(
                    $"Cannot connect SFTP {TestConfig.SftpServer}:{TestConfig.SftpPort}. {ex.Message}");
            }
        }

        /// <summary>
        /// Checks whether file exists in specified SFTP directory
        /// </summary>
        /// <param name="sftpDirectory">SFTP directory path</param>
        /// <param name="pathFile">File name or path with file name</param>
        /// <returns>(bool) True if exists</returns>
        public bool IsFileExists(string sftpDirectory, string pathFile)
        {
            try
            {
                if (!_client.IsConnected)
                {
                    _client.Connect();
                }

                var fileList = _client.ListDirectory(sftpDirectory);
                var fileName = Path.GetFileName(pathFile);
                return fileList.Any(x => x.Name == fileName && !x.IsSymbolicLink && !x.IsDirectory);
            }
            catch (Exception ex)
            {
                throw new SftpPathNotFoundException(
                    $@"Cannot connect or get file list on SFTP directory '{TestConfig.SftpServer}{sftpDirectory}': " +
                    $"{ex.Message}");
            }
        }

        ~FtpManager()
        {
            Dispose(false);
        }

        private void Dispose(bool isDispose)
        {
            if (isDispose)
            {
                _client.Dispose();
            }

            _client = null;
        }

        /// <summary>
        /// Closes connections and disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
