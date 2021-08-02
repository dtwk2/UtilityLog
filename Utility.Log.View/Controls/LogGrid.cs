using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EMA.ExtendedWPFVisualTreeHelper;
using Pcs.Hfrr.Log.View.Infrastructure;
using ReactiveUI;
using TomsToolbox.Wpf;

namespace Utility.Log.View.Controls {
   public class LogGrid : Control {
      private readonly Subject<DataGrid> radGridViewSubject = new Subject<DataGrid>();

      static LogGrid() {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(LogGrid), new FrameworkPropertyMetadata(typeof(LogGrid)));
      }

      public LogGrid() {

         this
             .WhenAnyValue(a => a.ItemsSource)
            .CombineLatest(radGridViewSubject, (enumerable, radGridView) => (enumerable, radGridView))
            .Subscribe(c => c.radGridView.ItemsSource = c.enumerable);

         var expandAllGroups = ReactiveCommand.Create<bool, bool>(a => a);
         ExpandAllGroups = expandAllGroups;
         expandAllGroups.CombineLatest(radGridViewSubject, (b1, radGridView) => (b1, radGridView))
            .Subscribe(c => {
               var (b1, radGridView) = c;

               //if (b1)
               //   radGridView.ExpandAllGroups();
               //else
               //   radGridView.CollapseAllGroups();
            });

         //radGridViewSubject.CombineLatest(this.WhenAnyValue(a => a.ItemsSource), (a, b) => (a, b)).Subscribe(ab => {

         //   var (radGridView, b) = ab;
         //   if (radGridView.Items?.Groups != null && radGridView.Items.Groups.Count > 1)
         //      foreach (var group in radGridView.Items.Groups.Cast<IGroup>().Where(ass => radGridView.IsExpanded(ass) && ass.Key.ToString() != LogLevel.Error.ToString())) {
         //         (radGridView).CollapseGroup(@group);
         //      }

         //   radGridView.WhenAnyValue(ad => ad.Items).Subscribe(cc => {
         //      cc.WhenAnyValue(cv => cv.Groups)
         //         .Subscribe(cee => {
         //            cc.ObservePropertyChanges().Subscribe(e => {

         //               if (e.PropertyName == "ItemProperties" && radGridView.Items.Groups.Count > 1)
         //                  foreach (var group in radGridView.Items.Groups.Cast<IGroup>().Where(ass =>
         //                     radGridView.IsExpanded(ass) && ass.Key.ToString() != LogLevel.Error.ToString())) {
         //                     (radGridView).CollapseGroup(@group);
         //                  }
         //            });
         //         });
         //   });

         //   radGridView.ObserveGroupRowIsExpandedChanges()
         //      .Subscribe(g => {
         //         foreach (var group in radGridView.Items.Groups.Cast<IGroup>()) {
         //            if (group.Key != g.Row.Group.Key)
         //               radGridView.CollapseGroup(group);
         //         }
         //      });

            //radGridView.ObserveGroupCollectionChanges()
            //   .Subscribe(e => {
            //      if (radGridView.Items.Groups.Count <= 1)
            //         return;
            //      foreach (var group in (e.Action) switch {
            //         NotifyGroupCollectionChangedAction.Add or NotifyGroupCollectionChangedAction.Reset =>
            //            radGridView.Items.Groups.Cast<IGroup>().Where(ass => radGridView.IsExpanded(ass) && ass.Key.ToString() != LogLevel.Error.ToString()),
            //         _ => Enumerable.Empty<IGroup>()
            //      })
            //         radGridView.CollapseGroup(group);
            //   });
         //});
      }



      public IEnumerable ItemsSource {
         get => (IEnumerable)GetValue(ItemsSourceProperty);
         set => SetValue(ItemsSourceProperty, value);
      }

      // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ItemsSourceProperty =
          DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LogGrid), new PropertyMetadata(null));

      public ICommand ExpandAllGroups {
         get { return (ICommand)GetValue(ExpandAllGroupsProperty); }
         set { SetValue(ExpandAllGroupsProperty, value); }
      }

      // Using a DependencyProperty as the backing store for ExpandAllGroups.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ExpandAllGroupsProperty =
          DependencyProperty.Register("ExpandAllGroups", typeof(ICommand), typeof(LogGrid), new PropertyMetadata(null));

      public override void OnApplyTemplate() {
         if (this.GetTemplateChild("MainRadGridView") is DataGrid radGridView) {
            this.radGridViewSubject.OnNext(radGridView);
            radGridView.LoadingRow += (_, e) => {
               if (e.Row.Item is LogGroup logViewModel &&
                   logViewModel.Logs.Length <= 1 &&
                   e.Row.FindAllChildren<DataGridCell>().FirstOrDefault()?.FindAllChildren<ToggleButton>()
                       .FirstOrDefault() is { } toggleButton) {
                  toggleButton.IsEnabled = false;
                  toggleButton.Visibility = Visibility.Hidden;
               }
            };
         }
      }
   }
}