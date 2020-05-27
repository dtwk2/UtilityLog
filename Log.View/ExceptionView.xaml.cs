using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;


namespace UtilityLog.View
{
    /// <summary>
    /// Interaction logic for ExceptionView.xaml
    ///<a href=http://www.codeproject.com/Tips/469452/WPF-ExceptionView">Adapted from </a>
    /// </summary>
    public partial class ExceptionView : UserControl
    {

        public ExceptionView(Exception exception)
        {
            InitializeComponent();
            this.Content1.Content = new ObjectView("header", exception);
        }
    }
}
