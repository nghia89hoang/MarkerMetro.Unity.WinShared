﻿using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Microsoft.Phone.Info;
#endif
﻿using Windows.System;
using Windows.UI.Xaml;
using MarkerMetro.Unity.WinIntegration;
using UnityPlayer;

#if NETFX_CORE
namespace Template
#else
namespace XXXXXXXXXXXXXXXXXXXX   //  <--- Your Windows Phone namespace here!
#endif
{
    /**
     * This is a partial class containing code that can be shared between all 
     * Unity porting projects in Marker Metro.
     * 
     * 
     * (Dont forget that the namespace must match)
     */
    public sealed partial class MainPage
    {
#if WINDOWS_PHONE
        private Timer timer;
#endif

        /**
         * Exhibits information about memory usage in the game screen. WP8 only.
         * 
         */
        private bool DisplayMemoryInfo = false;


        /**
         * Call this on MainPage.xaml.cs.
         */
        private void Initialize()
        {
            // wire up the configuration file handler:
            DeviceInformation.DoGetEnvironment = GetEnvironment;

            if (DisplayMemoryInfo)
                BeginRecording();

            // Calls WinIntegration's visibility change delegate:
            Window.Current.VisibilityChanged += (s, e) =>
            {
                AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    if (Helper.Instance.OnVisibilityChanged != null)
                        Helper.Instance.OnVisibilityChanged(e.Visible);
                }, false);
            };
        }

        private DeviceInformation.Environment GetEnvironment()
        {
#if QA
            return DeviceInformation.Environment.QA;
#elif DEBUG
            return DeviceInformation.Environment.Dev;
#else
            return DeviceInformation.Environment.Production;
#endif
        }

        /**
         * Add this to your DrawingSurfaceBackgroundGrid block in MaingPage.xaml:
         *  <TextBlock x:Name="TextBoxMemoryStats" Text="0 MB" IsHitTestVisible="False" Visibility="Collapsed"/>
         */
        private void BeginRecording()
        {
            // start a timer to report memory conditions every 3 seconds 
#if WINDOWS_PHONE
            TextBoxMemoryStats.Visibility = System.Windows.Visibility.Visible;

            timer = new Timer(state =>
            {
                string report = "";
                
                report +=
                   "Current: " + (DeviceStatus.ApplicationCurrentMemoryUsage / 1000000).ToString() + "MB\n" +
                   "Peak: " + (DeviceStatus.ApplicationPeakMemoryUsage / 1000000).ToString() + "MB\n" +
                   "Memory Limit: " + (DeviceStatus.ApplicationMemoryUsageLimit / 1000000).ToString() + "MB\n\n" +
                   "Device Total Memory: " + (DeviceStatus.DeviceTotalMemory / 1000000).ToString() + "MB\n" +
                   "Working Limit: " + Convert.ToInt32((Convert.ToDouble(DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit")) / 1000000)).ToString() + "MB";

                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    TextBoxMemoryStats.Text = report;
                    //Debug.WriteLine(report);
                });

            },
                null,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3));
#endif
        }
    }
}
