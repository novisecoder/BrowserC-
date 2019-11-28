using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static InternetArcade.TabForm;
using NativeMethods = InternetArcade.TabForm.NativeMethods;

namespace InternetArcade
{
    #region Tab Dragging

    internal class TabDragger
    {
        public TabDragger(CustomTabControl tabControl) : base()
        {
            this.tabControl = tabControl;
            tabControl.MouseDown += new MouseEventHandler(tabControl_MouseDown);
            tabControl.MouseMove += new MouseEventHandler(tabControl_MouseMove);
        }

        public TabDragger(CustomTabControl tabControl, TabDragBehavior behavior) : this(tabControl)
        {
            this.dragBehavior = behavior;
        }
        private CustomTabControl tabControl;
        private TabPage dragTab = null;
        private TabDragBehavior dragBehavior = TabDragBehavior.TabDragArrange;
        private TabDragBehavior DragBehavior
        {
            get
            {
                if (!tabControl.Multiline) return dragBehavior;
                return TabDragBehavior.None;
            }
        }
        private void tabControl_MouseDown(object sender, MouseEventArgs e)
        {
            dragTab = TabUnderMouse();
        }
        private void tabControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (DragBehavior == TabDragBehavior.None)
                return;
            if (e.Button == MouseButtons.Left)
            {
                if (dragTab != null)
                {
                    if (tabControl.TabPages.Contains(dragTab))
                    {
                        if (PointInTabStrip(e.Location))
                        {
                            TabPage hotTab = TabUnderMouse();
                            if (hotTab != dragTab && hotTab != null)
                            {
                                int id1 = tabControl.TabPages.IndexOf(dragTab);
                                int id2 = tabControl.TabPages.IndexOf(hotTab);
                                if (id1 > id2)
                                {
                                    for (int id = id2; id <= id1; id++)
                                    {
                                        SwapTabPages(id1, id);
                                    }
                                }
                                else
                                {
                                    for (int id = id2; id > id1; id--)
                                    {
                                        SwapTabPages(id1, id);
                                    }
                                }
                                tabControl.SelectedTab = dragTab;
                            }
                        }
                        else
                        {
                            if (this.dragBehavior == TabDragBehavior.TabDragOut)
                            {
                                if (dragTab.Tag != null)
                                {
                                    ((TabForm)dragTab.Tag).Dispose();
                                    dragTab.Tag = null;
                                }
                                else
                                {
                                    TabForm frm = new TabForm(dragTab);
                                }
                            }
                        }
                    }
                }
            }
        }

        private TabPage TabUnderMouse()
        {
            NativeMethods.TCHITTESTINFO HTI = new NativeMethods.TCHITTESTINFO(tabControl.PointToClient(Cursor.Position));
            int tabID = NativeMethods.SendMessage(tabControl.Handle, NativeMethods.TCM_HITTEST, IntPtr.Zero, ref HTI);
            return tabID == -1 ? null : tabControl.TabPages[tabID];
        }

