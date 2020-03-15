using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentInspectorSvc
{
    public partial class StudentInspectorSvc : ServiceBase
    {
        private System.Timers.Timer timer = null;
        private int period = 30;
        private int max = 2880;
        private string captureFolder = "c:\\screenshots";


        public StudentInspectorSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Load configuration
            captureFolder = "c:\\screenshots";
            if (!System.IO.Directory.Exists(captureFolder))
            {
                System.IO.Directory.CreateDirectory(captureFolder);
            }
            period = 30;
            max = 2880;

            // Create timer and launch
            timer = new System.Timers.Timer(period * 1000);
            timer.Elapsed += TakeScreentShot;
            timer.AutoReset = true;
            timer.Start();
        }

        private void TakeScreentShot(object sender, System.Timers.ElapsedEventArgs e)
        {
            string imgname = captureFolder + "\\" + DateTime.Now.Ticks.ToString() + ".png";
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                bmp.Save(imgname);  // saves the image
            }
        }

        protected override void OnStop()
        {
            timer.Stop();
        }
    }
}
