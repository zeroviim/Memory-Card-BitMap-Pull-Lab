using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing_bitshifting
{
    public partial class Form1 : Form
    {
        public byte[] paletteBytes;
        public byte[] bitmapBytes;
        public Color[] colorbytes;
        public string pathToMCR = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bitmapBytes = new byte[0x80];
            BinaryReader MCR = new BinaryReader(File.Open(pathToMCR, FileMode.Open));
            MCR.BaseStream.Position = 0x2080;
            bitmapBytes = MCR.ReadBytes(0x80);
            colorbytes = new Color[16];
            /*paletteBytes = new byte[32] { 0x00, 0x00, 0x3F, 0x73, 0xBD, 0xDE, 0x7B, 0x52,
                                          0x19, 0x46, 0xD7, 0x39, 0x5F, 0x02, 0x54, 0x25,
                                          0xAB, 0x00, 0xFF, 0x7F, 0x9F, 0x73, 0x5F, 0x67,
                                          0x1F, 0x5F, 0x45, 0x7D, 0x36, 0x7F, 0xC4, 0x7A }; //big endian*/
            paletteBytes = new byte[32] { 0x00, 0x00, 0x73, 0x3F, 0xDE, 0xBD, 0x52, 0x7B, 
                                          0x46, 0x19, 0x39, 0xD7, 0x02, 0x5F, 0x25, 0x54,
                                          0x00, 0xAB, 0x7F, 0xFF, 0x73, 0x9F, 0x67, 0x5F,
                                          0x5F, 0x1F, 0x7D, 0x45, 0x7F, 0x36, 0x7A, 0xC4 }; //little endian straight
            //Array.Reverse(paletteBytes, 0, 32);
            GeneratePaletteArray(paletteBytes);
            lbl_color0.BackColor = colorbytes[0];
            lbl_Color1.BackColor = colorbytes[1];
            lbl_Color2.BackColor = colorbytes[2];
            lbl_Color3.BackColor = colorbytes[3];
            lbl_Color4.BackColor = colorbytes[4];
            lbl_Color5.BackColor = colorbytes[5];
            lbl_Color6.BackColor = colorbytes[6];
            lbl_Color7.BackColor = colorbytes[7];
            lbl_Color8.BackColor = colorbytes[8];
            lbl_Color9.BackColor = colorbytes[9];
            lbl_Color10.BackColor = colorbytes[10];
            lbl_Color11.BackColor = colorbytes[11];
            lbl_Color12.BackColor = colorbytes[12];
            lbl_Color13.BackColor = colorbytes[13];
            lbl_Color14.BackColor = colorbytes[14];
            lbl_Color15.BackColor = colorbytes[15];
            GenerateBitmap();
        }

        private void GenerateBitmap()
        {
            Bitmap bitmapFrameOutput = new Bitmap(16, 16);
            int bitmapCount = 0;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x+= 2)
                {
                    byte leftNybble = (byte)(bitmapBytes[bitmapCount] >> 4);
                    byte rightNybble = (byte)(bitmapBytes[bitmapCount] << 4);
                    rightNybble = (byte)(rightNybble >> 4);
                    bitmapFrameOutput.SetPixel(x, y, colorbytes[rightNybble]);
                    bitmapFrameOutput.SetPixel(x+1, y, colorbytes[leftNybble]);
                    bitmapCount++;
                }
            }
            pcbx_Frame1.Image = bitmapFrameOutput;
            pcbx_Frame1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void GeneratePaletteArray(byte[] paletteBytes)
        {
            byte red5Bit, green5Bit, blue5Bit = 0;
            int red8Bit, green8Bit, blue8Bit = 0;
            byte tempByte1, tempByte2 = 0;
            int colorByteCounter = 0;
            for (int i = 0; i < paletteBytes.Length; i += 2)
            {
                byte byte1 = paletteBytes[i];
                byte byte2 = paletteBytes[i+1];
                red5Bit = 0;
                green5Bit = 0;
                blue5Bit = 0;
                red8Bit = 0;
                green8Bit = 0;
                blue8Bit = 0;
                tempByte1 = 0;
                tempByte2 = 0;
                if (colorByteCounter == 1)
                {
                    tempByte1 = 0;
                }
                tempByte1 = (byte)(byte1 << 1);
                tempByte1 = (byte)(tempByte1 >> 3);
                red5Bit = tempByte1;

                tempByte1 = (byte)(byte1 << 6);
                tempByte1 = (byte)(tempByte1 >> 3);
                tempByte2 = (byte)(byte2 >> 5);
                green5Bit = (byte)(tempByte1 | tempByte2);

                tempByte1 = (byte)(byte2 << 3);
                tempByte1 = (byte)(tempByte1 >> 3);
                blue5Bit = tempByte1;

                /*
                red5Bit = (byte)(byte1 >> 3);

                tempByte1 = (byte)(byte1 << 5);
                tempByte1 = (byte)(tempByte1 >> 3);
                tempByte2 = (byte)(byte2 >> 6);
                green5Bit = (byte)(tempByte1 | tempByte2);

                tempByte1 = (byte)(byte2 << 2);
                tempByte1 = (byte)(tempByte1 >> 3);
                blue5Bit = tempByte1;*/

                red8Bit = (red5Bit * 255) / 31;
                green8Bit = (green5Bit * 255) / 31;
                blue8Bit = (blue5Bit * 255) / 31;

                Color color = new Color();
                //color = Color.FromArgb(blue8Bit, red8Bit, green8Bit);
                color = Color.FromArgb(blue8Bit, green8Bit, red8Bit);
                //color = Color.FromArgb(red8Bit, green8Bit, blue8Bit);
                //color = Color.FromArgb(red8Bit, blue8Bit, green8Bit);
                colorbytes[colorByteCounter] = color;
                colorByteCounter++;
                /*MessageBox.Show(@"Original Bytes:" + paletteBytes[i].ToString("X2") + " | " + paletteBytes[i + 1].ToString("X2") + "\r\n" +
                                "Original Byte Binary Readout: " + Convert.ToString(Math.Abs(paletteBytes[i]), 2) + " | " + Convert.ToString(Math.Abs(paletteBytes[i+1]), 2) + "\r\n" +
                                "Binary Conversions: " + Convert.ToString(Math.Abs(red5Bit), 2) + " | " + Convert.ToString(Math.Abs(green5Bit), 2) + " | " + Convert.ToString(Math.Abs(blue5Bit), 2) + "\r\n" +
                                "Converted: " + red8Bit.ToString() + " | " + green8Bit.ToString() + " | " + blue8Bit.ToString());*/
            }
        }
    }
}
