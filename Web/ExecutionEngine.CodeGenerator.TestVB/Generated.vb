'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3053
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On



Partial Public Class TestTypes
    
    Protected Shadows ReadOnly Property CurrentFunction() As TestTypesFunction
        Get
            Return CType(MyBase.CurrentFunction,TestTypesFunction)
        End Get
    End Property
    
    Public Property p1() As String
        Get
            Return CType(Me.Variables("p1"),String)
        End Get
        Set
            Me.Variables("p1") = value
        End Set
    End Property
    
    Public Property p4() As System.String()
        Get
            Return CType(Me.Variables("p4"),System.String())
        End Get
        Set
            Me.Variables("p4") = value
        End Set
    End Property
    
    Public Property p5() As System.String(,)
        Get
            Return CType(Me.Variables("p5"),System.String(,))
        End Get
        Set
            Me.Variables("p5") = value
        End Set
    End Property
    
    Public Property p6() As System.String(,)()
        Get
            Return CType(Me.Variables("p6"),System.String(,)())
        End Get
        Set
            Me.Variables("p6") = value
        End Set
    End Property
    
    Public Property p3() As Nullable (Of System.Int32)
        Get
            Return CType(Me.Variables("p3"),Nullable (Of System.Int32))
        End Get
        Set
            Me.Variables("p3") = value
        End Set
    End Property
    
    Public Property p7() As List (Of System.String())
        Get
            Return CType(Me.Variables("p7"),List (Of System.String()))
        End Get
        Set
            Me.Variables("p7") = value
        End Set
    End Property
    
    Public Property p8() As List (Of System.String())()
        Get
            Return CType(Me.Variables("p8"),List (Of System.String())())
        End Get
        Set
            Me.Variables("p8") = value
        End Set
    End Property
    
    Public Property p9() As List (Of nullable (Of System.Int32)())()
        Get
            Return CType(Me.Variables("p9"),List (Of nullable (Of System.Int32)())())
        End Get
        Set
            Me.Variables("p9") = value
        End Set
    End Property
    
    Public Property ps4() As String()
        Get
            Return CType(Me.Variables("ps4"),String())
        End Get
        Set
            Me.Variables("ps4") = value
        End Set
    End Property
    
    Public Property ps5() As String(,)
        Get
            Return CType(Me.Variables("ps5"),String(,))
        End Get
        Set
            Me.Variables("ps5") = value
        End Set
    End Property
    
    Public Property ps6() As String(,)()
        Get
            Return CType(Me.Variables("ps6"),String(,)())
        End Get
        Set
            Me.Variables("ps6") = value
        End Set
    End Property
    
    Protected Sub [Return]()
        Me.ExecuteNextStep
    End Sub
    
    Public Overloads Shared Sub [Call](ByVal currentPage As Remotion.Web.ExecutionEngine.IWxePage, ByVal arguments As Remotion.Web.ExecutionEngine.IWxeCallArguments, ByVal p1 As String, ByVal p4 As System.String(), ByVal p5 As System.String(,), ByVal p6 As System.String(,)(), ByVal p3 As Nullable (Of System.Int32), ByVal p7 As List (Of System.String()), ByVal p8 As List (Of System.String())(), ByVal p9 As List (Of nullable (Of System.Int32)())(), ByVal ps4() As String, ByVal ps5(,) As String, ByVal ps6(,)() As String)
        Dim [function] As TestTypesFunction
        If (currentPage.IsReturningPostBack = false) Then
            [function] = New TestTypesFunction
            [function].p1 = p1
            [function].p4 = p4
            [function].p5 = p5
            [function].p6 = p6
            [function].p3 = p3
            [function].p7 = p7
            [function].p8 = p8
            [function].p9 = p9
            [function].ps4 = ps4
            [function].ps5 = ps5
            [function].ps6 = ps6
            [function].ExceptionHandler.SetCatchExceptionTypes(GetType(System.Exception))
            currentPage.ExecuteFunction([function], arguments)
            Throw New System.Exception("(Unreachable code)")
        Else
            [function] = CType(currentPage.ReturningFunction,TestTypesFunction)
            If (Not ([function].ExceptionHandler.Exception) Is Nothing) Then
                Throw [function].ExceptionHandler.Exception
            End If
        End If
    End Sub
    
    Public Overloads Shared Sub [Call](ByVal currentPage As Remotion.Web.ExecutionEngine.IWxePage, ByVal p1 As String, ByVal p4 As System.String(), ByVal p5 As System.String(,), ByVal p6 As System.String(,)(), ByVal p3 As Nullable (Of System.Int32), ByVal p7 As List (Of System.String()), ByVal p8 As List (Of System.String())(), ByVal p9 As List (Of nullable (Of System.Int32)())(), ByVal ps4() As String, ByVal ps5(,) As String, ByVal ps6(,)() As String)
        TestTypes.Call(currentPage, Remotion.Web.ExecutionEngine.WxeCallArguments.Default, p1, p4, p5, p6, p3, p7, p8, p9, ps4, ps5, ps6)
    End Sub
End Class

<System.SerializableAttribute()>  _
Public Class TestTypesFunction
    Inherits Remotion.Web.ExecutionEngine.WxeFunction
    
    Private Step1 As Remotion.Web.ExecutionEngine.WxePageStep = New Remotion.Web.ExecutionEngine.WxePageStep("TestTypes.aspx")
    
    Public Sub New()
        MyBase.New
    End Sub
    
    Public Sub New(ByVal p1 As String, ByVal p4 As System.String(), ByVal p5 As System.String(,), ByVal p6 As System.String(,)(), ByVal p3 As Nullable (Of System.Int32), ByVal p7 As List (Of System.String()), ByVal p8 As List (Of System.String())(), ByVal p9 As List (Of nullable (Of System.Int32)())(), ByVal ps4() As String, ByVal ps5(,) As String, ByVal ps6(,)() As String)
        MyBase.New(p1, p4, p5, p6, p3, p7, p8, p9, ps4, ps5, ps6)
    End Sub
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(0, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p1() As String
        Set
            Me.Variables("p1") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(1, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p4() As System.String()
        Set
            Me.Variables("p4") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(2, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p5() As System.String(,)
        Set
            Me.Variables("p5") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(3, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p6() As System.String(,)()
        Set
            Me.Variables("p6") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(4, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p3() As Nullable (Of System.Int32)
        Set
            Me.Variables("p3") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(5, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p7() As List (Of System.String())
        Set
            Me.Variables("p7") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(6, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p8() As List (Of System.String())()
        Set
            Me.Variables("p8") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(7, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property p9() As List (Of nullable (Of System.Int32)())()
        Set
            Me.Variables("p9") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(8, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property ps4() As String()
        Set
            Me.Variables("ps4") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(9, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property ps5() As String(,)
        Set
            Me.Variables("ps5") = value
        End Set
    End Property
    
    <Remotion.Web.ExecutionEngine.WxeParameterAttribute(10, Remotion.Web.ExecutionEngine.WxeParameterDirection.[In])>  _
    Public WriteOnly Property ps6() As String(,)()
        Set
            Me.Variables("ps6") = value
        End Set
    End Property
End Class
