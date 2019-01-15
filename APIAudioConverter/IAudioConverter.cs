using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAudioConverter
{
    public interface IAudioConverter
    {
        byte[] ConvertToMp3(string wavFile, int bitrate);
        void ConvertToMp3ffmpeg(string file, int bitrate);
    }
}
