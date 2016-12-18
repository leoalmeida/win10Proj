using ProjetoApp.Views;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Globalization;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace ProjetoApp
{
    
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        AppShell oldshell;

        public event EventHandler<BackRequestedEventArgs> BackRequested;
        

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            AppShell shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();
                // Set the default language
                shell.Language = ApplicationLanguages.Languages[0];
                shell.AppFrame.NavigationFailed += (s, args) =>
                    new Exception("Failed to load Page " + args.SourcePageType.FullName);
            }
            // Place our app shell in the current Window
            Window.Current.Content = shell;
            if (shell.AppFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page
                // suppressing the initial entrance animation.
                shell.AppFrame.Navigate(typeof(CustomerListPage), e.Arguments,
                    new SuppressNavigationTransitionInfo());
                // shell.AppFrame.Navigate(typeof(MainPage), e.Arguments,
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        public void LoginHandled()
        {
            Window.Current.Content = null;

            if (oldshell != null)
            {
                Window.Current.Content = oldshell.AppFrame;
            }
        }

    }
   
}