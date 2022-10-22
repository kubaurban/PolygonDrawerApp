using System.Windows.Forms;

namespace Project_1.Views
{
    public partial class LengthInputDialog : Form
    {
        public int InputLength
        {
            get
            {
                try
                {
                    return int.Parse(Input.Text);
                }
                catch { return -1; }
            }
        }

        public LengthInputDialog(int initValue)
        {
            InitializeComponent();

            Input.Text = initValue.ToString();
        }
    }
}
