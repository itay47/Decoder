using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using MetroFramework.Forms;

namespace GUI
{
    public partial class Form1 : MetroForm
    {
        private readonly ManualResetEvent iResetEvent = new ManualResetEvent(false);

        private string psPath;
        private string outputPath;
        private string cursesPath;
        private string dbPath;

        private const string BaseFolder = "Pluralsight";
        private System.Timers.Timer timer;
        private Decoder.Option.DecryptorOptions iDecryptorOptions;
        private bool FistRun = true;


        Decoder.Decryptor iDecryptor;
        public Form1()
        {
            InitializeComponent();
            iDecryptor = new Decoder.Decryptor();
            FistRun = true;
            this.timer = new System.Timers.Timer(500);
            timer.Elapsed += Timer_Elapsed;
            
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;

            this.metroTabControl1.Location = new Point(this.Width, this.metroTabControl1.Location.Y);
            this.metroPanel2.Location = new Point(this.metroPanel2.Location.X, 0);

            this.ShadowType = MetroFormShadowType.AeroShadow;

            this.psPath = GetDefaultpsPath();

            
            this.cursesPath = this.psPath + "\\courses";
            this.dbPath = this.psPath + "\\pluralsight.db";

            Transitions.Transition iTransition = new Transitions.Transition(new Transitions.TransitionType_Deceleration(1000));

            iTransition.add(this.metroTabControl1, "Left", this.Width - 777);
            iTransition.add(this.metroPanel2, "Top", this.Height - 422);
            iTransition.run();

            timer.Enabled = true;
            timer.Start();
            this.metroTextBox3.Text = dbPath;
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string title = await iDecryptor.GetCurrentCourseTitle();
            if (!string.IsNullOrEmpty(title))
            {
                int total = await iDecryptor.GetCourseCount();
                int completed = await iDecryptor.GetCourse_Completed_Decrypt();
                ChangeCurrentCourse(title, completed, total) ;
            }
        }

        private string GetDefaultpsPath()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string PsDefault = Path.Combine(new string[] { localAppData,BaseFolder});

                    
            return PsDefault;
        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            metroStyleManager1.Theme = (metroStyleManager1.Theme == MetroFramework.MetroThemeStyle.Light ? MetroFramework.MetroThemeStyle.Dark : MetroFramework.MetroThemeStyle.Light);
        }

        private void metroTextBox1_Click(object sender, EventArgs e)
        {
            using (var iDialog = new OpenFileDialog())
            {
                iDialog.InitialDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    BaseFolder
                    );

                iDialog.ValidateNames = false;
                iDialog.CheckFileExists = false;
                iDialog.CheckPathExists = true;

                iDialog.FileName = "Select Courses folder";

                if(iDialog.ShowDialog() == DialogResult.OK)
                {
                    this.cursesPath = Path.GetDirectoryName(iDialog.FileName);
                    this.metroTextBox1.Text = cursesPath;
                    this.metroTextBox3.Text = cursesPath;
                }
            }
        }

        private void metroTextBox2_Click(object sender, EventArgs e)
        {
            using (var iDialog = new OpenFileDialog())
            {
                iDialog.InitialDirectory = "C:\\";

                iDialog.ValidateNames = false;
                iDialog.CheckFileExists = false;
                iDialog.CheckPathExists = true;

                iDialog.FileName = "Select output folder";

                if (iDialog.ShowDialog() == DialogResult.OK)
                {
                    this.outputPath = Path.GetDirectoryName(iDialog.FileName);
                    this.metroTextBox2.Text = outputPath;
                }
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            metroButton1.Enabled = false;

            Invoke((MethodInvoker)(() => this.metroLabel7.Text = "Completed:"));

            if (!string.IsNullOrWhiteSpace(this.outputPath) && !string.IsNullOrWhiteSpace(this.psPath))
            {
                iDecryptorOptions = new Decoder.Option.DecryptorOptions();

                iDecryptorOptions.InputPath = this.cursesPath;
                iDecryptorOptions.DatabasePath = this.dbPath;
                iDecryptorOptions.OutputPath = this.outputPath;

                if (this.useDatabaseCheckbox.Checked)
                {
                    iDecryptorOptions.UseDatabase = true;
                }

                if (this.createSubsCheckbox.Checked)
                {
                    iDecryptorOptions.CreateTranscript = true;
                }

                if (this.removeFolderCheckbox.Checked)
                {
                    iDecryptorOptions.RemoveFolderAfterDecryption = true;
                }

                
                iDecryptor = new Decoder.Decryptor(iDecryptorOptions);

                this.metroProgressSpinner1.Visible = true;
                
                timer.Start();

                Task.Factory.StartNew(async () =>
                {

                    // task decrypt PS videos
                    await iDecryptor.DecryptAllFolders(iDecryptorOptions.InputPath, iDecryptorOptions.OutputPath);
                    
                    this.metroProgressSpinner1.Visible = false;
                    this.metroLabel5.Text = "The decryption has been completed!";
                    this.metroLabel5.Visible = true;

                    ChangeCurrentCourse("", await iDecryptor.GetCourse_Completed_Decrypt(), await iDecryptor.GetCourseCount());
                    await setTimeout( this.metroLabel5,this.metroButton1, 3000);
                    this.timer.Stop();

                }, TaskCreationOptions.LongRunning);
            }
            else
            {
                this.metroLabel5.Text = "There was an error. Check paths!";
                this.metroLabel5.Style = MetroFramework.MetroColorStyle.Red;
                this.metroLabel5.Visible = true;

                Task.Factory.StartNew(async () =>
                {
                    ChangeCurrentCourse("", await iDecryptor.GetCourse_Completed_Decrypt(), await iDecryptor.GetCourseCount());
                    await setTimeout(this.metroLabel5, this.metroButton1, 3000);
                });
            }
        }

        public static async Task setTimeout(MetroFramework.Controls.MetroLabel iLabel, MetroFramework.Controls.MetroButton iButton, long iTime)
        {
            var iTimer = new System.Timers.Timer();
            iTimer.Interval = iTime;

            iTimer.Elapsed += (iSeconds, en) =>
            {
                iLabel.Visible = false;

                iButton.Enabled = true;
            };

            await Task.Run( () => iTimer.Start());
        }

        private void metroLabel6_Click(object sender, EventArgs e)
        {

        }

        private void metroToggle2_CheckedChanged(object sender, EventArgs e)
        {
            switch (metroToggle2.Checked)
            {
                case true:
                    this.metroTextBox1.Enabled = true;
                    break;

                case false:
                    this.metroTextBox1.Enabled = false;
                    this.cursesPath = GetDefaultpsPath() + "\\courses"; ;
                    break;
            }

            this.metroTextBox3.Text = this.cursesPath;

        }

        private void metroLabel5_Click(object sender, EventArgs e)
        {

        }

        private void ChangeCurrentCourse(string title, int completed, int total)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => this.metroLabel7.Text = $"Completed: {completed}/{total}"));
                Invoke((MethodInvoker)(() => this.metroLabel5.Text = title));
            }
            else
                this.metroLabel5.Text = title;
        }

    }
}
