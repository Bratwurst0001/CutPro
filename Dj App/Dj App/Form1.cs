using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace Dj_App
{
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll", EntryPoint = "waveOutSetVolume")]
        public static extern int WaveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("Winmm.dll", SetLastError = true)]
        static extern int mciSendString(string lpszCommand, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszReturnString, int cchReturn, IntPtr hwndCallback);
        public static String MainPath = Directory.GetCurrentDirectory();
        public static String Music = "\\Music";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("MeinPath: " + MainPath + Music);
            onload();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //deck1

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //deck2

        }

        private void beginnStream_Click(object sender, EventArgs e)
        {
            SaveMP3(MainPath + Music,YTURL.Text,"NIX");
        }

        private void SaveMP3(string SaveToFolder, string VideoURL, string MP3Name)
        {
            var source = @SaveToFolder;
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(VideoURL);
            MessageBox.Show("Save to: " + source + "\\" + "\"" + vid.FullName + "\"");
            try
            {
                File.WriteAllBytes(source + "\\" + "Convert.mp4", vid.GetBytes());
            }
            catch(Exception e)
            {

            }

            var inputFile = new MediaFile { Filename = source +"\\"+ "Convert.mp4" };
            Random random = new Random();
            var outputFile = new MediaFile { Filename = MainPath + Music + "\\" + vid.FullName.Replace(".mp4",".mp3") };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
                File.Delete(source + "\\" + "Convert.mp4");
                ConvertMp3ToWav(MainPath + Music + "\\" + vid.FullName.Replace(".mp4", ".mp3"), MainPath + Music + "\\" + vid.FullName.Replace(".mp4", ".wav"));
                File.Delete(MainPath + Music + "\\" + vid.FullName.Replace(".mp4", ".mp3"));
            }
            
            onload();
        }

        private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        {
            using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(_outPath_, pcm);
                }
            }
        }
        public void onload()
        {
            if (Directory.Exists(MainPath + Music))
            {
                //load Music
                DirectoryInfo di = new DirectoryInfo(MainPath + Music);
                FileInfo[] files = di.GetFiles("*.wav");
                string str = "";
                foreach (FileInfo file in files)
                {
                    if (!Deck1Songs.Items.Contains(file.Name))
                    {
                        Deck1Songs.Items.Add(file.Name);
                    }
                    if (!Deck2Songs.Items.Contains(file.Name))
                    {
                        Deck2Songs.Items.Add(file.Name);
                    }
                }
            }
            else
            {
                //Create Music Folder
                Directory.CreateDirectory(MainPath + Music);
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            string selected = Deck1Songs.SelectedItem.ToString();
            String ReplacedPath = MainPath + Music + "\\" + selected;

            StringBuilder sb = new StringBuilder();
            string sFileName = ReplacedPath;
            string sAliasName = "MP3";
            int nRet = mciSendString("open \"" + sFileName + "\" alias " + sAliasName, sb, 0, IntPtr.Zero);
            nRet = mciSendString("play " + sAliasName, sb, 0, IntPtr.Zero);
        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            SetVol(guna2TrackBar1.Value);
        }
        private void SetVol(double arg)
        {
            double newVolume = ushort.MaxValue * arg / 10.0;

            uint v = ((uint)newVolume) & 0xffff;
            uint vAll = v | (v << 16);

            int retVal = WaveOutSetVolume(IntPtr.Zero, vAll);
        }

        private void pictureBox4_Click_1(object sender, EventArgs e)
        {
            string selected = Deck2Songs.SelectedItem.ToString();
            String ReplacedPath = MainPath + Music + "\\" + selected;

            StringBuilder sb = new StringBuilder();
            string sFileName = ReplacedPath;
            string sAliasName = "MP3";
            int nRet = mciSendString("open \"" + sFileName + "\" alias " + sAliasName, sb, 0, IntPtr.Zero);
            nRet = mciSendString("play " + sAliasName, sb, 0, IntPtr.Zero);
        }

        private void guna2TrackBar2_Scroll(object sender, ScrollEventArgs e)
        {
            SetVol(guna2TrackBar2.Value);
        }

        private void Deck1Songs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //deck1 song
            Song1.Text = Deck1Songs.SelectedItem.ToString();
            BPMDetector BPMDetector = new BPMDetector(MainPath + Music + "\\" + Deck1Songs.SelectedItem.ToString());
            BPM1.Text = "" + BPMDetector.getBPM();
        }

        private void Deck2Songs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Deck2 song
            Song2.Text = Deck2Songs.SelectedItem.ToString();
            BPMDetector BPMDetector = new BPMDetector(MainPath + Music + "\\" + Deck2Songs.SelectedItem.ToString());
            BPM2.Text = "" + BPMDetector.getBPM();
        }
    }
}
