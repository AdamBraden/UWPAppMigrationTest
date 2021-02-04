using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using System.Runtime.InteropServices;
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using WinRT;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;
#endif
using Windows.Data.Xml.Dom;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BlankUWPApp1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            this.SizeChanged += MainPage_SizeChanged;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var showDialog = new MessageDialog("Read nuget packages folder?");
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();

#if !WINDOWS_UWP
            //Interop helpers for non-CoreWindow
            ParentDialogToWindow(showDialog);
            ParentDialogToWindow(folderPicker);
#endif

            //show a prompt
            await showDialog.ShowAsync();

            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                // Get the first folder in the current folder, sorted by date.
                IReadOnlyList<StorageFolder> sortedItems = await folder.GetFoldersAsync();
                var items = sortedItems.Select(x => x.Name).ToList();
                dirList.Text = string.Join("\n", items);

            }
            //fails under app container
            //dirList.Text s = string.Join("\n",System.IO.Directory.GetDirectories(path));

            }

        /// If the page is resized to less than 500 pixels, use the layout for narrow widths. 
        /// If the page is resized so that the width is less than the height, use the tall (portrait) layout.
        /// Otherwise, use the default layout.
        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 500)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width < e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
        }
        public void PrintStars(string s)
        {
            //C# 7 feature
            if (int.TryParse(s, out var i)) { System.Diagnostics.Debug.WriteLine(new string('*', i)); }
            else { Debug.WriteLine("Cloudy - no stars tonight!"); }
        }

#if !WINDOWS_UWP
        private static void ParentDialogToWindow(object dialog)
        {
            //Interop helpers for non-CoreWindow
            IntPtr windowHandle = (App.Current as App).WindowHandle;
            var initializeWithWindow = dialog.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(windowHandle);
        }
#endif

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
