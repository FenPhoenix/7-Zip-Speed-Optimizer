using System.Drawing;
using System.Windows.Forms;

namespace SevenZipSpeedOptimizer;

public sealed class StandardButton : Button
{
    public StandardButton()
    {
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        MinimumSize = new Size(75, 23);
        Padding = new Padding(6, 0, 6, 0);
    }
}
