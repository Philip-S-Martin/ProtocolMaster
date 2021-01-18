using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model
{
    class SnippetsToComplete
    {
        // should load data to plot model on UI thread
        /*
        public void OnEventsLoaded(List<ProtocolEvent> data)
        {
            Task UITask = new Task(() =>
            {
                if (data != null)
                {
                    App.Window.TimelineView.LoadPlotData(data);
                }
            }, canc,TaskScheduler.FromCurrentSynchronizationContext());
        }
        */
    }
}
