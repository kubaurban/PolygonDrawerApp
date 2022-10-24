using System.Windows.Forms;

namespace Project_1.Views
{
    public partial class LengthInputDialog : Form
    {
        public float InputLength
        {
            get
            {
                try
                {
                    return float.Parse(Input.Text);
                }
                catch { return -1; }
            }
        }

        public LengthInputDialog(float initValue)
        {
            InitializeComponent();

            Input.Text = initValue.ToString();
        }
    }
}
