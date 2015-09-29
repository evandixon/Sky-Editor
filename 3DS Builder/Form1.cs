using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using blz;
using CTR;
using _3DS_Builder.Properties;
using System.Threading.Tasks;

namespace _3DS_Builder
{

    public partial class Form1 : Form
    {
        public Form1()
        {

            //Code by SciresM
            InitializeComponent();
            B_Go.Enabled = false;
            CB_Logo.Items.AddRange(new[] { "Nintendo", "Distributed", "Licensed", "iQue", "iQueForSystem" });
            CB_Logo.SelectedIndex = 0;
            RecognizedGames = new Dictionary<ulong, string[]>();
            string[] lines = Resources.ResourceManager.GetString("_3dsgames").Split('\n').ToArray();
            foreach (string l in lines)
            {
                string[] vars = l.Split('	').ToArray();
                ulong titleid = Convert.ToUInt64(vars[0], 16);
                if (RecognizedGames.ContainsKey(titleid))
                {
                    char lc = RecognizedGames[titleid].ToArray()[0].ToCharArray()[3];
                    char lc2 = vars[1].ToCharArray()[3];
                    if (lc2 == 'A' || lc2 == 'E' || (lc2 == 'P' && lc == 'J')) //Prefer games in order US, PAL, JP
                    {
                        RecognizedGames[titleid] = vars.Skip(1).Take(2).ToArray();
                    }
                }
                else
                {
                    RecognizedGames.Add(titleid, vars.Skip(1).Take(2).ToArray());
                }
            }
            CHK_Card2.Checked = true;
            //End Code by SciresM



        }

        public volatile int threads = 0;

        public static Dictionary<ulong, string[]> RecognizedGames;
        internal static string LOGO_NAME;
        internal static bool Card2;

        // UI Alerts
        internal static DialogResult Alert(params string[] lines)
        {
            SystemSounds.Asterisk.Play();
            string msg = String.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        internal static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            SystemSounds.Question.Play();
            string msg = String.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Prompt", btn, MessageBoxIcon.Asterisk);
        }

        // UI Related
        private void B_SavePath_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
                TB_SavePath.Text = sfd.FileName;

