Imports System

Namespace SampleNamespace

	''' <summary>
	''' Class with operators
	''' </summary>
	Public Class Fraction
	
		Private num As Integer
		Private den As Integer
		
		Public Sub New(ByVal num As Integer, ByVal den As Integer)
			Me.num = num
			Me.den = den
		End Sub

		' overload operator +
		Public Shared Operator +(ByVal a As Fraction, ByVal b As Fraction) As Fraction
			Return New Fraction(a.num * b.den + b.num * a.den, a.den * b.den)
		End Operator

		' overload operator *
		Public Shared Operator *(ByVal a As Fraction, ByVal b As Fraction) As Fraction
			Return New Fraction(a.num * b.num, a.den * b.den)
		End Operator
		
		' overload operator /
		Public Shared Operator /(ByVal a As Fraction, ByVal b As Fraction) As Fraction
			Return New Fraction(a.num * b.den, a.den * b.num)
		End Operator

		' overload operator =
		Public Shared Operator =(ByVal a As Fraction, ByVal b As Fraction) As Boolean
			Return a.num = b.num AndAlso a.den = b.den
		End Operator

		' overload operator <>
		Public Shared Operator <>(ByVal a As Fraction, ByVal b As Fraction) As Boolean
			Return Not (a = b)
		End Operator
		
		' overload operator <
		Public Shared Operator <=(ByVal a As Fraction, ByVal b As Fraction) As Boolean
			Throw New NotImplementedException
		End Operator
		
		' overload operator >
		Public Shared Operator >=(ByVal a As Fraction, ByVal b As Fraction) As Boolean
			Throw New NotImplementedException
		End Operator


		' define operator double
		Public Shared Widening Operator CType(ByVal f As Fraction) As Double
			Return CDbl(f.num) / f.den
		End Operator

		' define operator decimal
		Public Shared Narrowing Operator CType(ByVal f As Fraction) As Decimal
			Return CDec(f.num) / f.den
		End Operator
		
	End Class
	
End Namespace
