﻿using Jarvis_Windows.Sources.Utils.Services;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.Views.ContextMenuView
{
    public class CustomContextMenuView : ContextMenuStrip
    {
        public PopupDictionaryService PopupDictionaryService { get; }
        public SendEventGA4 SendEventGA4 { get; }

        public CustomContextMenuView(PopupDictionaryService popupDictionaryService, SendEventGA4 sendEventGA4)
        {
            PopupDictionaryService = popupDictionaryService;
            SendEventGA4 = sendEventGA4;
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

        private void Sidebar_Click(object? sender, EventArgs e)
        {
            EventAggregator.PublishAIChatBubbleStatusChanged(this, EventArgs.Empty);
        }


        private void Setting_Click(object? sender, EventArgs e)
        {
            PopupDictionaryService.IsShowSettingMenu = true;
        }

        private async void QuitMenuItem_Click(object? sender, EventArgs e)
        {
            try
            {
                await SendEventGA4.SendEvent("quit_app");
                Process.GetCurrentProcess().Kill();
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
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
                /*int borderRadius = 10; // Kích thước góc bo tròn
                int arcSize = borderRadius * 2;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(rc.X, rc.Y, arcSize, arcSize, 180, 90); // Góc trên bên trái
                    path.AddArc(rc.Right - arcSize, rc.Y, arcSize, arcSize, 270, 90); // Góc trên bên phải
                    path.AddArc(rc.Right - arcSize, rc.Bottom - arcSize, arcSize, arcSize, 0, 90); // Góc dưới bên phải
                    path.AddArc(rc.X, rc.Bottom - arcSize, arcSize, arcSize, 90, 90); // Góc dưới bên trái
                    path.CloseFigure();

                    e.Graphics.FillPath(Brushes.Transparent, path);
                    e.Graphics.DrawPath(Pens.Transparent, path);
                }*/
            }
        }
    }
}