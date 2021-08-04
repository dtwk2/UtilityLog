using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Splat;

namespace Utility.Log.View.Infrastructure
{
    public class ConditionalStyleSelector : StyleSelector
    {
        private ObservableCollection<ConditionalStyleRule> rules;

        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            return item is Utility.Log.Model.Log { Level: LogLevel level } &&
                   this.Rules.FirstOrDefault(a => Equals(a.Value, level)) is { } rule ?
                rule.Style :
                base.SelectStyle(item, container);
        }

        public ObservableCollection<ConditionalStyleRule> Rules => this.rules ??= new ObservableCollection<ConditionalStyleRule>();
    }

    public class ConditionalStyleRule
    {
        public object Value { get; set; }

        public Style Style { get; set; }
    }
}