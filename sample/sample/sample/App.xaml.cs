using System;
using SimpleInjector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sample
{
    public partial class App : Application
    {
        private static Container _container;

        public static Container Container => _container ??= new Container();
        
        public App ()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

