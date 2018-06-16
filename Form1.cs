using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace DragNDropSaver
{
    public partial class Form1 : Form
    {
        private ulong NextImageCount = 1;
        private string NextImageName { get { return "image1 (" + NextImageCount + ").jpg"; } }

        public Form1()
        {
            InitializeComponent();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;

            return null;
        }

        private void IncrementImageCount()
        {
            ++NextImageCount;
            nextImage.Text = NextImageName;
        }
        
        private void SetFolder(string path)
        {
            if (!Directory.Exists(path))
                return;

            textBox1.Text = path;

            NextImageCount = 0;
            IncrementImageCount();
            foreach (string file in Directory.EnumerateFiles(path, "image1*.jpg"))
                IncrementImageCount();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string draggedFilePath = ((string[])e.Data.GetData("FileDrop"))[0];
            Image bmp = Image.FromFile(draggedFilePath);

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp.Save(textBox1.Text + @"\" + NextImageName, jgpEncoder, myEncoderParameters);

            IncrementImageCount();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetFolder(Directory.GetCurrentDirectory());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res = folderBrowserDialog1.ShowDialog();
            if (res == DialogResult.OK)
                SetFolder(folderBrowserDialog1.SelectedPath);
        }
    }
}
