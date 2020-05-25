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

        /// <summary>
        /// Gets the value of the AssemblyProduct attribute of the app.  
        /// If unable to lookup the attribute, returns an empty string.
        /// </summary>
        public static string Product => _product ?? (_product = AssemblyHelper.GetProductName());

        static string _defaultTitle;
        static string _product;

        // Font sizes based on the "normal" size.
        //double _small;
        //double _med;
        //double _large;

        // This is used to dynamically calculate the mainGrid.MaxWidth when the Window is resized,
        // since I can't quite get the behavior I want without it.  See CalcMaxTreeWidth().
        double _chromeWidth;
        private bool _initialized;


        public static readonly DependencyProperty ControlsBorderBrushProperty = DependencyProperty.Register(nameof(ControlsBorderBrush), typeof(Brush), typeof(ExceptionView), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty ExceptionSourceProperty = DependencyProperty.Register(nameof(ExceptionSource), typeof(object), typeof(ExceptionView), new PropertyMetadata(default(Exception), OnExceptionSourceChanged));

        public static readonly RoutedEvent ExceptionSourceChangedEvent = EventManager.RegisterRoutedEvent(nameof(ExceptionSourceChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Exception>), typeof(ExceptionView));

        public static readonly DependencyProperty ShowDetailsProperty = DependencyProperty.Register(nameof(ShowDetails), typeof(bool), typeof(ExceptionView), new PropertyMetadata(false, OnShowDetailPropertyChanged));



        public string InnerProperty
        {
            get { return (string)GetValue(InnerPropertyProperty); }
            set { SetValue(InnerPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerPropertyProperty =
            DependencyProperty.Register("InnerProperty", typeof(string), typeof(ExceptionView), new PropertyMetadata(null));



        public ExceptionView() : this(null, null)
        {
        }

        /// <summary>
        /// The exception and header message cannot be null.  If owner is specified, this window
        /// uses its Style and will appear centered on the Owner.  You can override this before
        /// calling ShowDialog().
        /// </summary>
        public ExceptionView(string headerMessage, Exception e)
        {
            InitializeComponent();
            Initialize(headerMessage, e);

            void Initialize(string headerMessage, Exception e)
            {
                if (!_initialized)
                {
                    Loaded += (sender, args) =>
                    {
                        treeCol.Width = new GridLength(treeCol.ActualWidth, GridUnitType.Pixel);
                        _chromeWidth = ActualWidth - mainGrid.ActualWidth;
                        ToggleDetails();
                        CalcMaxTreeWidth();
                    };

                    if (DefaultPaneBrush != null)
                    {
                        treeView1.Background = DefaultPaneBrush;
                    }

                    docViewer.Background = treeView1.Background;

                    // We use three font sizes.  The smallest is based on whatever the "standard"
                    // size is for the current system/app, taken from an arbitrary control.

                    //_small = treeView1.FontSize;
                    //_med = _small * 1.1;
                    //_large = _small * 1.2;
                }

                _initialized = true;
                docViewer.Document = null;
                BuildTree(treeView1, e, headerMessage, InnerProperty);
            }

        }


        public static Brush DefaultPaneBrush { get; set; }

        /// <summary>
        /// The default title to use for the ExceptionView window.  Automatically initialized 
        /// to "Error - [ProductName]" where [ProductName] is taken from the application's
        /// AssemblyProduct attribute (set in the AssemblyInfo.cs file).  You can change this
        /// default, or ignore it and set Title yourself before calling ShowDialog().
        /// </summary>
        public static string DefaultTitle
        {
            get => _defaultTitle ??= "Error" + (string.IsNullOrEmpty(Product) ? string.Empty : $" - {Product}");
            set => _defaultTitle = value;
        }

        public object ExceptionSource
        {
            get => (object)GetValue(ExceptionSourceProperty);
            set => SetValue(ExceptionSourceProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<Exception> ExceptionSourceChanged
        {
            add => AddHandler(ExceptionSourceChangedEvent, value);
            remove => RemoveHandler(ExceptionSourceChangedEvent, value);
        }

        public Brush ControlsBorderBrush
        {
            get => (Brush)GetValue(ControlsBorderBrushProperty);
            set => SetValue(ControlsBorderBrushProperty, value);
        }

        public bool ShowDetails
        {
            get => (bool)GetValue(ShowDetailsProperty);
            set => SetValue(ShowDetailsProperty, value);
        }



        private static void OnExceptionSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ExceptionView)d;
            var args = new RoutedPropertyChangedEventArgs<Exception>(
                (Exception)e.OldValue,
                (Exception)e.NewValue)
            {
                RoutedEvent = ExceptionView.ExceptionSourceChangedEvent
            };
            instance.RaiseEvent(args);
            instance.docViewer.Document = null;
            BuildTree(instance.treeView1, args.NewValue, String.Empty, instance.InnerProperty);
        }


        private static void OnShowDetailPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExceptionView ExceptionView)
            {
                ExceptionView.ToggleDetails();
            }
        }


        private void ToggleDetails()
        {
            if (ShowDetails)
            {
                treeView1.Visibility = Visibility.Visible;
                gridSplitter.Visibility = Visibility.Visible;
                Grid.SetColumn(docViewer, 2);
                Grid.SetColumnSpan(docViewer, 1);
            }
            else
            {
                treeView1.Visibility = Visibility.Collapsed;
                gridSplitter.Visibility = Visibility.Collapsed;
                Grid.SetColumn(docViewer, 0);
                Grid.SetColumnSpan(docViewer, 3);
            }
        }


        static dynamic GetFontSizes(TreeView treeView)
        {
            dynamic runTimeObject = new ExpandoObject();
            runTimeObject.Small = treeView.FontSize;
            runTimeObject.Medium = treeView.FontSize * 1.1;
            runTimeObject.Large = treeView.FontSize * 1.2;
            return runTimeObject;
        }



        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowCurrentItem();
        }

        private void chkWrap_Checked(object sender, RoutedEventArgs e)
        {
            ShowCurrentItem();
        }

        private void chkWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowCurrentItem();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Build a FlowDocument with Inlines from all top-level tree items.
            if (!(sender is TreeView treeView))
                return;

            Do(treeView);
            // The Inlines that were being displayed are now in the temporary document we just built,
            // causing them to disappear from the viewer.  This puts them back.

            ShowCurrentItem();

        }


        void ShowCurrentItem()
        {
            if (treeView1.SelectedItem != null)
            {

                var doc = GetDoc();
                var inlines = (treeView1.SelectedItem as TreeViewItem)?.Tag as IEnumerable<Inline>;

                if (chkWrap.IsChecked == false)
                {
                    doc.PageWidth = CalcNoWrapWidth(inlines) + 50;
                }

                var para = new Paragraph();
                para.Inlines.AddRange(inlines);
                doc.Blocks.Add(para);

                docViewer.Document = doc;
            }

            FlowDocument GetDoc() => new FlowDocument
            {
                FontSize = GetFontSizes(treeView1).Small,
                FontFamily = treeView1.FontFamily,
                TextAlignment = TextAlignment.Left,
                Background = docViewer.Background
            };

            // Determines the page width for the Inlilness that causes no wrapping.
            static double CalcNoWrapWidth(IEnumerable<Inline> inlines)
            {
                double pageWidth = 0;
                var tb = new TextBlock();
                var size = new Size(double.PositiveInfinity, double.PositiveInfinity);

                foreach (var inline in inlines)
                {
                    tb.Inlines.Clear();
                    tb.Inlines.Add(inline);
                    tb.Measure(size);

                    if (tb.DesiredSize.Width > pageWidth) pageWidth = tb.DesiredSize.Width;
                }

                return pageWidth;
            }
        }

        // Builds the tree in the left pane.
        // Each TreeViewItem.Tag will contain a list of Inlines
        // to display in the right-hand pane When it is selected.
        private static void BuildTree(TreeView treeView, object e, string summaryMessage, string innerProperty)
        {
            treeView.Items.Clear();

            // The first node in the tree contains the summary message and all the
            // nested exception messages.

            var inlines = new List<Inline>();
            var firstItem = new TreeViewItem { Header = "Summary" };
            treeView.Items.Add(firstItem);

            if (!string.IsNullOrEmpty(summaryMessage))
            {
                var inline = new Bold { FontSize = GetFontSizes(treeView).Large };

                ReplaceWithLinks(inline, summaryMessage);

                inlines.Add(inline);
                inlines.Add(new LineBreak());
            }

            // Now add top-level nodes for each exception while building
            // the contents of the first node.
            while (e != null)
            {
                //AddLines(inlines, e.Message);

                inlines.Add(new LineBreak());

                AddException(treeView, e);
                e = e.GetType().GetProperty(innerProperty).GetValue(e);//.InnerException;
            }

            firstItem.Tag = inlines;
            firstItem.IsSelected = true;

            static void ReplaceWithLinks(Span inline, string message)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    return;
                }

                var filePathRegex = new Regex(@"((?:[a-zA-Z]\:){0,1}(?:[\\/][\w.]+){1,})", RegexOptions.Compiled);
                var filePathsMatches = filePathRegex.Matches(message);

                if (filePathsMatches.Count > 0)
                {
                    var parts = filePathRegex.Split(message);
                    foreach (var part in parts)
                    {
                        if (filePathRegex.IsMatch(part) && Uri.TryCreate(part, UriKind.Absolute, out var uri))
                        {
                            var hyperLink = new Hyperlink
                            {
                                NavigateUri = uri,
                                ToolTip = $"Open: {part}",
                                Inlines =
                            {
                                part
                            }
                            };
                            hyperLink.RequestNavigate += HyperLinkOnRequestNavigate;
                            inline.Inlines.Add(hyperLink);
                        }
                        else
                        {
                            inline.Inlines.Add(new Run(part));
                        }
                    }
                }
                else
                {
                    inline.Inlines.Add(message);
                }

                static void HyperLinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
                {
                    if (e.Uri != null)
                    {
                        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    }
                    e.Handled = true;
                }
            }

        }




        static void Do(TreeView treeView)
        {
            var inlines = new List<Inline>();
            var doc = new FlowDocument();
            var para = new Paragraph();

            doc.FontSize = GetFontSizes(treeView).Small;
            doc.FontFamily = treeView.FontFamily;
            doc.TextAlignment = TextAlignment.Left;

            foreach (TreeViewItem treeItem in treeView.Items)
            {
                if (inlines.Any())
                {
                    // Put a line of underscores between each exception.

                    inlines.Add(new LineBreak());
                    inlines.Add(new Run("____________________________________________________"));
                    inlines.Add(new LineBreak());
                }

                if (treeItem.Tag != null && treeItem.Tag is IEnumerable<Inline> tagInlines)
                {
                    inlines.AddRange(tagInlines);
                }
            }

            para.Inlines.AddRange(inlines);
            doc.Blocks.Add(para);

            // Now place the doc contents on the clipboard in both
            // rich text and plain text format.

            var range = new TextRange(doc.ContentStart, doc.ContentEnd);
            var data = new DataObject();

            using (Stream stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Rtf);
                data.SetData(DataFormats.Rtf, Encoding.UTF8.GetString((stream as MemoryStream).ToArray()));
            }

            data.SetData(DataFormats.StringFormat, range.Text);
            Clipboard.SetDataObject(data);
        }


        // Adds the string to the list of Inlines, substituting
        // LineBreaks for an newline chars found.
        static void AddLines(ICollection<Inline> inlines, string str)
        {
            var lines = str.Split('\n');

            inlines.Add(new Run(lines[0].Trim('\r')));

            foreach (var line in lines.Skip(1))
            {
                inlines.Add(new LineBreak());
                inlines.Add(new Run(line.Trim('\r')));
            }
        }

        // Adds the exception as a new top-level node to the tree with child nodes
        // for all the exception's properties.
        static void AddException(TreeView treeView, object e)
        {
            // Create a list of Inlines containing all the properties of the exception object.
            // The three most important properties (message, type, and stack trace) go first.

            var exceptionItem = new TreeViewItem();
            var inlines = new List<Inline>();
            var properties = e.GetType().GetProperties();

            exceptionItem.Header = e.GetType();
            exceptionItem.Tag = inlines;
            treeView.Items.Add(exceptionItem);

            var fontSizes = GetFontSizes(treeView);
            Inline inline = new Bold(new Run(e.GetType().ToString()))
            {
                FontSize = fontSizes.Large
            };
            inlines.Add(inline);
            inlines.Add(new LineBreak());
            var medium = GetFontSizes(treeView).Medium;
            //AddProperty(inlines, "Message", e.Message, medium);
            //AddProperty(inlines, "Stack Trace", e.StackTrace, medium);

            foreach (var info in properties)
            {
                // Skip InnerException because it will get a whole
                // top-level node of its own.

                if (info.Name == "InnerException")
                {
                    continue;
                }

                var value = info.GetValue(e, null);

                if (value == null)
                {
                    continue;
                }

                if (value is string s)
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                }
                else if (value is IDictionary data)
                {
                    value = RenderDictionary(data);
                    if (string.IsNullOrEmpty((string)value))
                    {
                        continue;
                    }
                }
                else if (value is IEnumerable enumerable && !(enumerable is string))
                {
                    value = RenderEnumerable(enumerable);
                    if (string.IsNullOrEmpty((string)value))
                    {
                        continue;
                    }
                }

                //if (info.Name != "Message" &&
                //    info.Name != "StackTrace")
                //{
                //    // Add the property to list for the exceptionItem.
                //    AddProperty(inlines, info.Name, value, medium);
                //}

                // Create a TreeViewItem for the individual property.
                var propertyItem = new TreeViewItem();
                var propertyInlines = new List<Inline>();

                propertyItem.Header = info.Name;
                propertyItem.Tag = propertyInlines;
                exceptionItem.Items.Add(propertyItem);
                AddProperty(propertyInlines, info.Name, value, medium);

            }

            static void AddProperty(List<Inline> inlines, string propName, object propVal, double fontSize)
            {
                inlines.Add(new LineBreak());
                var inline = new Bold(new Run(propName + ":")) { FontSize = fontSize };
                inlines.Add(inline);
                inlines.Add(new LineBreak());

                if (propVal is string str)
                {
                    // Might have embedded newlines.

                    AddLines(inlines, str);
                }
                else
                {
                    inlines.Add(new Run(propVal.ToString()));
                }
                inlines.Add(new LineBreak());
            }

            static string RenderEnumerable(IEnumerable data)
            {
                var result = new StringBuilder();

                foreach (var obj in data)
                {
                    result.AppendFormat("{0}\n", obj);
                }

                if (result.Length > 0) result.Length = result.Length - 1;
                return result.ToString();
            }

            static string RenderDictionary(IDictionary data)
            {
                var result = new StringBuilder();

                foreach (var key in data.Keys)
                {
                    if (key != null && data[key] != null)
                    {
                        result.AppendLine(key + " = " + data[key]);
                    }
                }

                if (result.Length > 0) result.Length = result.Length - 1;
                return result.ToString();
            }

        }




        private void CalcMaxTreeWidth()
        {
            // This prevents the GridSplitter from being dragged beyond the right edge of the window.
            // Another way would be to use star sizing for all Grid columns including the left 
            // Grid column (i.e. treeCol), but that causes the width of that column to change when the
            // window's width changes, which I don't like.

            // mainGrid.MaxWidth = ActualWidth - _chromeWidth;
            treeCol.MaxWidth = mainGrid.MaxWidth - textCol.MinWidth;
        }




        static class AssemblyHelper
        {
            // Initializes the Product property.
            public static string GetProductName()
            {
                var result = "";

                try
                {
                    var customAttributes = GetAppAssembly()?.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                    if (customAttributes != null && customAttributes.Length > 0)
                    {
                        result = ((AssemblyProductAttribute)customAttributes[0]).Product;
                    }
                }
                catch
                {
                    // ignored
                }

                return result;


                // Tries to get the assembly to extract the product name from.
                static Assembly GetAppAssembly()
                {
                    Assembly appAssembly = null;

                    try
                    {
                        // This is supposedly how Windows.Forms.Application does it.
                        appAssembly = Application.Current.MainWindow.GetType().Assembly;
                    }
                    catch
                    {
                        // ignored
                    }

                    // If the above didn't work, try less desireable ways to get an assembly.

                    if (appAssembly == null)
                    {
                        appAssembly = Assembly.GetEntryAssembly();
                    }

                    return appAssembly ?? (appAssembly = Assembly.GetExecutingAssembly());
                }
            }

        }
    }
}
