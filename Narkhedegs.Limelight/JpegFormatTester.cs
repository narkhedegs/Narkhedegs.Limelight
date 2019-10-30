using System;
using System.IO.Abstractions;

namespace Narkhedegs.Limelight
{
    public class JpegFormatTester : IJpegFormatTester
    {
        private readonly IFileSystem _fileSystem;

        public JpegFormatTester(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Test(string filePath)
        {
            bool isJpeg;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var fileStream = _fileSystem.File.OpenRead(filePath))
            {
                var firstByte = fileStream.ReadByte();
                var secondByte = fileStream.ReadByte();

                isJpeg = string.Equals($"{firstByte:x2}{secondByte:x2}", "ffd8",
                    StringComparison.CurrentCultureIgnoreCase);
            }

            return isJpeg;
        }
    }
}