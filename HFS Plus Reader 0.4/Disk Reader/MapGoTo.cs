using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Disk_Reader
{
    public partial class MapGoTo : Form
    {
        public delegate void GoToEventHandler(object sender, GoToEventArgs a);
        public event GoToEventHandler GoTo;

        public MapGoTo()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.GoToBox_Hex.Select();
        }

        private void GoToBox_Go_Click(object sender, EventArgs e)
        {
            long input = 0;
            if (GoToBox_Decimal.Checked)
            {
                try
                {
                    input = Int64.Parse(GoToBox_TextBox.Text);

                    DoGoToEvent(new GoToEventArgs(input));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Input was not a valid number.");
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Number too large");
                }
            }
            else
            {
                try
                {
                    input = Int64.Parse(GoToBox_TextBox.Text, System.Globalization.NumberStyles.AllowHexSpecifier);

                    DoGoToEvent(new GoToEventArgs(input));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Input was not a valid number.");
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Number too large");
                }
            }
        }

        protected virtual void DoGoToEvent(GoToEventArgs a)
        {
            if (GoTo != null)
            {
                GoTo(this, a);

                this.Close();
            }
        }
    }
}
