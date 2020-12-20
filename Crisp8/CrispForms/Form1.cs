using Crisp8;
using Crisp8.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrispForms
{
    public partial class Form1 : Form
    {
        private Chip8 chip;
        Thread t;
        Keyboard kb;

        public Form1()
        {
            AllocConsole();
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            var res = openFileDialog1.ShowDialog();
            if (res != DialogResult.Cancel)
            {
                var rom = File.ReadAllBytes(openFileDialog1.FileName);
                kb = new Keyboard();
                chip = new Chip8(rom, new Screen(this.pictureBox1), new Sound(), kb);
                this.t = new Thread(new ThreadStart(chip.Start));
                t.Start();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            chip.Kill();
            GC.Collect();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // nvm don't need
        }
    }

    public class Screen : IScreen
    {
        private Bitmap renderer;
        private byte[] screen;
        private PictureBox p;

        public Screen(PictureBox p)
        {
            this.p = p;
            this.screen = new byte[64 * 32];
            this.renderer = new Bitmap(64, 32);
            Clear();
        }

        public void Clear()
        {
            screen = new byte[64 * 32];
        }

        public void Render()
        {
            for(int x = 0; x < 64; x++)
            {
                for(int y = 0; y < 32; y++)
                {
                    var i = x + (y * 64);
                    if (screen[i] == 1)
                        renderer.SetPixel(x, y, Color.White);
                    else
                        renderer.SetPixel(x, y, Color.Black);
                }
            }

            if (p.InvokeRequired)
            {
                p.Invoke(new MethodInvoker(
                delegate ()
                {
                    SmolPush();
                }));
            }
            else
            {
                SmolPush();
            }
        }

        private void SmolPush()
        {
            p.BackgroundImage = renderer;
            p.Refresh();
        }

        public bool Pixel(int x, int y)
        {
            var loc = x + (y * 64);
            // sprites loop when index is higher than screen.
            if(x > renderer.Width - 1)
            {
                return Pixel(x - (renderer.Width - 1), y);
            }
            if (y > renderer.Height - 1)
            {
                return Pixel(x, y - (renderer.Height - 1));
            }

            var old = screen[loc];
            screen[loc] ^= 1;

            return screen[loc] == 0 && old == 1;
        }
    }

    public class Sound : ISoundHandler
    {
        public void Beep()
        {
        }
    }

    public class Keyboard : IKeyboard
    {
        public bool isKeyDown(byte key)
        {
            return false;
        }

        public bool isKeyUp(byte key)
        {
            return !isKeyDown(key);
        }

        public byte waitForKeyPress()
        {
            return 0xF;
        }
    }
}
