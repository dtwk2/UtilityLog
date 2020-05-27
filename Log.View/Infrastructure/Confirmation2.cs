//using Forge.Forms.Annotations;
//using System;
//using System.Windows;
//using System.Windows.Shapes;

//namespace Forge.Forms
//{
//    [Form(Mode = DefaultFields.None)]
//    [Title("{Binding Title}", IsVisible = "{Binding Title|IsNotEmpty}")]
//    [Text("{Binding Message}", IsVisible = "{Binding Message|IsNotEmpty}")]
//    [Action("negative", "{Binding NegativeAction}",
//        IsCancel = true,
//        ClosesDialog = true,
//        IsVisible = "{Binding NegativeAction|IsNotEmpty}",
//        Icon = "{Binding NegativeActionIcon}")]
//    [Action("positive", "{Binding PositiveAction}",
//        IsDefault = true,
//        ClosesDialog = true,
//        IsVisible = "{Binding PositiveAction|IsNotEmpty}",
//        Icon = "{Binding PositiveActionIcon}")]
//    public  class Confirmation2 : DialogBase
//    {
//        private readonly Exception exception;

//        public Confirmation2()
//        {
//        }

//        public Confirmation2(string message)
//        {
//            Message = message;
//        }

//        public Confirmation2(string message, string title)
//        {
//            Message = message;
//            Title = title;
//        }

//        public Confirmation2(string message, string title, string positiveAction)
//        {
//            Message = message;
//            Title = title;
//            PositiveAction = positiveAction;
//        }

//        public Confirmation2(string message, string title, string positiveAction, string negativeAction, Exception exception)
//        {
//            Message = message;
//            Title = title;
//            PositiveAction = positiveAction;
//            NegativeAction = negativeAction;
//            this.exception = exception;
//        }

      
//        [DirectContent]
//        public UIElement Exception => new UtilityLog.View.ObjectView("df",this.exception);



//        [DirectContent]
//        public string RawText { get; set; } = "This is a raw string";

//        [DirectContent]
//        public UIElement RawElement => new Ellipse
//        {
//            Width = 100d,
//            Height = 100d,
//            Fill = System.Windows.Media.Brushes.Green
//        };

//        [Break]

//        [DirectContent]
//        public CustomContent CustomControl { get; } = new CustomContent
//        {
//            FirstName = "John",
//            LastName = "Doe"
//        };
//    }

//    public class CustomContent
//    {
//        public string FirstName { get; set; }

//        public string LastName { get; set; }
//    }
//}
