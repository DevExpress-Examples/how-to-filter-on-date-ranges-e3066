Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Editors

Namespace FilterOnDateRanges
	Partial Public Class MainPage
		Inherits UserControl
		Public Sub New()
			InitializeComponent()

			Dim list As New List(Of TestData)()
			For month As Integer = 1 To 12
				For day As Integer = 1 To 28
					list.Add(New TestData() With {.CurDate = New DateTime(DateTime.Today.Year, month, day)})
				Next day
			Next month
			grid.DataSource = list
		End Sub
	End Class

	Public Class TestData
		Private privateCurDate As DateTime
		Public Property CurDate() As DateTime
			Get
				Return privateCurDate
			End Get
			Set(ByVal value As DateTime)
				privateCurDate = value
			End Set
		End Property
	End Class

	Public Class DateEditFilter
		Inherits ContentControl
		Public Shared ReadOnly FilterProperty As DependencyProperty
		Shared Sub New()
			FilterProperty = DependencyProperty.Register("Filter", GetType(CriteriaOperator), GetType(DateEditFilter), New PropertyMetadata(Nothing))
		End Sub

		Public Property Filter() As CriteriaOperator
			Get
				Return CType(GetValue(FilterProperty), CriteriaOperator)
			End Get
			Set(ByVal value As CriteriaOperator)
				SetValue(FilterProperty, value)
			End Set
		End Property

		Private dateEditFrom As DateEdit
		Private dateEditTo As DateEdit
		Public Overrides Sub OnApplyTemplate()
			MyBase.OnApplyTemplate()

			If dateEditFrom IsNot Nothing Then
				RemoveHandler dateEditFrom.EditValueChanged, AddressOf dateEdit_EditValueChanged
			End If
			If dateEditTo IsNot Nothing Then
				RemoveHandler dateEditTo.EditValueChanged, AddressOf dateEdit_EditValueChanged
			End If

			dateEditFrom = TryCast(FindName("PART_DateEditFrom"), DateEdit)
			dateEditTo = TryCast(FindName("PART_DateEditTo"), DateEdit)

			Dim op As GroupOperator = TryCast(Filter, GroupOperator)
			If op IsNot Nothing Then
				dateEditFrom.EditValue = (TryCast((TryCast(op.Operands(0), BinaryOperator)).RightOperand, OperandValue)).Value
				dateEditTo.EditValue = (TryCast((TryCast(op.Operands(1), BinaryOperator)).RightOperand, OperandValue)).Value
			End If

			AddHandler dateEditTo.EditValueChanged, AddressOf dateEdit_EditValueChanged
			AddHandler dateEditFrom.EditValueChanged, AddressOf dateEdit_EditValueChanged
		End Sub

		Private Sub dateEdit_EditValueChanged(ByVal sender As Object, ByVal e As EditValueChangedEventArgs)
			Filter = CriteriaOperator.And(New BinaryOperator("CurDate", dateEditFrom.EditValue, BinaryOperatorType.GreaterOrEqual), New BinaryOperator("CurDate", dateEditTo.EditValue, BinaryOperatorType.LessOrEqual))
		End Sub


	End Class
End Namespace
