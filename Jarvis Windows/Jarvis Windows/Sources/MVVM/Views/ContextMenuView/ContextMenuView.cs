using Jarvis_Windows.Sources.Utils.Services;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using Jarvis_Windows.Sources.MVVM.Models;

namespace Jarvis_Windows.Sources.MVVM.Views.ContextMenuView
{
    public class CustomContextMenuView : ContextMenuStrip
    {
        public CustomContextMenuView()
        {
            DecorateMenuItems();
        }

        private void DecorateMenuItems()
        {
            this.BackColor = ColorTranslator.FromHtml("#9DBBE6");
            this.ForeColor = ColorTranslator.FromHtml("#1450A3");
            this.Renderer = new CustomToolStripProfessionalRenderer();
            this.Items.Add("Sidebar", null, Sidebar_Click);
            this.Items.Add("Settings", null, Setting_Click);
            this.Items.Add("Quit Jarvis", null, QuitMenuItem_Click);
        }

        private PropertyMessage InitPropertyMessage(string propertyName, object value)
        {
            return new PropertyMessage(propertyName, value);
        }

        private void Sidebar_Click(object? sender, EventArgs e)
        {
            PopupDictionaryService.Instance().IsShowMainNavigation = true;
            EventAggregator.PublishPropertyMessageChanged(
                InitPropertyMessage("IsShowMainNavigation-Chat", true), null
            );
        }

        private void Setting_Click(object? sender, EventArgs e)
        {
            PopupDictionaryService.Instance().IsShowMainNavigation = true;
            EventAggregator.PublishPropertyMessageChanged(
                InitPropertyMessage("IsShowMainNavigation-Settings", true), null
            );
        }

        private void QuitMenuItem_Click(object? sender, EventArgs e)
        {
            try
            {
                _ = GoogleAnalyticService.Instance().SendEvent("quit_app");
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }

    class CustomToolStripProfessionalRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);
            
            {
                //Rectangle rc = new Rectangle(System.Drawing.Point.Empty, e.Item.Size);
                //e.Graphics.FillRectangle(System.Drawing.Brushes.Transparent, rc);
                //e.Graphics.DrawRectangle(Pens.Transparent, 1, 0, rc.Width - 2, rc.Height - 1);
                /*int borderRadius = 10; // Kích thu?c góc bo tròn
                int arcSize = borderRadius * 2;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(rc.X, rc.Y, arcSize, arcSize, 180, 90); // Góc trên bên trái
                    path.AddArc(rc.Right - arcSize, rc.Y, arcSize, arcSize, 270, 90); // Góc trên bên ph?i
                    path.AddArc(rc.Right - arcSize, rc.Bottom - arcSize, arcSize, arcSize, 0, 90); // Góc du?i bên ph?i
                    path.AddArc(rc.X, rc.Bottom - arcSize, arcSize, arcSize, 90, 90); // Góc du?i bên trái
                    path.CloseFigure();

                    e.Graphics.FillPath(Brushes.Transparent, path);
                    e.Graphics.DrawPath(Pens.Transparent, path);
                }*/
            }
        }
    }
}
