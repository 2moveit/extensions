﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Signum.Windows;
using Signum.Windows.Authorization;
using Signum.Windows.Operations;
using Signum.Windows.Processes;
using Signum.Windows.Reports;
using Signum.Windows.Scheduler;
using Signum.Entities.Operations;
using Signum.Entities;
using Signum.Entities.Basics;
using Signum.Windows.Files;
using System.Threading;
using Signum.Test;
using Signum.Windows.Extensions.Sample.Controls;

namespace Signum.Windows.Extensions.Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
            : base()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            Async.ExceptionHandler = UnhandledAsyncException;

            InitializeComponent();
        }


        void UnhandledAsyncException(Exception e, Window win)
        {
            Program.HandleException("Error en llamada asíncrona", e);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Program.HandleException("Error inesperado", e.Exception);
            e.Handled = true;
        }

        internal static BitmapFrame LoadIcon(string segurosImage)
        {
            return ImageLoader.LoadIcon(PackUriHelper.Reference("Imagenes/" + segurosImage, typeof(App)));
        }

        public static void StartApplication()
        {
            Navigator.Start(new NavigationManager
            {
                EntitySettings = new Dictionary<Type, EntitySettings>()
                {
                }
            });

            Constructor.Start(new ConstructorManager
            {
                Constructors = new Dictionary<Type, Func<Window, object>>()
                {

                }
            });

            OperationClient.Start(new OperationManager
            {
                Settings = new Dictionary<Enum, OperationSettings>()
                {
                }
            });

            AuthClient.Start(true, true, true, true, true, true);

            //ProcessClient.Start();
            //SchedulerClient.Start();

            LinksWidget.Start();
            ReportClient.Start(true, false, false);

            Links.RegisterGlobalLinks((r, c) => new[]{
                new QuickLinkExplore(new ExploreOptions(typeof(LogOperationDN) )
                                    {
                                        FilterOptions = { new FilterOption("Target", r) },
                                        OrderOptions = { new OrderOption("Start") }
                                    })
            });

            Navigator.AddSettings(new List<EntitySettings>()
            {
                new EntitySettings<AlbumDN>(EntityType.Default){ View = e => new Album() },

                new EntitySettings<LabelDN>(EntityType.Admin) { View = e => new Label() },
                new EntitySettings<ArtistDN>(EntityType.Default) { View = e => new Artist() },
                new EntitySettings<BandDN>(EntityType.Default) { View = e => new Band() },

                new EntitySettings<AmericanMusicAwardDN>(EntityType.Default) { View = e => new AmericanMusicAward() },
                new EntitySettings<GrammyAwardDN>(EntityType.Default) { View = e => new GrammyAward() },
                new EntitySettings<PersonalAwardDN>(EntityType.Default) { View = e => new PersonalAward() },

                new EntitySettings<CountryDN>(EntityType.Admin) { View = e => new Country() },

                new EntitySettings<Signum.Test.NoteDN>(EntityType.Default) { View = e => new Note() },
            });

            Navigator.Initialize();
        }
    }
}