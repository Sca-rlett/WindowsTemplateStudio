﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Templates.Core.Mvvm;
using Microsoft.Templates.UI.Extensions;
using Microsoft.Templates.UI.ViewModels.Common;

namespace Microsoft.Templates.UI.Controls
{
    /// <summary>
    /// Interaction logic for StatusBox.xaml
    /// </summary>
    public partial class StatusBox : UserControl
    {
        private DispatcherTimer _hideTimer;

        public StatusViewModel Status
        {
            get { return (StatusViewModel)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(StatusViewModel), typeof(StatusBox), new PropertyMetadata(null, OnStatusPropertyChanged));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            private set { SetValue(CloseCommandProperty, value); }
        }
        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(StatusBox), new PropertyMetadata(null));

        private static void OnStatusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StatusBox;
            if (control != null)
            {
                control.UpdateStatus(e.NewValue as StatusViewModel);
            }
        }

        public StatusBox()
        {
            _hideTimer = new DispatcherTimer();
            _hideTimer.Tick += OnHideTimerTick;
            CloseCommand = new RelayCommand(OnClose);
            InitializeComponent();
        }

        private void OnClose() => UpdateVisibility(false);

        private void OnHideTimerTick(object sender, EventArgs e)
        {
            this.FadeOut();
            _hideTimer.Stop();
        }

        private void UpdateStatus(StatusViewModel status)
        {
            var isVisible = status != null && status.Status != StatusType.Empty;
            UpdateVisibility(isVisible, status.AutoHideSeconds);
        }

        private void UpdateVisibility(bool isVisible, int autoHideSeconds = 0)
        {
            if (isVisible)
            {
                Panel.SetZIndex(this, 2);
                closeButton.Focusable = true;
                if (autoHideSeconds > 0)
                {
                    _hideTimer.Interval = TimeSpan.FromSeconds(autoHideSeconds);
                    _hideTimer.Start();
                }
                this.FadeIn();
            }
            else
            {
                Panel.SetZIndex(this, 0);
                closeButton.Focusable = false;
                this.FadeOut(0);
            }
        }
    }
}
