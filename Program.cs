using System;
using System.Diagnostics;
using System.IO;

namespace BatchAppWithFfmpegProcess
{
    internal class Program
    {

        static void Main(string[] args)
        {

#if DEBUG
            string workingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
#else
           /*
                Within each Tasks directory, the Batch service creates a working directory (wd) whose unique path is 
                specified by the AZ_BATCH_TASK_WORKING_DIR environment variable. This directory provides read/write access 
                to the task. The task can create, read, update, and delete files under this directory. 
                This directory is retained based on the RetentionTime constraint that is specified for the task.
                The stdout.txt and stderr.txt files are written to the Tasks folder during the execution of the task.
             */
            string workingDirectory = Environment.GetEnvironmentVariable("AZ_BATCH_TASK_WORKING_DIR");
#endif
            var applicationPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string ffmpegPath = Path.Combine(applicationPath, @"lib\");

            string videoFilePath = Path.Combine(applicationPath, "MichaelJacksonSmoothCriminal_Trimmed.mp4");
            string thumbnailFilePath = Path.Combine(workingDirectory, "MichaelJacksonSmoothCriminal_Trimmed.jpg");

            if (File.Exists(thumbnailFilePath))
            {
                File.Delete(thumbnailFilePath);
            }

            string arguments =  $"-i \"{videoFilePath}\" -ss 00:00:01.000 -frames:v 1 \"{thumbnailFilePath}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath + $"\\ffmpeg.exe",
                Arguments = arguments,
                RedirectStandardError = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = ffmpegPath
            };

            using (var process = new Process { StartInfo = startInfo })
            {

                process.Start();
                process.WaitForExit();
            }
            if (File.Exists(thumbnailFilePath))
            {
                Console.WriteLine("Hurray, it worked!!!");
            }
            else
            {
                Console.WriteLine("File was not created.");
            }
        }

    }
}
