Option Strict On
Option Explicit On

Imports System.ComponentModel

Namespace SampleNamespace
	
	''' <summary>
	''' This is a sample module
	''' </summary>
	<Description("Some module")> _
	Public Module SampleModule
		
		Private _value As Integer
		
		Sub DoSomething()
		End Sub
		
	End Module
	
End Namespace

Public Module GlobalModule
End Module