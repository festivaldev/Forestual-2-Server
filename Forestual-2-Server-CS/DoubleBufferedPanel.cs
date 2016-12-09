using System.Windows.Forms;

namespace Forestual2ServerCS
{
    public partial class DoubleBufferedPanel : Panel
    {
        public bool State { get; set; }

        public DoubleBufferedPanel() : base() {
            DoubleBuffered = true;
        }
    }
}
