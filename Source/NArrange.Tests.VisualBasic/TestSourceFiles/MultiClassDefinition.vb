Imports System
Imports System.Collections.Generic
Imports System.Text

Imports System.ComponentModel

Namespace SampleNamespace
	Public Class SampleClass1
	End Class

	Class SampleClass2
		Inherits Exception
	End Class

	Friend Class SampleClass3
		Inherits List(Of Integer)
		Implements IDisposable
		Implements IComparable

		Public Sub Dispose() Implements IDisposable.Dispose
			Throw New Exception("The method or operation is not implemented.")
		End Sub

		Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
			Throw New Exception("The method or operation is not implemented.")
		End Function
	End Class

	Friend NotInheritable Class SampleClass4(Of T1 As {IComparable, IConvertible, New}, T2 As {Class, IComparable(Of T2), Global.System.IConvertible, New})
		Implements IComparable
		Implements IDisposable

		Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Sub Dispose() Implements IDisposable.Dispose
			Throw New Exception("The method or operation is not implemented.")
		End Sub
	End Class

	Friend NotInheritable Class SampleClass5
		Private Sub New()
		End Sub
	End Class

	Public NotInheritable Class SampleClass6
		Private Sub New()
		End Sub
	End Class

	Public NotInheritable Class SampleClass7
		Implements Global.System.IDisposable
		Implements IComparable(Of Integer)
		Public Function CompareTo(ByVal obj As Integer) As Integer Implements IComparable(Of Integer).CompareTo
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Sub Dispose() Implements IDisposable.Dispose
			Throw New Exception("The method or operation is not implemented.")
		End Sub
	End Class

	Public MustInherit Class SampleClass8
	End Class
	
	Friend NotInheritable Class SampleClass9(Of T1, T2)
	End Class
End Namespace

Public Class GlobalClass
End Class
