using System.IO;
using System.IO.Compression;

namespace TempExtract
{
    public class ZipProcessor
    {
        private readonly string _File;
        private readonly string _BaseName; // pre-compute this since it's used multiple times.

        public ZipProcessor(string file)
        {
            _File = file;

            // we don't need to do any tests for illegal characters on the basename
            // since it's likely to be coming from a filesystem that's equally or
            // more restrictive (like fat32 -> ext4). i don't think that there'd be
            // a use case where the system temporary directory is stored on a more
            // restrictive filesystem than the source file name
            _BaseName = Path.GetFileNameWithoutExtension(file);
        }

        /// <summary>
        /// Extracts the zip to a temporary directory.
        /// </summary>
        /// <returns>The directory to which the zip was extracted.</returns>
        public string Extract()
        {
            var archive = ZipFile.OpenRead(_File);
            string outputDirectory = _CreateTempPath();


            if (archive.Entries.Count <= 1)
            {
                // if there's only one entry in the zip file, just place it directly
                // in the output directory. this also handles the case if the zip
                // is empty for some reason
                archive.ExtractToDirectory(outputDirectory);
            }
            else
            {
                // if there's more than one entry, put them all into a new directory
                // to make it easier to copy the entire output as a unit
                string subdirectory = Path.Combine(outputDirectory, _BaseName);
                Directory.CreateDirectory(subdirectory);
                archive.ExtractToDirectory(subdirectory);
            }

            return outputDirectory;
        }

        private string _CreateTempPath()
        {
            // create the initial output directory name based on the name of
            // the zip file

            string parentDirectory = Path.Combine(Path.GetTempPath(), "TempExtract");
            string directoryName = _BaseName;

            // append a suffix to get a new directory name, in the case that
            // the directory already exists

            int suffix = 1;
            while (Directory.Exists(Path.Combine(parentDirectory, directoryName)))
            {
                directoryName = $"{_BaseName}_{suffix}";
            }

            return Path.Combine(parentDirectory, directoryName);
        }
    }
}