using System;
using System.Windows;
using System.Windows.Controls;
using IADEditor.Content.Enums;
using IADEditor.DLLWrapper;
using IADEditor.DLLWrapper.Structs;
using IADEditor.Editors.GeometryEditor;
using IADEditor.Utilities.Controls;

namespace IADEditor.Content;

public partial class PrimitiveMeshDialog : Window
{
    public PrimitiveMeshDialog()
    {
        InitializeComponent();
        Loaded += (s, e) => UpdatePrimitive();
    }

    private void OnPrimitiveType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdatePrimitive();

    private void OnSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        => UpdatePrimitive();

    private void OnScalarBox_ValueChanged(object sender, RoutedEventArgs e)
        => UpdatePrimitive();
    
    private void UpdatePrimitive()
    {
        if (!IsInitialized) { return; }

        var primitiveType = (PrimitiveMeshType)PrimTypeComboBox.SelectedIndex;
        var info = new PrimitiveInitInfo { Type = primitiveType };

        switch (primitiveType)
        {
            case PrimitiveMeshType.Plane:
                    info.SegmentX = (int)XSliderPlane.Value;
                    info.SegmentX = (int)ZSliderPlane.Value;
                    info.Size.X = Value(WidthScalarBoxPlane, 0.001f);
                    info.Size.Z = Value(LengthScalarBoxPlane, 0.001f);
                    break;
            case PrimitiveMeshType.Cube:
                break;
            case PrimitiveMeshType.UvSphere:
                break;
            case PrimitiveMeshType.IcoSphere:
                break;
            case PrimitiveMeshType.Cylinder:
                break;
            case PrimitiveMeshType.Capsule:
                break;
            default:
                break;
        }

        var geometry = new Geometry();
        ContentToolsAPI.CreatePrimitiveMesh(geometry, info);
        (DataContext as GeometryEditor).SetAsset(geometry);
    }

    private float Value(ScalarBox scalarBox, float min)
    {
        float.TryParse(scalarBox.Value, out var result);
        return Math.Max(result, min);
    }
}