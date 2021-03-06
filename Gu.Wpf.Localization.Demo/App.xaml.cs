﻿namespace Gu.Wpf.Localization.Demo
{
    using System.Globalization;
    using System.Threading;
    using System.Windows;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(e.Args[0]);
            }

            base.OnStartup(e);
        }
    }
}