            Validate_Go();
        }
        private void B_Go_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }

            string EXEFS_PATH = TB_Exefs.Text;
            string ROMFS_PATH = TB_Romfs.Text;
            string EXHEADER_PATH = TB_Exheader.Text;
            string SERIAL_TEXT = TB_Serial.Text;
            string SAVE_PATH = TB_SavePath.Text;

            Enabled = false;
            new Thread(() =>
            {
                threads++;
                SetPrebuiltBoxes(false);
                CTR_ROM.buildROM(Card2, LOGO_NAME, EXEFS_PATH, ROMFS_PATH, EXHEADER_PATH, SERIAL_TEXT, SAVE_PATH, PB_Show, RTB_Progress);
                SetPrebuiltBoxes(true);
                threads--;
            }).Start();
            Enabled = true;
        }
        private void Validate_Go()
        {
            bool enable = CTR_ROM.isValid(TB_Exefs.Text, TB_Romfs.Text, TB_Exheader.Text,
                TB_SavePath.Text, TB_Serial.Text, Card2);
            B_Go.Enabled = enable;
        }

        private void B_Romfs_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }

            if (CHK_PrebuiltRomfs.Checked)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() != DialogResult.OK) return;

                string magic;
                try
                {
                    using (BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName)))
                        magic = new string(br.ReadBytes(4).Select(c => (char)c).ToArray());
                }
                catch
                {
                    MessageBox.Show("Failed to read the provided file. Try again?");
                    return;
                }
                if (magic != "IVFC")
                {
                    MessageBox.Show("Provided file is not a valid romfs.");
                    return;
                }
                TB_Romfs.Text = ofd.FileName;
                Validate_Go();
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() != DialogResult.OK) return;

                TB_Romfs.Text = fbd.SelectedPath;
                Validate_Go();
            }
        }
        private void B_Exefs_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }
            if (CHK_PrebuiltExefs.Checked)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() != DialogResult.OK) return;

                TB_Exefs.Text = ofd.FileName;
                Validate_Go();
            }
            else
            {

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() != DialogResult.OK) return;

                string[] files = (new DirectoryInfo(fbd.SelectedPath)).GetFiles().Select(f => Path.GetFileNameWithoutExtension(f.FullName)).ToArray();
                if (((files.Contains("code") || files.Contains(".code")) && !(files.Contains(".code") && files.Contains("code"))) && files.Contains("banner") && files.Contains("icon") && files.Length < 10)
                {
                    FileInfo fi = (new DirectoryInfo(fbd.SelectedPath)).GetFiles()[Math.Max(Array.IndexOf(files, "code"), Array.IndexOf(files, ".code"))];
                    if (fi.Name == "code.bin")
                    {
                        Alert("Renaming \"code.bin\" to \".code.bin\"");
                        string newName = fi.DirectoryName + Path.DirectorySeparatorChar + ".code.bin";
                        File.Move(fi.FullName, newName);
                        fi = new FileInfo(newName);
                    }
                    if (fi.Length % 0x200 == 0)
                    {
                        if (Prompt(MessageBoxButtons.YesNo, "Detected Decompressed code.bin.", "Compress? File will be replaced. Do not build an ExeFS with an uncompressed code.bin if the Exheader doesn't specify it.") == DialogResult.Yes)
                        {
                            new Thread(() => { threads++; SetPrebuiltBoxes(false); new BLZCoder(new[] { "-en", fi.FullName }, PB_Show); SetPrebuiltBoxes(true); threads--; Alert("Compressed!"); }).Start();
                        }
                    }
                    if (files.Contains("logo"))
                    {
                        Alert("Deleting unneeded exefs logo binary.");
                        File.Delete((new DirectoryInfo(fbd.SelectedPath)).GetFiles()[Array.IndexOf(files, "logo")].FullName);
                    }
                    TB_Exefs.Text = fbd.SelectedPath;
                    Validate_Go();
                }
                else
                {
                    Alert("Your selected ExeFS is missing something essential.");
                }
            }
        }
        private void B_Exheader_Click(object sender, EventArgs e)
        {

            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                if (fi.Length < 0x800)
                {
                    Alert("Selected Exheader is too short. Correct size is 0x800 for Exheader and AccessDescriptor.");
                    return;
                }
                TB_Exheader.Text = ofd.FileName;
                Exheader exh = new Exheader(TB_Exheader.Text);
                if (RecognizedGames.ContainsKey(exh.TitleID))
                {
                    if (Prompt(MessageBoxButtons.YesNo, "Detected " + RecognizedGames[exh.TitleID][1] + ". Load Defaults?") == DialogResult.Yes)
                        TB_Serial.Text = exh.GetSerial();
                }
            }
            Validate_Go();
        }

        private void CB_Logo_SelectedIndexChanged(object sender, EventArgs e)
        {
            LOGO_NAME = CB_Logo.Text;
        }
        private void CHK_Card2_CheckedChanged(object sender, EventArgs e)
        {
            Card2 = CHK_Card2.Checked;
            if (Card2 == false)
            {
                MessageBox.Show("Note: NOR Flash (Card1) is not recommended for maximum compatibility.");
            }
            Validate_Go();
        }
        private void TB_Serial_TextChanged(object sender, EventArgs e)
        {
            TB_Serial.Text = TB_Serial.Text.ToUpper();
            Validate_Go();
        }
        private void CHK_PrebuiltRomfs_CheckedChanged(object sender, EventArgs e)
        {
            TB_Romfs.Text = string.Empty;
            Validate_Go();
        }
        private void CHK_PrebuiltExefs_CheckedChanged(object sender, EventArgs e)
        {
            TB_Exefs.Text = string.Empty;
            Validate_Go();
        }
        private void SetPrebuiltBoxes(bool en)
        {
            foreach (CheckBox c in new[] { CHK_PrebuiltExefs, CHK_PrebuiltRomfs })
            {
                if (c.InvokeRequired)
                {
                    c.Invoke(new Action(() => c.Enabled = en));
                }
                else
                    c.Enabled = en;
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //Command-line argument support added by evandixon
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 4)
            {
                string EXEFS_PATH = args[1];
                string ROMFS_PATH = args[2];
                string EXHEADER_PATH = args[3];

                Exheader exh = new Exheader(EXHEADER_PATH);
                if (RecognizedGames.ContainsKey(exh.TitleID))
                {
                    TB_Serial.Text = exh.GetSerial();
                }

                string SERIAL_TEXT = TB_Serial.Text;
                string SAVE_PATH = args[4];

                Enabled = false;

                await Task.Run(() => {
                    SetPrebuiltBoxes(false);
                    CTR_ROM.buildROM(Card2, LOGO_NAME, EXEFS_PATH, ROMFS_PATH, EXHEADER_PATH, SERIAL_TEXT, SAVE_PATH, PB_Show, RTB_Progress);
                    SetPrebuiltBoxes(true);
                });

                this.Close();
            }
        }
    }
}