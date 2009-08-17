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
using System.Diagnostics;
using Signum.Utilities;
using Signum.Utilities.ExpressionTrees;
using Signum.Utilities.Reflection;
using System.Reflection;
using System.ComponentModel;
using Signum.Entities;
using System.IO;
using Microsoft.Win32;
using Signum.Entities.Basics;
using Signum.Entities.Reflection;
using Signum.Entities.Files;

namespace Signum.Windows.Files
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FileLine: LineBase
    {
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register("Entity", typeof(object), typeof(FileLine), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((FileLine)d).EntityChanged(e.OldValue, e.NewValue)));
        public object Entity
        {
            get { return (object)GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        public static readonly DependencyProperty EntityTemplateProperty =
           DependencyProperty.Register("EntityTemplate", typeof(DataTemplate), typeof(FileLine), new UIPropertyMetadata(null));
        public DataTemplate EntityTemplate
        {
            get { return (DataTemplate)GetValue(EntityTemplateProperty); }
            set { SetValue(EntityTemplateProperty, value); }
        }

        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register("Open", typeof(bool), typeof(FileLine), new FrameworkPropertyMetadata(true, (d, e) => ((FileLine)d).UpdateVisibility()));
        public bool Open
        {
            get { return (bool)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(bool), typeof(FileLine), new FrameworkPropertyMetadata(true, (d, e) => ((FileLine)d).UpdateVisibility()));
        public bool View
        {
            get { return (bool)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        public static readonly DependencyProperty SaveProperty =
            DependencyProperty.Register("Save", typeof(bool), typeof(FileLine), new FrameworkPropertyMetadata(true, (d, e) => ((FileLine)d).UpdateVisibility()));
        public bool Save
        {
            get { return (bool)GetValue(SaveProperty); }
            set { SetValue(SaveProperty, value); }
        }

        public static readonly DependencyProperty RemoveProperty =
            DependencyProperty.Register("Remove", typeof(bool), typeof(FileLine), new FrameworkPropertyMetadata(true, (d, e) => ((FileLine)d).UpdateVisibility()));
        public bool Remove
        {
            get { return (bool)GetValue(RemoveProperty); }
            set { SetValue(RemoveProperty, value); }
        }

        protected override DependencyProperty CommonRouteValue()
        {
            return EntityProperty;
        }

        public delegate void SaveFileDelegate(object entity);
        public delegate void ViewFileDelegate(object entity);
        public delegate object OpenFileDelegate();

        public event Func<object> Opening;
        public event Action<object> Saving;
        public event Action<object> Viewing;
        public event Func<object, bool> Removing;

        public event Action<FileDialog> CustomizeFileDialog; 

        Type cleanType;

        public FileLine()
        {
            InitializeComponent();

        }

        public override void OnLoad(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            base.OnLoad(sender, e);

            if (this.NotSet(EntityTemplateProperty))
            {
                EntityTemplate = Navigator.FindDataTemplate(this, Type);
            }

            if (typeof(Lazy).IsAssignableFrom(Type))
            {
                cleanType = Reflector.ExtractLazy(Type);
            }
            else
            {
                cleanType = Type;
            }

            UpdateVisibility(); 
        }


        protected virtual void EntityChanged(object oldValue, object newValue)
        {
            UpdateVisibility();
        }

        protected void UpdateVisibility()
        {
            btSave.Visibility = Save && Entity != null ? Visibility.Visible : Visibility.Collapsed;
            btOpen.Visibility = Open && Entity == null ? Visibility.Visible : Visibility.Collapsed;
            btView.Visibility = View && Entity != null? Visibility.Visible : Visibility.Collapsed;
            btRemove.Visibility = Remove && Entity != null ? Visibility.Visible : Visibility.Collapsed;
        }
      
        private void OnViewing(object entity)
        {
            if (Entity == null || !View)
                return;

            if (Viewing != null)
            {
                Viewing(entity);
            }
            else if (typeof(IFile).IsAssignableFrom(cleanType))
            {
                IFile file = (IFile)Server.Convert(entity, cleanType);
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), file.FileName);
                File.WriteAllBytes(filePath, file.BinaryFile);
                Process.Start(filePath);
            }
            else
                throw new ApplicationException("Viewing has no default implementation for {0}".Formato(Type));
        }

        private void OnSaving(object entity)
        {
            if (Saving != null)
            {
                Saving(entity);
            }
            else if (typeof(IFile).IsAssignableFrom(cleanType))
            {
                IFile file = (IFile)Server.Convert(entity, cleanType);

                SaveFileDialog sfd = new SaveFileDialog();
                if (CustomizeFileDialog != null)
                    CustomizeFileDialog(sfd); 

                if (sfd.ShowDialog() == true)
                {
                    File.WriteAllBytes(sfd.FileName, file.BinaryFile);
                }                
            }
            else
                throw new ApplicationException("Saving has no default implementation for {0}".Formato(Type)); 
        }

        private object OnOpening()
        {
            if (Opening != null)
                return Opening();

            if (typeof(IFile).IsAssignableFrom(cleanType))
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (CustomizeFileDialog != null)
                    CustomizeFileDialog(ofd); 

                if (ofd.ShowDialog() == true)
                {
                    IFile file = (IFile)Activator.CreateInstance(cleanType);
                    file.FileName = System.IO.Path.GetFileName(ofd.FileName);
                    file.BinaryFile = File.ReadAllBytes(ofd.FileName);

                    return Server.Convert(file, Type);
                }

                return null;
            }

            throw new ApplicationException("Opening has no default implementation for {0}".Formato(Type)); 
        }

        protected bool OnRemoving(object entity)
        {
            if (Removing != null)
                return Removing(entity);

            return true;
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            if (Entity == null || !Remove)
                return;

            if (OnRemoving(Entity))
                Entity = null;
        }

        private void btView_Click(object sender, RoutedEventArgs e)
        {
            if (Entity == null || !View)
                return;

            OnViewing(Entity);
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (Entity == null || !Save)
                return;

            OnSaving(Entity); 
        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Entity != null || !Open)
                return;

            object entity = OnOpening();

            if (entity != null)
                Entity = entity;
        }
    }
}
