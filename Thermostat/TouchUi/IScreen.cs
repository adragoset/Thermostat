using System;
using Microsoft.SPOT;
using GHI.Glide.Display;

namespace Thermostat.TouchUi
{
    public delegate void UpdateScreenEventHandler(object sender, UpdateScreenArgs e);

    public class UpdateScreenArgs : EventArgs
    {
        public string Name { get; private set; }
        public IScreen Window { get; private set; }

        public UpdateScreenArgs(string e, IScreen window)
        {
            this.Name = e;
            this.Window = window;
        }


    }

    public interface IScreen
    {
        Window Window { get; }

        event UpdateScreenEventHandler StatusUpdated;

        void Open();

        void Close();

    }
}
