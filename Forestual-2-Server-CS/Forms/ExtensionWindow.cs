using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forestual2ServerCS.Forms
{
    public partial class ExtensionWindow : Form
    {
        public ExtensionWindow() {
            InitializeComponent();

            dbpContainer.Paint += OnDbpContainerPaint;
        }

        private void OnDbpContainerPaint(object sender, PaintEventArgs e) {
            var Rect = new Rectangle(0, 0, dbpContainer.Width, 40);
            e.Graphics.FillRectangle(new SolidBrush(Color.WhiteSmoke), Rect);
            Rect.Offset(7, 0);
            e.Graphics.DrawString("MazeRunnerExtension.dll", new Font("Segoe UI", 10f), new SolidBrush(Color.Black), Rect, new StringFormat { LineAlignment = StringAlignment.Center });
        }
    }
}
