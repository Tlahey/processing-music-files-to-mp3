﻿using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAudioConverter
{
    public class WmaAudioConverter : IAudioConverter
    {
        public byte[] ConvertToMp3(string file, int bitrate = 128)
        {
            using (var retMs = new MemoryStream())
            using (var rdr = new WMAFileReader(file))
            using (var wtr = new NAudio.Lame.LameMP3FileWriter(retMs, rdr.WaveFormat, bitrate))
            {
                rdr.CopyTo(wtr);
                return retMs.ToArray();
            }
        }


        public void ConvertToMp3ffmpeg(string file, int bitrate)
        {
            throw new NotImplementedException();
        }
    }
}
