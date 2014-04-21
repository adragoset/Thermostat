using System;
using Microsoft.SPOT;

using GHI.Glide;
using GHI.Glide.UI;
using GHI.Glide.Geom;
using Gadgeteer.Modules.GHIElectronics;

namespace Thermostat.TouchUi
{
    public class TouchInitialization
    {
        private DisplayCP7 Display;
        private Point last = new Point(0, 0);
        private bool touched = false;

        public TouchInitialization(DisplayCP7 display) {
            this.Display = display;
            this.Display.ScreenPressed += new DisplayCP7.TouchEventHandler(display_CP7_ScreenPressed);
            this.Display.ScreenReleased += new DisplayCP7.TouchEventHandlerTouchReleased(screen_released);
        }

        private void screen_released(DisplayCP7 sender)
        {
            this.touched = false;

            GlideTouch.RaiseTouchUpEvent(sender, new TouchEventArgs(this.last));
        }

               public void display_CP7_ScreenPressed(DisplayCP7 sender, DisplayCP7.TouchStatus touchStatus)
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
    }
}
