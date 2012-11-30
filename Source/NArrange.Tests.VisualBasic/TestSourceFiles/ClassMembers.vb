Imports System.Text
Imports System
Imports System.Runtime
Imports System.IO
Imports System.Collections
Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.ComponentModel

Namespace SampleNamespace

	''' <summary>
	''' This is a class definition.
	''' </summary>
	Public Class SampleClass
		
		#Region "Fields"

		'This field has a regular comment
		Dim _simpleField As Boolean
		Private _fieldWithInitialVal As Integer = 1
		Private Dim _dimField As Boolean

		''' <summary>
		''' This is a static readonly string
		''' </summary>
		Protected Shared ReadOnly StaticStr As String = "static; string;"

		Private _genericField As Nullable(Of Integer)
		Protected Friend _arrayField As String() = {}
		Friend internal As Boolean

		''' <summary>
		''' This field has an attribute
		''' </summary>
		<ThreadStatic()> _
		Private Shared _attributedField As String = Nothing

		Public Const ConstantStr As String = "constant string"

		Private _val1, _val2 As Integer
		Private Shared _val3 , _val4 ,_val5, _val6 As Integer
		Private _val7 As Integer : Private _val8 As Integer

		#End Region

		#Region "Constructors"

		''' <summary>
		''' Instance constructor
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Internal constructor with params
		''' </summary>
		''' <param name="arrayParam"></param>
		Friend Sub New(ByVal arrayParam As String())
		End Sub

		' Static constructor
		REM Another comment
		'' Not an XML comment
		''' XML comment
		Shared Sub New()
		End Sub		' New

		#End Region

		#Region "Properties"

		''' <summary>
		''' Simple property
		''' </summary>
		Public Property SimpleProperty() As Boolean
			Get
				Return _simpleField
			End Get
			Set
				_simpleField = value
			End Set
		End Property

		'
		' This is a protected property.  Also virtual.
		'
		Protected Overridable ReadOnly Property ProtectedProperty() As Integer
			Get
				Return _fieldWithInitialVal
			End Get
		End Property

		''' <summary>
		''' This property is static
		''' </summary>
		' Mixed comment style here
		Public Shared ReadOnly Property StaticProperty() As String
			Get
				Return StaticStr + " is returned."
			End Get
		End Property

		' This property has multiple attributes and different
		' ordering for the static specification.
		<Obsolete("This property has attributes.")> _
		<Description("Multiple attribute property.")> _
		Public Shared ReadOnly Property AttributedProperty() As String
			Get
				Return _attributedField
			End Get
		End Property

		''' <summary>
		''' Generic property.  This comment has extra whitespace
		''' before the member, but should still be considered header comments.
		''' This member also has an attribute before the comment
		''' that should still be matched to the property.
		''' </summary>

		<Obsolete(), Description("Multiple attribute property.")> _
		Friend Property GenericProperty() As Nullable(Of Integer)
			Get
				Return _genericField
			End Get
			Set
				_genericField = value
				If _genericField.HasValue Then
					internal = True
				End If
			End Set
		End Property

		''' <summary>
		''' This property returns an array
		''' </summary>
		Public Shadows ReadOnly Property ArrayProperty() As String()
			Get
				Return _arrayField
			End Get
		End Property

		''' <summary>
		''' Indexer property
		''' </summary>
		''' <param name="index"></param>
		''' <returns></returns>
		Public Default Property Item(ByVal index As Integer) As String
			Get
				Return _arrayField(index)
			End Get
			Set
				_arrayField(index) = value
			End Set
		End Property
		
		Public Default Property Item(ByVal string1 As String, ByVal string2 As String) As Integer
			Get
				Return 0
			End Get
			Set
			End Set
		End Property

		#End Region

		#Region "Methods"
		
		''' <summary>
		''' Finalizer
		''' </summary>
		''' <remarks></remarks>
		Protected Overrides Sub Finalize()
			Try

				
			Finally
				MyBase.Finalize()
			End Try
		End Sub

		' Simple method
		Public Sub DoSomething()
			' 
			' Make sure we detect that we're in a string while 
			' parsing the following line.
			'
			Console.WriteLine("}")
			RaiseEvent ExplicitEvent(Me, false)
		End Sub

		' 
		' Comment here 
		'         
		Public Overloads Overrides Function ToString() As String
			' This comment has a block end character in it. } 

			Return "SampleClass"
		End Function

		''' <summary>
		''' Simple method with params and a return value
		''' </summary>
		''' <param name="intParam"></param>
		''' <param name="stringParam"></param>
		''' <returns></returns>
		Private Function GetBoolValue(ByVal intParam As Integer, ByVal stringParam As String) As Boolean
			Return True
		End Function

		''' <summary>
		''' This method has parameter attributes
		''' </summary>
		''' <param name="intParam">Int parameter</param>
		''' <returns></returns>
		<Description("Method with parameter attributes")> _
		Friend Shared Function GetWithParamAttributes( _
			<Description("Int parameter")> _
			ByVal intParam As Integer, _ 
			<Description("String parameter")> _
			ByVal stringParam As String) As Nullable(Of Integer)
			If intParam = 0 Then
				Return Nothing
			Else
				Return intParam
			End If
		End Function

		''' <summary>
		''' This method has parameter attributes
		''' </summary>
		Public Function GetWithTypeParameters(Of T1 As {IDisposable, New}, T2 As {IDisposable, New})(ByVal typeParam1 As Action(Of T1), ByVal typeParam2 As Action(Of T2)) As Boolean
			Try
				typeParam1(New T1())
				typeParam2(New T2())
				Return True
			Catch generatedExceptionName As InvalidOperationException
				Return False
			End Try
		End Function

		<DllImport("User32.dll")> _
		Public Shared Function MessageBox(ByVal h As Integer, ByVal m As String, ByVal c As String, ByVal type As Integer) As Integer
		End Function

		#End Region

		#Region "Delegates"

		''' <summary>
		''' Sample delegate definition
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="boolParam"></param>
		Public Delegate Sub SampleEventHandler(ByVal sender As Object, ByVal boolParam As Boolean)
		
		''' <summary>
		''' Generic delegate
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <param name="t1"></param>
		''' <param name="t2"></param>
		''' <returns></returns>
		Private Delegate Function Compare(Of T As Class)(ByVal t1 As T, ByVal t2 As T) As Integer

		#End Region

		#Region "Events"

		''' <summary>
		''' Simple event
		''' </summary>
		Public Event SimpleEvent As SampleEventHandler
		
		''' <summary>
		''' Generic event
		''' </summary>
		Public Event GenericEvent As EventHandler(Of EventArgs)
		
		''' <summary>
		''' Another event
		''' </summary>
		Public Event AnotherEvent(ByVal args As EventArgs)
		
		''' <summary>
		''' Explicit event
		''' </summary>
		Public Custom Event ExplicitEvent As SampleEventHandler
			AddHandler(ByVal value As SampleEventHandler)
			End AddHandler
			RemoveHandler(ByVal value As SampleEventHandler)
			End RemoveHandler
      		RaiseEvent(ByVal sender As Object, ByVal boolParam As Boolean)
      		End RaiseEvent
		End Event

		#End Region

		#Region "Nested Types"

		''' <summary>
		''' Sample enumeration
		''' </summary>
		<Flags()> _
		Private Enum SampleEnum
			None = 0
			Some = 1
			More = 2
		End Enum

		''' <summary>
		''' Nested structure
		''' </summary>
		Public Structure SampleStructure
			Public ReadOnly Name As String

			''' <summary>
			''' Creates a new SampleStructure
			''' </summary>
			Public Sub New(ByVal name As String)
				Name = name
			End Sub
		End Structure

		''' <summary>
		''' Nested class
		''' </summary>
		Private Class SampleNestedClass(Of T As New)
			''' <summary>
			''' Creates a new SampleNestedClass
			''' </summary>
			Public Sub New()
			End Sub
		End Class

		''' <summary>
		''' Sample nested, static class (Note: VB static classes are sealed with a private constructor)
		''' </summary>
		Friend NotInheritable Class SampleNestedStaticClass
			Private Sub New()
			End Sub
			Public Shared Function DoSomething(ByVal stringParam As String) As Boolean
				Return True
			End Function
		End Class

		#End Region
		
	End Class
End Namespace