using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Touch;
using GT = Gadgeteer;

using GHIElectronics.NETMF.Glide;
using GHIElectronics.NETMF.Glide.Display;
using GHIElectronics.NETMF.Glide.UI;
using Gadgeteer.Modules.GHIElectronics;

namespace Thermostat.Ui
{
    public class ThermostatTouchUi
    {
        private Display_CP7 Display;

        private static GHIElectronics.NETMF.Glide.Display.Window window;
        private static TextBlock textblock;
        private static Button button;

        public ThermostatTouchUi(Display_CP7 display, string windowXml)
        {
            Display = display;
            Display.ScreenPressed += new Display_CP7.TouchEventHandler(display_CP7_ScreenPressed);

        }

        void display_CP7_ScreenPressed(Display_CP7 sender, Display_CP7.TouchStatus touchStatus)
        {
            if (touchStatus.numTouches > 0)
            {
                var touch = touchStatus.touchPos[0];
                Display.SimpleGraphics.DisplayEllipse(GT.Color.Red, (uint)touch.xPos, (uint)touch.yPos, (uint)25, (uint)25);
            }
        }

        
    }
}
