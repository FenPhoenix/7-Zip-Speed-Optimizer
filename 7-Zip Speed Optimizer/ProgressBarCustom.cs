using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SevenZipSpeedOptimizer;

public sealed class ProgressBarCustom : ProgressBar
{
    /// <summary>
    /// Sets the progress bar's value instantly. Avoids the la-dee-dah catch-up-when-I-feel-like-it nature of
    /// the progress bar that makes it look annoying and unprofessional.
    /// </summary>
    [PublicAPI]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public new int Value
    {
        get => base.Value;
        set
        {
            value = value.Clamp(Minimum, Maximum);

            if (value == Maximum)
            {
                base.Value = Maximum;
            }
            else
            {
                base.Value = (value + 1).Clamp(Minimum, Maximum);
                base.Value = value;
            }
        }
    }
}