        private bool PointInTabStrip(Point point)
        {
            Rectangle tabBounds = Rectangle.Empty;
            Rectangle displayRC = tabControl.DisplayRectangle; ;

            switch (tabControl.Alignment)
            {
                case TabAlignment.Bottom:
                    tabBounds.Location = new Point(0, displayRC.Bottom);
                    tabBounds.Size = new Size(tabControl.Width, tabControl.Height - displayRC.Height);
                    break;

                case TabAlignment.Left:
                    tabBounds.Size = new Size(displayRC.Left, tabControl.Height);
                    break;

                case TabAlignment.Right:
                    tabBounds.Location = new Point(displayRC.Right, 0);
                    tabBounds.Size = new Size(tabControl.Width - displayRC.Width, tabControl.Height);
                    break;

                default:
                    tabBounds.Size = new Size(tabControl.Width, displayRC.Top);
                    break;
            }
            tabBounds.Inflate(-3, -3);
            return tabBounds.Contains(point);
        }
        private void SwapTabPages(int index1, int index2)
        {
            if ((index1 | index2) != -1)
            {
                TabPage tab1 = tabControl.TabPages[index1];
                TabPage tab2 = tabControl.TabPages[index2];
                tabControl.TabPages[index1] = tab2;
                tabControl.TabPages[index2] = tab1;
            }
        }
    }

    internal class TabForm : Form
    {
        public TabForm(TabPage tabPage) : base()
        {
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.WindowsDefaultBounds;
            this.MinimizeBox = true;
            this.MaximizeBox = true;
            this.tabPage = tabPage;
            tabPage.Tag = this;
            this.tabControl = (CustomTabControl)tabPage.Parent;
            this.tabID = tabControl.TabPages.IndexOf(tabPage);
            this.ClientSize = tabPage.Size;
            this.Location = tabControl.PointToScreen(new Point(tabPage.Left, tabControl.PointToClient(Cursor.Position).Y - SystemInformation.ToolWindowCaptionHeight / 2));
            this.Text = tabPage.Text;
            UnDockFromTab();
            this.dragOffset = tabControl.PointToScreen(Cursor.Position);
            this.dragOffset.X -= this.Location.X;
            this.dragOffset.Y -= this.Location.Y;
        }
        private TabPage tabPage;
        private CustomTabControl tabControl;
        private int tabID;
        private Point dragOffset;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DockToTab();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_MOVING)
            {
                NativeMethods.RECT rc = (NativeMethods.RECT)m.GetLParam(typeof(NativeMethods.RECT));
                Point pt = tabControl.PointToClient(Cursor.Position);
                Rectangle pageRect = tabControl.DisplayRectangle;
                Rectangle tabsRect = Rectangle.Empty;
                switch (tabControl.Alignment)
                {
                    case TabAlignment.Left:
                        tabsRect.Size = new Size(pageRect.Left, tabControl.Height);
                        break;
                    case TabAlignment.Bottom:
                        tabsRect.Location = new Point(0, pageRect.Bottom);
                        tabsRect.Size = new Size(tabControl.Width, tabControl.Bottom - pageRect.Bottom);
                        break;

                    case TabAlignment.Right:
                        tabsRect.Location = new Point(pageRect.Right, 0);
                        tabsRect.Size = new Size(tabControl.Right - pageRect.Right, tabControl.Height);
                        break;

                    default:
                        tabsRect.Size = new Size(tabControl.Width, pageRect.Top);
                        break;
                }
                if (tabsRect.Contains(pt))
                    DockToTab();
                else
                    UnDockFromTab();
            }
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case NativeMethods.WM_NCLBUTTONDBLCLK:
                    this.Close();
                    break;

                case NativeMethods.WM_EXITSIZEMOVE:
                    if (!this.Visible)
                        this.Close();
                    break;

                case NativeMethods.WM_MOUSEMOVE:
                    if (m.WParam.ToInt32() == 1)
                    {
                        if (!captured)
                        {
                            Point pt = tabControl.PointToScreen((Cursor.Position));
                            Point newPosition = new Point(pt.X - dragOffset.X, pt.Y - dragOffset.Y);
                            this.Location = newPosition;
                        }
                        NativeMethods.RECT rc = new NativeMethods.RECT(this.Bounds);
                        IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
                        Marshal.StructureToPtr(rc, lParam, true);
                        NativeMethods.SendMessage(this.Handle, NativeMethods.WM_MOVING, IntPtr.Zero, lParam);
                        Marshal.FreeHGlobal(lParam);
                    }
                    break;

                case NativeMethods.WM_SETCURSOR:
                    captured = true;
                    break;

                default:
                    break;

            }
        }
        private bool captured;
        private void DockToTab()
        {
            if (!tabControl.TabPages.Contains(tabPage))
            {
                for (int id = this.Controls.Count - 1; id >= 0; id--)
                {
                    tabPage.Controls.Add(this.Controls[0]);
                }
                tabControl.TabPages.Insert(tabID, tabPage);
                tabControl.SelectedTab = tabPage;

                tabControl.Capture = true;
                this.Close();
            }
        }
        private void UnDockFromTab()
        {
            if (this.Visible || this.IsDisposed)
                return;
            for (int id = tabPage.Controls.Count - 1; id >= 0; id--)
            {
                this.Controls.Add(tabPage.Controls[0]);
            }

            tabControl.TabPages.Remove(tabPage);
            this.Capture = true;
            this.Show();
        }
        internal sealed class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left, Top, Right, Bottom;
                public RECT(Rectangle bounds)
                {
                    this.Left = bounds.Left;
                    this.Top = bounds.Top;
                    this.Right = bounds.Right;
                    this.Bottom = bounds.Bottom;
                }
                public override string ToString()
                {
                    return String.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
                }
            }
            public const int WM_NCLBUTTONDBLCLK = 0xA3;
            public const int WM_SETCURSOR = 0x20;

            public const int WM_NCHITTEST = 0x84;

            public const int WM_MOUSEMOVE = 0x200;
            public const int WM_MOVING = 0x216;
            public const int WM_EXITSIZEMOVE = 0x232;

            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref TCHITTESTINFO lParam);

            [StructLayout(LayoutKind.Sequential)]
            public struct TCHITTESTINFO
            {
                public Point pt;
                public TCHITTESTFLAGS flags;
                public TCHITTESTINFO(Point point)
                {
                    pt = point;
                    flags = TCHITTESTFLAGS.TCHT_ONITEM;
                }
            }
            [Flags()]
            public enum TCHITTESTFLAGS
            {
                TCHT_NOWHERE = 1,
                TCHT_ONITEMICON = 2,
                TCHT_ONITEMLABEL = 4,
                TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
            }

            public const int TCM_HITTEST = 0x130D;
        }
        public enum TabDragBehavior { None, TabDragArrange, TabDragOut }
    }

    #endregion

}
