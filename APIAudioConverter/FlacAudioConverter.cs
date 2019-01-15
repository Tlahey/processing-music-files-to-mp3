using BigMansStuff.NAudio.FLAC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAudioConverter
{
    public class FlacAudioConverter : IAudioConverter
    {
        public void ConvertToMp3ffmpeg(string file, int bitrate = 128)
        {
            /*
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            
            startInfo.Arguments = string.Format("ffmpeg -i {0} -ab {2}k -ac 2 {1}.mp3", file, );
            process.StartInfo = startInfo;
            process.Start();
            */
            string strCmdText = string.Format("-i \"{0}.flac\" -ab {1}k -ac 2 \"{0}.mp3\"", file, bitrate.ToString());
            ProcessStartInfo info = new ProcessStartInfo("ffmpeg.exe");
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            info.Arguments = strCmdText;
            var process = Process.Start(info);
            process.WaitForExit();
        }

        public byte[] ConvertToMp3(string wavFile, int bitrate)
        {
            throw new NotImplementedException();
        }
    }
}
