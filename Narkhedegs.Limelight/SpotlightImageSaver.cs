using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Narkhedegs.Limelight
{
    public class SpotlightImageSaver : ISpotlightImageSaver
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJpegFormatTester _jpegFormatTester;

        public SpotlightImageSaver(IFileSystem fileSystem, IJpegFormatTester jpegFormatTester)
        {
            _jpegFormatTester = jpegFormatTester;
            _fileSystem = fileSystem;
        }

        public void Save()
        {
            if (!_fileSystem.Directory.Exists(Constants.LimelightImageDirectoryPath))
            {
                _fileSystem.Directory.CreateDirectory(Constants.LimelightImageDirectoryPath);
            }

            var spotlightImageFileGroups = _fileSystem.Directory.GetFiles(Constants.SpotlightImageDirectoryPath)
                .Where(filePath => _jpegFormatTester.Test(filePath))
                .Select(fileName => _fileSystem.FileInfo.FromFileName(fileName))
                .GroupBy(fileInformation => fileInformation.CreationTime.Date);

            foreach (var spotlightImageFileGroup in spotlightImageFileGroups)
            {
                for (var i = 0; i < spotlightImageFileGroup.Count(); i++)
                {
                    var spotlightImageFile = spotlightImageFileGroup.ElementAt(i);
                    var destinationPath =
                        Path.Combine(Constants.LimelightImageDirectoryPath,
                            $"{spotlightImageFileGroup.Key:yyyy-MM-dd}-{i + 1}.jpeg");

                    spotlightImageFile.CopyTo(destinationPath, true);
                }
            }
        }
    }
}