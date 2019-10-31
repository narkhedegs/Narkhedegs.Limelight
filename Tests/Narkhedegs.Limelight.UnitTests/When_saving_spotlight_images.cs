using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NUnit.Framework;

namespace Narkhedegs.Limelight.UnitTests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    class When_saving_spotlight_images
    {
        private SpotlightImageSaver _spotlightImageSaver;
        private MockFileSystem _fileSystemMock;
        private Mock<IJpegFormatTester> _jpegFormatTesterMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new MockFileSystem();

            _jpegFormatTesterMock = new Mock<IJpegFormatTester>();
            _jpegFormatTesterMock.Setup(jpegFormatTester => jpegFormatTester.Test(It.IsAny<string>())).Returns(true);

            _spotlightImageSaver = new SpotlightImageSaver(_fileSystemMock, _jpegFormatTesterMock.Object);
        }

        [Test]
        public void It_should_create_limelight_image_directory_if_it_does_not_exist()
        {
            _fileSystemMock.AddDirectory(Constants.SpotlightImageDirectoryPath);

            _spotlightImageSaver.Save();

            Assert.AreEqual(true, _fileSystemMock.Directory.Exists(Constants.LimelightImageDirectoryPath));
        }

        [Test]
        public void It_should_save_all_spotlight_images_in_limelight_directory()
        {
            var today = DateTime.Now;
            var yesterday = today.AddDays(-1);
            var dayBeforeYesterday = today.AddDays(-2);

            _fileSystemMock.AddFile(Path.Combine(Constants.SpotlightImageDirectoryPath, Guid.NewGuid().ToString()),
                new MockFileData("today1")
                {
                    CreationTime = today
                });
            _fileSystemMock.AddFile(Path.Combine(Constants.SpotlightImageDirectoryPath, Guid.NewGuid().ToString()),
                new MockFileData("today2")
                {
                    CreationTime = today
                });
            _fileSystemMock.AddFile(Path.Combine(Constants.SpotlightImageDirectoryPath, Guid.NewGuid().ToString()),
                new MockFileData("yesterday1")
                {
                    CreationTime = yesterday
                });
            _fileSystemMock.AddFile(Path.Combine(Constants.SpotlightImageDirectoryPath, Guid.NewGuid().ToString()),
                new MockFileData("yesterday2")
                {
                    CreationTime = yesterday
                });
            _fileSystemMock.AddFile(Path.Combine(Constants.SpotlightImageDirectoryPath, Guid.NewGuid().ToString()),
                new MockFileData("dayBeforeYesterday1")
                {
                    CreationTime = dayBeforeYesterday
                });
            _fileSystemMock.AddFile(Path.Combine(Constants.SpotlightImageDirectoryPath, Guid.NewGuid().ToString()),
                new MockFileData("dayBeforeYesterday2")
                {
                    CreationTime = dayBeforeYesterday
                });

            _spotlightImageSaver.Save();

            var today1 = _fileSystemMock.GetFile(Path.Combine(Constants.LimelightImageDirectoryPath, $"{today:yyyy-MM-dd}-1.jpeg"));
            var today2 = _fileSystemMock.GetFile(Path.Combine(Constants.LimelightImageDirectoryPath, $"{today:yyyy-MM-dd}-2.jpeg"));
            var yesterday1 = _fileSystemMock.GetFile(Path.Combine(Constants.LimelightImageDirectoryPath, $"{yesterday:yyyy-MM-dd}-1.jpeg"));
            var yesterday2 = _fileSystemMock.GetFile(Path.Combine(Constants.LimelightImageDirectoryPath, $"{yesterday:yyyy-MM-dd}-2.jpeg"));
            var dayBeforeYesterday1 = _fileSystemMock.GetFile(Path.Combine(Constants.LimelightImageDirectoryPath, $"{dayBeforeYesterday:yyyy-MM-dd}-1.jpeg"));
            var dayBeforeYesterday2 = _fileSystemMock.GetFile(Path.Combine(Constants.LimelightImageDirectoryPath, $"{dayBeforeYesterday:yyyy-MM-dd}-2.jpeg"));

            Assert.That(today1 != null);
            Assert.AreEqual("today1", today1.TextContents);

            Assert.That(today2 != null);
            Assert.AreEqual("today2", today2.TextContents);

            Assert.That(yesterday1 != null);
            Assert.AreEqual("yesterday1", yesterday1.TextContents);

            Assert.That(yesterday2 != null);
            Assert.AreEqual("yesterday2", yesterday2.TextContents);

            Assert.That(dayBeforeYesterday1 != null);
            Assert.AreEqual("dayBeforeYesterday1", dayBeforeYesterday1.TextContents);

            Assert.That(dayBeforeYesterday2 != null);
            Assert.AreEqual("dayBeforeYesterday2", dayBeforeYesterday2.TextContents);
        }
    }
}
