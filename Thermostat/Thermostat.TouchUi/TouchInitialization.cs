using System;
using Microsoft.SPOT;

using Gadgeteer.Modules.GHIElectronics;
using GHI.Glide;
using GHI.Glide.UI;
using GHI.Glide.Geom;

namespace Thermostat.TouchUi
{
    public class TouchInitialization
    {
        private Display_CP7 Display;
        private Point last = new Point(0, 0);
        private bool touched = false;

        public TouchInitialization(Display_CP7 display) {
            this.Display = display;
            this.Display.ScreenPressed += (a, b) => display_CP7_ScreenPressed(a, b);
            this.Display.ScreenReleased += (a) => display_CP7_ScreenReleased(a);
        }

        public void display_CP7_ScreenPressed(Display_CP7 sender, Display_CP7.TouchStatus touchStatus)
        {
            if (touchStatus.numTouches <= 0 || GlideTouch.IgnoreAllEvents)
                return;

            Point touch = new Point(touchStatus.touchPos[0].xPos, touchStatus.touchPos[0].yPos);

            if (this.touched)
            {
                if (this.last.X != touch.X || this.last.Y != touch.Y)
                    GlideTouch.RaiseTouchMoveEvent(sender, new TouchEventArgs(touch));
                //GlideTouch.RaiseTouchDownEvent(sender, new TouchEventArgs(touch));

                this.last.X = touch.X;
                this.last.Y = touch.Y;
            }
            else
            {
                this.last.X = touch.X;
                this.last.Y = touch.Y;
                this.touched = true;

                GlideTouch.RaiseTouchDownEvent(sender, new TouchEventArgs(touch));
            }
        }

        public void display_CP7_ScreenReleased(Display_CP7 sender)
        {
            this.touched = false;

            GlideTouch.RaiseTouchUpEvent(sender, new TouchEventArgs(this.last));
        }
    }
}
