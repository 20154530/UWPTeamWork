﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Media3D;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace UWPTeamWork
{
    public sealed class SlideClock : Control
    {
        public enum HandleType
        {
            Hour = 0,
            Min = 1,
            Sec = 2
        }//当前选定指针
        private DoubleAnimation SecB_RotateTransform_Storyboard_DoubleAnimation;
        private Storyboard SecB_RotateTransform_Storyboard;
        private DoubleAnimation MinB_RotateTransform_Storyboard_DoubleAnimation;
        private Storyboard MinB_RotateTransform_Storyboard;

        public static Timer Sec_Timer;
        public static Timer MainTimer;

        private double Pointliner;
        private int tiemdds = 0;

        //属性
        #region 选中的指针
        public HandleType SelectedH
        {
            get { return (HandleType)GetValue(SelectedHProperty); }
            set { SetValue(SelectedHProperty, value); }
        }
        public static readonly DependencyProperty SelectedHProperty =
            DependencyProperty.Register("SelectedH", typeof(HandleType), typeof(SlideClock), new PropertyMetadata(false, new PropertyChangedCallback(OnSecBTappedChanged)));

        private static void OnSecBTappedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region 时间/h
        public double HoursAng
        {
            get { return (double)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }
        public static readonly DependencyProperty HoursProperty =
            DependencyProperty.Register("HoursAng", typeof(double), typeof(SlideClock), new PropertyMetadata(0, new PropertyChangedCallback(OnHoursChanged)));
        private static void OnHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region 时间/m
        public double MinutesAng
        {
            get { return (double)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }
        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("MinutesAng", typeof(double), typeof(SlideClock), new PropertyMetadata(0, new PropertyChangedCallback(OnMinutesChanged)));
        private static void OnMinutesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region 时间/s
        public double SecondsAng
        {
            get { return (double)GetValue(SecondsAngProperty); }
            set { SetValue(SecondsAngProperty, value); }
        }
        public static readonly DependencyProperty SecondsAngProperty =
            DependencyProperty.Register("SecondsAng", typeof(double), typeof(SlideClock), new PropertyMetadata(0, new PropertyChangedCallback(OnSecondsChanged)));
        private static void OnSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((double)e.NewValue == 360)
            {
                SlideClock k = (SlideClock)d;
                k.SecB_RotateTransform_Storyboard.Begin();
                k.SecondsAng = 0;
            }
        }
        #endregion

        protected override void OnApplyTemplate()
        {

            var Pause = GetTemplateChild("Stop") as Button;
            Pause.Click += Pause_Click;
            var Resume = GetTemplateChild("Resume") as Button;
            Resume.Click += Resume_Click;
            var Start = GetTemplateChild("Start") as Button;
            Start.Click += Start_Click;

            base.OnApplyTemplate();
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Handle_Show", false);
            Sec_Timer.Start();
            MainTimer.Start();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Handle_Hide", false);
            Sec_Timer.Stop();
            MainTimer.Stop();
            SecondsAng = tiemdds*6;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Handle_Show", false);
            SecondsAng = 0;
            MainTimer.Start();
            Sec_Timer.Start();
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            
            if (!e.IsInertial)
            {
                double angleOfLine = Math.Atan2((e.Position.Y - ActualHeight / 2), (e.Position.X - ActualWidth / 2)) * 180 / Math.PI;
                if (Pointliner < ActualHeight * 3 / 5)
                {
                    if (Pointliner > ActualHeight / 4)
                    {
                        MinutesAng = angleOfLine + 90;
                    }
                }
            }
            base.OnManipulationDelta(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            Pointliner = Math.Sqrt(Math.Abs(e.GetCurrentPoint(this).Position.X - ActualWidth / 2) * Math.Abs(e.GetCurrentPoint(this).Position.X - ActualWidth / 2)
                  + Math.Abs(e.GetCurrentPoint(this).Position.Y - ActualHeight / 2) * Math.Abs(e.GetCurrentPoint(this).Position.Y - ActualHeight / 2));
            base.OnPointerPressed(e);
        }

        public SlideClock()
        {
            this.DefaultStyleKey = typeof(SlideClock);
            this.ManipulationMode = ManipulationModes.All;
            this.Loaded += OnLoaded;

            Sec_Timer = new Timer { Interval = 40 };
            Sec_Timer.Elapsed += Sec_Timer_Tick;

            MainTimer = new Timer { Interval = 1000 };
            MainTimer.Elapsed += Sec_Timer_Tick1;
        }

        private async void Sec_Timer_Tick1(object sender, ElapsedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                tiemdds++;
                Debug.WriteLine(SecondsAng + "  " + tiemdds);
            });
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void Sec_Timer_Tick(object sender, object e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SecondsAng += 0.24;
            });
        }

        #region 属性变化通知
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
