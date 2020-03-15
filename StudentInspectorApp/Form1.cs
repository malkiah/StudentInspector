using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentInspectorApp
{
    public partial class StudentInspectorForm : Form
    {
        private System.Timers.Timer timer = null;
        private int period = 5;
        private int max = 2880;
        private string dataFolder = null;
        private string dataPath = null;
        private SQLiteConnection conn = null;
        private Random random = new Random();

        public StudentInspectorForm()
        {
            InitializeComponent();
        }

        private void StudentInspectorForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
            CreateTimer();
            LoadData();
        }

        private void LoadData()
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name FROM images ORDER BY id DESC";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ImageEntry entry = new ImageEntry
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        lstItems.Items.Add(entry);
                    }
                    txtNum.Text = lstItems.Items.Count.ToString();
                }
            }

        }

        private void CreateTimer()
        {
            // Create timer and launch
            timer = new System.Timers.Timer((random.Next(period) + 1) * 1000);
            timer.Elapsed += TakeScreenShot;
            timer.AutoReset = true;
            timer.Start();
        }

        private void LoadConfiguration()
        {
            // Load configuration
            dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "StudentInspector\\data";
            dataPath = dataFolder + "\\data.db";
            string cs = @"URI=file:" + dataPath;
            if (!System.IO.Directory.Exists(dataFolder))
            {
                System.IO.Directory.CreateDirectory(dataFolder);
            }
            if (!System.IO.File.Exists(dataPath))
            {
                conn = new SQLiteConnection(cs);
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE images (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, data TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                conn = new SQLiteConnection(cs);
                conn.Open();
            }
            period = 60;
            max = 1500;
        }

        private void UpdateNewEntry(ImageEntry entry)
        {
            lstItems.Items.Insert(0, entry);
            if (lstItems.Items.Count > max)
            {
                int removenum = lstItems.Items.Count - max;
                for (int i = 0; i < removenum; i++)
                {
                    ImageEntry rentry = lstItems.Items[lstItems.Items.Count - 1] as ImageEntry;
                    lstItems.Items.RemoveAt(lstItems.Items.Count - 1);
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM images WHERE id = @0";
                        SQLiteParameter pid = new SQLiteParameter("@0", rentry.ID);
                        cmd.Parameters.Add(pid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            txtNum.Text = lstItems.Items.Count.ToString();
        }

        private void TakeScreenShot(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                using (Bitmap bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        try
                        {
                            g.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size);
                            ImageEntry entry = SaveBitmap(bmp, screen);
                            if (entry != null)
                            {
                                this.Invoke((MethodInvoker)(() => UpdateNewEntry(entry)));
                            }
                        }
                        catch
                        { }
                    }
                }
            }
            timer.Interval = (random.Next(period) + 1) * 1000;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private ImageEntry SaveBitmap(Bitmap bmp, Screen screen)
        {
            ImageEntry result = null;
            ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
            using (EncoderParameters p = GetEncoderParameters())
            {
                //string path = dataFolder + "\\" + DateTime.Now.Ticks.ToString() + ".jpg";

                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    bmp.Save(stream, ici, p);
                    //bmp.Save(path, ici, p);
                    string base64data = Convert.ToBase64String(stream.ToArray());
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO images (name, data) VALUES(@0,@1)";
                        string name = DateTime.Now.ToString() + " - " + screen.DeviceName;
                        SQLiteParameter pname = new SQLiteParameter("@0", name);
                        SQLiteParameter pdata = new SQLiteParameter("@1", base64data);
                        cmd.Parameters.Add(pname);
                        cmd.Parameters.Add(pdata);
                        cmd.ExecuteNonQuery();
                        result = new ImageEntry() {
                            ID = conn.LastInsertRowId,
                            Name = name
                        };
                    }
                }
            }

            return result;

        }

        private static EncoderParameters GetEncoderParameters()
        {
            EncoderParameters p = new EncoderParameters(1);
            p.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 15L);
            return p;
        }

        private void StudentInspectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void studentInspectorNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Show();
                this.WindowState = FormWindowState.Maximized;
                studentInspectorNotifyIcon.Visible = true;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                studentInspectorNotifyIcon.Visible = true;
                Hide();
            }
        }

        private void StudentInspectorForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                studentInspectorNotifyIcon.Visible = true;
                Hide();
            }

        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstItems.SelectedItem is ImageEntry entry)
            {
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM images WHERE id = @0";
                    SQLiteParameter pid = new SQLiteParameter("@0", entry.ID);
                    cmd.Parameters.Add(pid);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string data = reader.GetString(0);
                            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(Convert.FromBase64String(data)))
                            {
                                if (pbImage.Image != null)
                                {
                                    pbImage.Image.Dispose();
                                }
                                pbImage.Image = Bitmap.FromStream(stream);
                            }
                        }
                    }
                }
            }
        }

        private void StudentInspectorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;
            conn.Close();
            conn.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Student Inspector Software
(c) Jorge García Ochoa de Aspuru
Maristak Bilbao","Acerca de...");
        }

        private void btnOcultar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            studentInspectorNotifyIcon.Visible = true;
            Hide();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            ConfigForm config = new ConfigForm();
            config.ShowDialog(this);
        }
    }
}
