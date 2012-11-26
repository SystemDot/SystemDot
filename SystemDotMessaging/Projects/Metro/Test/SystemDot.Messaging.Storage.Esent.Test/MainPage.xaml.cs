﻿using System;
using SystemDot.Messaging.Storage.Changes;
using SystemDot.Serialisation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SystemDot.Messaging.Storage.Esent.Test
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var store = new EsentChangeStore(new FileSystem(), new PlatformAgnosticSerialiser());
            store.Initialise("Messaging");

            var changes = store.GetChanges("Test");

            for (int i = 0; i < 5000; i++)
            {
                Guid id = store.StoreChange("Test", new TestChange { Number = i });
                var change = store.GetChange(id).As<TestChange>();
            }

            changes = store.GetChanges("Test");
        }
    }

    public class TestChange : Change
    {
        public int Number { get; set; }
    }
}