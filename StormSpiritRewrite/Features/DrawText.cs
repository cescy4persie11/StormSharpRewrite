using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using SharpDX;
using Ensage.Common;
using Ensage.Common.Menu;
using Ensage.Common.Objects.UtilityObjects;

namespace StormSpiritRewrite.Features
{
    public class DrawText
    {
        private string text;

        private Vector2 textSize;

        private readonly Sleeper sleeper;

        public DrawText()
        {
            this.sleeper = new Sleeper();
        }

        public Color Color { get; set; }

        public FontFlags FontFlags { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; private set; }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                if (this.sleeper.Sleeping)
                {
                    return;
                }

                this.Size = Drawing.MeasureText(this.text, "Arial", this.textSize, this.FontFlags);
                this.sleeper.Sleep(2000);
            }
        }

        public Vector2 TextSize
        {
            get
            {
                return this.textSize;
            }

            set
            {
                this.textSize = value;
                this.Size = Drawing.MeasureText(this.text, "Arial", this.textSize, this.FontFlags);
                this.sleeper.Sleep(2000);
            }
        }

        public void Draw()
        {
            Drawing.DrawText(this.text, this.Position, this.textSize, this.Color, this.FontFlags);
        }

        public void DrawTextSelfZip(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.5));

            this.text = "SelfZip" + " [" + Utils.KeyToText(Variables.MenuManager.SelfZipMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextChaseZip(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.54));

            this.text = "Chase" + " [" + Utils.KeyToText(Variables.MenuManager.ChaseZipMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextInitiate(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.58));

            this.text = "Combo" + " [" + Utils.KeyToText(Variables.MenuManager.InitiateZipMenu.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextFlee(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.62));

            this.text = "Flee" + " [" + Utils.KeyToText(Variables.MenuManager.FleeHotKey.GetValue<KeyBind>().Key) + "] " + Variables.MenuManager.TpDistance;
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Yellow;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }

        public void DrawTextTpEnabled(bool on)
        {
            var startPos = new Vector2(Convert.ToSingle(Drawing.Width) - 130, Convert.ToSingle(Drawing.Height * 0.66));

            this.text = "TP" + " [" + Utils.KeyToText(Variables.MenuManager.FleeTpEnabled.GetValue<KeyBind>().Key) + "] " + (on ? "ON" : "OFF");
            this.Position = startPos;
            this.textSize = new Vector2(20);
            this.Color = !on ? Color.Red : Color.Green;
            this.FontFlags = FontFlags.AntiAlias | FontFlags.DropShadow | FontFlags.Additive | FontFlags.Custom | FontFlags.StrikeOut;
            this.Draw();
        }








    }
}
