using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using NUnit.Framework;

namespace Narkhedegs.Limelight.UnitTests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    class When_testing_for_jpeg_format
    {
        private JpegFormatTester _jpegFormatTester;
        private MockFileSystem _fileSystemMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new MockFileSystem();

            _jpegFormatTester = new JpegFormatTester(_fileSystemMock);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [Test]
        public void It_should_throw_ArgumentNullException_if_filePath_parameter_is_null_or_empty_or_whitespace(string filePath)
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _jpegFormatTester.Test(filePath));
            Assert.AreEqual(exception.ParamName, "filePath");
        }

        [Test]
        public void It_should_return_true_if_file_is_in_jpeg_format()
        {
            var filePath = @"C:\test";
            _fileSystemMock.AddFile(filePath, new MockFileData(HexToBytes("FFD8")));

            Assert.AreEqual(_jpegFormatTester.Test(filePath), true);
        }        
        
        [Test]
        public void It_should_return_false_if_file_is_not_in_jpeg_format()
        {
            var filePath = @"C:\test";
            _fileSystemMock.AddFile(filePath, new MockFileData(HexToBytes("424D")));

            Assert.AreEqual(_jpegFormatTester.Test(filePath), false);
        }

        private byte[] HexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}