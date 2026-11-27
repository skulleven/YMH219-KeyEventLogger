using System.Diagnostics;
using System.Globalization;

namespace KeyEventLogger
{
    public partial class Form1 : Form
    {
        private sealed class HeldKeyInfo
        {
            public Keys KeyCode { get; init; }
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
            public int ListBoxIndex { get; set; }
            public int KeyEventsIndex { get; init; }
            public string LastText { get; set; } = string.Empty;
        }

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.KeyPress += Form1_KeyPress;
            this.Deactivate += Form1_Deactivate;
            this.Disposed += (s, e) => holdTimer.Dispose();

            holdTimer.Interval = 50;
            holdTimer.Tick += HoldTimer_Tick;
        }

        bool recording = false;

        List<string> keyEvents = new List<string>();
        private readonly Dictionary<Keys, HeldKeyInfo> heldKeys = new Dictionary<Keys, HeldKeyInfo>();
        private readonly System.Windows.Forms.Timer holdTimer = new System.Windows.Forms.Timer();

        private static string FormatKeyDown(Keys keyCode, TimeSpan elapsed)
        {
            return $"Key Down: {keyCode} ({elapsed.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture)} s)";
        }

        private void UpdateHeldKeyEntry(HeldKeyInfo info)
        {
            string updatedText = FormatKeyDown(info.KeyCode, info.Stopwatch.Elapsed);
            if (updatedText == info.LastText)
            {
                return;
            }

            info.LastText = updatedText;
            if (info.KeyEventsIndex >= 0 && info.KeyEventsIndex < keyEvents.Count)
            {
                keyEvents[info.KeyEventsIndex] = updatedText;
            }

            if (info.ListBoxIndex >= 0 && info.ListBoxIndex < listBox1.Items.Count)
            {
                listBox1.Items[info.ListBoxIndex] = updatedText;
            }
        }

        private void StopAllHeldKeys()
        {
            holdTimer.Stop();
            foreach (var info in heldKeys.Values)
            {
                info.Stopwatch.Stop();
                UpdateHeldKeyEntry(info);
            }
            heldKeys.Clear();
        }

        private void HoldTimer_Tick(object? sender, EventArgs e)
        {
            if (!recording || heldKeys.Count == 0)
            {
                holdTimer.Stop();
                return;
            }

            foreach (var info in heldKeys.Values)
            {
                UpdateHeldKeyEntry(info);
            }
        }

        private void Form1_Deactivate(object? sender, EventArgs e)
        {
            StopAllHeldKeys();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (recording)
            {
                e.SuppressKeyPress = true;

                if (heldKeys.ContainsKey(e.KeyCode))
                {
                    return;
                }

                string keyEvent = FormatKeyDown(e.KeyCode, TimeSpan.Zero);
                keyEvents.Add(keyEvent);
                int keyEventsIndex = keyEvents.Count - 1;
                int listBoxIndex = listBox1.Items.Add(keyEvent);

                heldKeys[e.KeyCode] = new HeldKeyInfo
                {
                    KeyCode = e.KeyCode,
                    ListBoxIndex = listBoxIndex,
                    KeyEventsIndex = keyEventsIndex,
                    LastText = keyEvent
                };

                if (!holdTimer.Enabled)
                {
                    holdTimer.Start();
                }
            }
        }

        private void Form1_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (recording)
            {
                string keyEvent = $"Key Press: {e.KeyChar}";
                keyEvents.Add(keyEvent);
                listBox1.Items.Add(keyEvent);
                e.Handled = true;
            }
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (recording)
            {
                if (heldKeys.Remove(e.KeyCode, out var info))
                {
                    info.Stopwatch.Stop();
                    UpdateHeldKeyEntry(info);
                    if (heldKeys.Count == 0)
                    {
                        holdTimer.Stop();
                    }
                }

                string keyEvent = $"Key Up: {e.KeyCode}";
                keyEvents.Add(keyEvent);
                listBox1.Items.Add(keyEvent);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (recording)
            {
                case false:
                    button1.Text = "Durdur";
                    label1.Text = "Durum : Kayıtta";
                    recording = true;
                    break;
                case true:
                    StopAllHeldKeys();
                    button1.Text = "Başlat";
                    label1.Text = "Durum : Boşta";
                    recording = false;
                    break;
            }
        }

        private void SaveFile()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save Key Events";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(saveFileDialog.FileName, keyEvents);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var info in heldKeys.Values)
            {
                info.ListBoxIndex = -1;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
    }
}