using System;
using System.IO;

namespace Narkhedegs.Limelight
{
    public static class Constants
    {
        public static readonly string SpotlightImageDirectoryPath = Environment.ExpandEnvironmentVariables(
            @"%UserProfile%\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");

        public static readonly string LimelightImageDirectoryPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Limelight");
    }
}
