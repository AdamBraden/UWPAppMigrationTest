using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using System.Runtime.InteropServices;
using Windows.Data.Xml.Dom;
#if WINDOWS_UWP
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;
#endif

namespace BlankUWPApp1
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
#if WINDOWS_UWP
        protected override void OnLaunched(LaunchActivatedEventArgs e)
#else
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
#endif
        {

#if WINDOWS_UWP
            var curWindow = Window.Current;
#else
            m_window = new Window();
            m_window.Content = new Frame();
            var curWindow = m_window;
            curWindow.Title = "My Test App - hWnd";
            
            //Intialize interop helper for dialog parenting scenarios
            var windowNative = m_window.As<IWindowNative>();
            m_windowHandle = windowNative.WindowHandle;

            // Initialize the main Page to load into Window
            m_page = new MainPage();

            //Hook up On*Activated events
            //Could be here or Main
            //Most likely at end of the method
            var activationArgs = AppInstance.GetActivatedEventArgs();
            if (activationArgs.Kind == ActivationKind.File)
            {
                OnFileActivated(activationArgs as FileActivatedEventArgs);
            }
#endif

            Frame rootFrame = curWindow.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
#if WINDOWS_UWP
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
#endif
                // Place the frame in the current Window
                curWindow.Content = rootFrame;
            }


#if WINDOWS_UWP
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                }      
                //Q: How does UWP know what the 'first' page is to pass as e.arguments?
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
#else
            rootFrame.Navigate(typeof(MainPage), m_page);

#endif

            // Ensure the current window is active
            curWindow.Activate();

        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);

            // TODO: Handle file activation
            // The number of files received is args.Files.Size
            // The name of the first file is args.Files[0].Name

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
            XmlNodeList textElements = toastXml.GetElementsByTagName("text");
            textElements[0].AppendChild(toastXml.CreateTextNode("My Test Application"));
            textElements[1].AppendChild(toastXml.CreateTextNode($"File Type activated: {args.Files[0].Name}"));
            ToastNotification notification = new ToastNotification(toastXml);
            notification.ExpirationTime = DateTime.Now.AddMinutes(5);
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }



        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
#if WINDOWS_UWP
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
#else
        void OnNavigationFailed(object sender, Microsoft.UI.Xaml.Navigation.NavigationFailedEventArgs e)
#endif
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private Window m_window;
        private Page m_page;
        private IntPtr m_windowHandle;
        public IntPtr WindowHandle { get { return m_windowHandle; } }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        }

        //CoreWindow interop helper, pass in your hwnd
        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }

    }

}
