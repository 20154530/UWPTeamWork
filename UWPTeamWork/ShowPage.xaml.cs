﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using UWPTeamWork.tile;
using xBindDataExample.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWPTeamWork
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShowPage : Page
    {
        Note note;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //这个e.Parameter是获取传递过来的参数，其实大家应该再次之前判断这个参数是否为null的，我偷懒了
            if (e != null)
            {
                note = (Note)e.Parameter;
                Text();
            }
        }
        public ShowPage()
        {
            this.InitializeComponent();
            

            this.Loaded += ShowPage_Loaded;
            
            //ShowBox.Text = note.MyText;
        }

        private void Text()
        {
            Title.Text = note.summary;
            ShowBox.Text = note.MyText;
        }

        private void ShowPage_Loaded(object sender, RoutedEventArgs e)
        {
           // ShowBox.Text = note.MyText;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            note.MyText = ShowBox.Text;
            note.summary = Title.Text;
            NoteManager.Save("timer");
            //更新磁贴
            TileService.UpdatePrimaryTile(note.summary,note.MyText);

            Frame.Navigate(typeof(NotePage));
        }

        private void MySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double x = MySlider.Value;
            int y = (int)(x / 10.0) * 4 + 16;
            ShowBox.FontSize = y;
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            NoteManager.DeleteNote(note);
            NoteManager.Save("");
            Frame.Navigate(typeof(NotePage));
        }
    }
}
