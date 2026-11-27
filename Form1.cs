namespace KeyEventLogger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.KeyPress += Form1_KeyPress;
        }

        bool recording = false;

        List<string> keyEvents = new List<string>();

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (recording) 
            {
                string keyEvent = $"Key Down: {e.KeyCode}";
                keyEvents.Add(keyEvent);
                listBox1.Items.Add(keyEvent);
            }
            
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (recording)
            {
                string keyEvent = $"Key Press: {e.KeyChar}";
                keyEvents.Add(keyEvent);
                listBox1.Items.Add(keyEvent);
                e.Handled = true;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (recording)
            {
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
                    label1.Text = "Durum : Kayýtta";
                    recording = true;
                    break;
                case true:
                    button1.Text = "Baþlat";
                    label1.Text = "Durum : Boþta";
                    recording = false;
                    break;
            }
        }
    }
}
