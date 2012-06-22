﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Signum.Entities.Omnibox;
using System.Collections;
using Signum.Utilities;
using System.Threading;

namespace Signum.Windows.Omnibox
{
    /// <summary>
    /// Interaction logic for OmniboxAutocomplete.xaml
    /// </summary>
    public partial class OmniboxAutocomplete : UserControl
    {
        public OmniboxAutocomplete()
        {
            InitializeComponent();

            this.autoCompleteTb.ItemTemplate = Fluent.GetDataTemplate(()=>new OmniboxTemplate());
            this.autoCompleteTb.Delay = TimeSpan.FromMilliseconds(100);
        }

        private IEnumerable AutoCompleteTextBox_AutoCompleting(string arg, CancellationToken ct)
        {
            return OmniboxParser.Results(arg, ct);
        }

        private void autoCompleteTb_Closed(object sender, CloseEventArgs e)
        {
            var selected = autoCompleteTb.SelectedItem as OmniboxResult;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                autoCompleteTb.SelectEnd();
            else if (selected != null)
                OmniboxClient.OnResultSelected.Invoke(selected);
        }
    }
}
