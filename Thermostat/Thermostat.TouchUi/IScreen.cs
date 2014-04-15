using System;
using Microsoft.SPOT;
using GHI.Glide.Display;

namespace Thermostat.TouchUi
{
    public delegate void UpdateScreenEventHandler(object sender, UpdateScreenArgs e);

    public class UpdateScreenArgs : EventArgs
    {
        public string Name { get; private set; }

        public UpdateScreenArgs(string e)
        {
            this.Name = e;
        }


    }

    public interface IScreen
    {
        Window Window { get; }

        event UpdateScreenEventHandler StatusUpdated;

    }
}
