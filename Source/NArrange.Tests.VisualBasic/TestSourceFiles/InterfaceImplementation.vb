Imports System
Imports System.ComponentModel
Imports System.Collections
Imports System.Collections.Generic

Namespace SampleNamespace

    Public Class InterfaceImplementation(Of T)
        Inherits Component
        Implements IList(Of T)
        Implements IList
        Implements ICollection
        Implements IBindingList, ITypedList

        Public Sub Add(ByVal item As T) Implements ICollection(Of T).Add
        End Sub

        Public Sub Clear() Implements ICollection(Of T).Clear, IList.Clear
        End Sub

        Public Function Contains(ByVal item As T) As Boolean _
            Implements ICollection(Of T).Contains
        End Function

        Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) _
            Implements ICollection(Of T).CopyTo
        End Sub
        
        Public Sub CopyTo(ByVal array As Array, ByVal arrayIndex As Integer) _
			Implements ICollection.CopyTo
        End Sub

        Public ReadOnly Property Count() As Integer _
            Implements ICollection(Of T).Count, _
                ICollection.Count
            Get
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of T).IsReadOnly, IList.IsReadOnly
            Get
            End Get
        End Property

        Public Overloads Function Remove(ByVal item As T) As Boolean _
        Implements System.Collections.Generic.ICollection(Of T).Remove
        End Function

        Public Function GetEnumerator() As IEnumerator(Of T) _
            Implements IEnumerable(Of T).GetEnumerator
            Return Nothing
        End Function

        Public Function IndexOf(ByVal item As T) As Integer Implements IList(Of T).IndexOf
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As T) Implements IList(Of T).Insert
        End Sub

        Default Public Property Item(ByVal index As Integer) As T Implements IList(Of T).Item
            Get
            End Get
            Set(ByVal value As T)
            End Set
        End Property

        Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of T).RemoveAt, _
            IList.RemoveAt
        End Sub

        Public ReadOnly Property IsSynchronized() As Boolean Implements ICollection.IsSynchronized
            Get
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements ICollection.SyncRoot
            Get
                Return Nothing
            End Get
        End Property

        Public Function AddObject(ByVal value As Object) As Integer Implements IList.Add
        End Function

        Public Function ContainsObject(ByVal value As Object) As Boolean Implements IList.Contains
        End Function

        Public Function IndexOfObject(ByVal value As Object) As Integer Implements IList.IndexOf
        End Function

        Public Sub InsertObject(ByVal index As Integer, ByVal value As Object) Implements IList.Insert
        End Sub

        Public Function GetObjectEnumerator() As IEnumerator _
           Implements IEnumerable.GetEnumerator
            Return Nothing
        End Function

        Public ReadOnly Property IsFixedSize() As Boolean Implements IList.IsFixedSize
            Get
            End Get
        End Property

        Public Property ObjectItem(ByVal index As Integer) As Object Implements IList.Item
            Get
                Return Nothing
            End Get
            Set(ByVal value As Object)
            End Set
        End Property

        Public Overloads Sub Remove(ByVal value As Object) Implements IList.Remove
        End Sub

        Public Sub AddIndex(ByVal [property] As PropertyDescriptor) _
            Implements IBindingList.AddIndex
        End Sub

        Public Function AddNew() As Object Implements IBindingList.AddNew
            Return Nothing
        End Function

        Public ReadOnly _ 
			Property AllowEdit() As Boolean Implements IBindingList.AllowEdit
            Get
            End Get
        End Property

        Public ReadOnly Property AllowNew() As Boolean Implements IBindingList.AllowNew
            Get
            End Get
        End Property

        Public ReadOnly Property AllowRemove() As Boolean Implements IBindingList.AllowRemove
            Get
            End Get
        End Property

        Public Sub ApplySort(ByVal [property] As PropertyDescriptor, _ 
			ByVal direction As ListSortDirection) _
            Implements IBindingList.ApplySort
        End Sub

        Public Function Find(ByVal [property] As PropertyDescriptor, ByVal key As Object) As Integer _
            Implements IBindingList.Find
        End Function

        Public ReadOnly Property IsSorted() As Boolean Implements IBindingList.IsSorted
            Get
            End Get
        End Property

        Public Event ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs) _
            Implements IBindingList.ListChanged

        Public Sub RemoveIndex(ByVal [property] As PropertyDescriptor) Implements _
            IBindingList.RemoveIndex
        End Sub

        Public Sub RemoveSort() _ 
			Implements _
            IBindingList.RemoveSort
        End Sub

        Public ReadOnly Property SortDirection() As ListSortDirection _
            Implements IBindingList.SortDirection
            Get
            End Get
        End Property

        Public ReadOnly Property SortProperty() As PropertyDescriptor _
            Implements IBindingList.SortProperty
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property _ 
			SupportsChangeNotification() As Boolean _
            Implements IBindingList.SupportsChangeNotification
            Get
            End Get
        End Property

        Public ReadOnly Property SupportsSearching() As Boolean Implements IBindingList.SupportsSearching
            Get
            End Get
        End Property

        Public ReadOnly Property SupportsSorting() As Boolean Implements IBindingList.SupportsSorting
            Get
            End Get
        End Property

        Public Function GetItemProperties(ByVal listAccessors() As PropertyDescriptor) _
            As PropertyDescriptorCollection _
            Implements ITypedList.GetItemProperties
            Return Nothing
        End Function

        Public Function GetListName(ByVal listAccessors() As PropertyDescriptor) As _
            String Implements ITypedList.GetListName 'Comment here
            Return Nothing
        End Function

    End Class
End Namespace
