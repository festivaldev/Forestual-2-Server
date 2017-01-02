using System.Windows.Forms;

namespace Forestual2ServerCS
{
    public partial class DoubleBufferedPanel : Panel
    {
        public MainWindow.AccountState State { get; set; }

        public DoubleBufferedPanel() : base() {
            DoubleBuffered = true;
        }
    }
}
