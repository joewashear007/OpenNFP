using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plotly.Blazor;

namespace OpenNFP.Client.Utils
{
    public class PlotyClick: EventArgs
    {
        public int Index { get; set; }
    }

    [EventHandler("plotlyclick", typeof(PlotyClick), enableStopPropagation: true, enablePreventDefault: true)]
    static class EventHandlers
    {


        /// <summary>
        ///  Can be used to subscribe click events for Plot points.
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="objectReference"></param>
        /// <returns></returns>
        public static async Task SubscribeClickEvent<T>(this IJSRuntime jsRuntime, DotNetObjectReference<T> objectReference, string id) where T: class
        {
            await jsRuntime.InvokeVoidAsync($"subscribeClickEvent", objectReference, id);
        }
    }
}
