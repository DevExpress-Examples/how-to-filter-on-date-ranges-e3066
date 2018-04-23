using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;

namespace FilterOnDateRanges {
    public partial class MainPage : UserControl {
        public MainPage() {
            InitializeComponent();

            List<TestData> list = new List<TestData>();
            for (int month = 1; month <= 12; month++)
                for (int day = 1; day <= 28; day++)
                    list.Add(new TestData() {
                        CurDate = new DateTime(DateTime.Today.Year, month, day)
                    });
            grid.ItemsSource = list;
        }
    }

    public class TestData {
        public DateTime CurDate {
            get;
            set;
        }
    }

    public class DateEditFilter : ContentControl {
        public static readonly DependencyProperty FilterProperty;
        static DateEditFilter() {
            FilterProperty = DependencyProperty.Register("Filter", typeof(CriteriaOperator),
                typeof(DateEditFilter), new PropertyMetadata(null));
        }

        public CriteriaOperator Filter {
            get {
                return (CriteriaOperator)GetValue(FilterProperty);
            }
            set {
                SetValue(FilterProperty, value);
            }
        }

        DateEdit dateEditFrom;
        DateEdit dateEditTo;
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            if (dateEditFrom != null)
                dateEditFrom.EditValueChanged -=
                    new EditValueChangedEventHandler(dateEdit_EditValueChanged);
            if (dateEditTo != null)
                dateEditTo.EditValueChanged -=
                    new EditValueChangedEventHandler(dateEdit_EditValueChanged);

            dateEditFrom = FindName("PART_DateEditFrom") as DateEdit;
            dateEditTo = FindName("PART_DateEditTo") as DateEdit;

            GroupOperator op = Filter as GroupOperator;
            if (op != null) {
                dateEditFrom.EditValue =
                    ((op.Operands[0] as BinaryOperator).RightOperand as OperandValue).Value;
                dateEditTo.EditValue =
                    ((op.Operands[1] as BinaryOperator).RightOperand as OperandValue).Value;
            }

            dateEditTo.EditValueChanged +=
                new EditValueChangedEventHandler(dateEdit_EditValueChanged);
            dateEditFrom.EditValueChanged +=
                new EditValueChangedEventHandler(dateEdit_EditValueChanged);
        }

        void dateEdit_EditValueChanged(object sender, EditValueChangedEventArgs e) {
            Filter = CriteriaOperator.And(new BinaryOperator("CurDate", dateEditFrom.EditValue, BinaryOperatorType.GreaterOrEqual),
                                          new BinaryOperator("CurDate", dateEditTo.EditValue, BinaryOperatorType.LessOrEqual));
        }


    }
}
