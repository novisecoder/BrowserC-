using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InternetArcade.Classes
{

    public class ToolStripSpringComboBox : ToolStripComboBox
    {
        [DefaultValue(0)]
        public int RightMargin { get; set; }
        public object DataSource { get; set; }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (IsOnOverflow || Owner.Orientation == Orientation.Vertical)
            {
                return DefaultSize;
            }
            int width = Owner.DisplayRectangle.Width;

            if (Owner.OverflowButton.Visible)
            {
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;
            }
            int springBoxCount = 0;

            foreach (ToolStripItem item in Owner.Items)
            {
                if (item.IsOnOverflow) continue;
                if (item is ToolStripSpringComboBox)
                {
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                {
                    width = width - item.Width - item.Margin.Horizontal;
                }
            }

            if (springBoxCount > 1) width /= springBoxCount;
            if (width < DefaultSize.Width) width = DefaultSize.Width;
            Size size = base.GetPreferredSize(constrainingSize);
            size.Width = width;
            return size;
        }
    }
}