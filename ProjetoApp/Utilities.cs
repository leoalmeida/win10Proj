using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ProjetoApp
{
    /// <summary>
    /// Static class for global utilities. 
    /// </summary>
    internal class Utilities
    {
        /// <summary>
        /// Updates the UI thread from a background thread. 
        /// From task snippet: 
        /// https://github.com/Microsoft/Windows-task-snippets/blob/master/tasks/UI-thread-access-from-background-thread.md
        /// </summary>
        public static async Task CallOnUiThreadAsync(DispatchedHandler handler) =>
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, handler);
    }
}
