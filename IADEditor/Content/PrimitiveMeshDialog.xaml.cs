using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IADEditor.Content.Enums;
using IADEditor.DLLWrapper;
using IADEditor.DLLWrapper.Structs;
using IADEditor.Editors;
using IADEditor.Editors.GeometryEditor;
using IADEditor.GameProject;
using IADEditor.Utilities.Controls;
using Microsoft.Win32;

namespace IADEditor.Content;

public partial class PrimitiveMeshDialog : Window
{
    private static readonly List<ImageBrush> _textures = new List<ImageBrush>();
    
    static PrimitiveMeshDialog()
    {
        LoadTextures();
    }

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
        var smoothingAngle = 0;

        switch (primitiveType)
        {
            case PrimitiveMeshType.Plane:
                info.SegmentX = (int)XSliderPlane.Value;
                info.SegmentZ = (int)ZSliderPlane.Value;
                info.Size.X = Value(WidthScalarBoxPlane, 0.001f);
                info.Size.Z = Value(LengthScalarBoxPlane, 0.001f);
                break;
            case PrimitiveMeshType.Cube:
                return;
            case PrimitiveMeshType.UvSphere:
                info.SegmentX = (int)XSliderUvSphere.Value;
                info.SegmentY = (int)YSliderUvSphere.Value;
                info.Size.X = Value(XScalarBoxUvSphere, 0.001f);
                info.Size.Y = Value(YScalarBoxUvSphere, 0.001f);
                info.Size.Z = Value(ZScalarBoxUvSphere, 0.001f);
                smoothingAngle = (int)AngleSliderUvSphere.Value;
                break;
            case PrimitiveMeshType.IcoSphere:
                return;
            case PrimitiveMeshType.Cylinder:
                return;
            case PrimitiveMeshType.Capsule:
                return;
            default:
                break;
        }

        var geometry = new Geometry
        {
            ImportSettings =
            {
                SmoothingAngle = smoothingAngle
            }
        };
        ContentToolsAPI.CreatePrimitiveMesh(geometry, info);
        (DataContext as GeometryEditor).SetAsset(geometry);
        OnTexture_CheckBox_Click(TextureCheckBox, null);
    }

    private float Value(ScalarBox scalarBox, float min)
    {
        float.TryParse(scalarBox.Value, out var result);
        return Math.Max(result, min);
    }
    
    private static void LoadTextures()
    {
        var uris = new List<Uri>
        {
            new("pack://application:,,,/Resources/PrimitiveMeshView/PlaneTexture.png"),
            new("pack://application:,,,/Resources/PrimitiveMeshView/PlaneTexture.png"),
            new("pack://application:,,,/Resources/PrimitiveMeshView/UVSphereTexture.png"),
        };
        
        _textures.Clear();

        foreach (var uri in uris)
        {
            var resource = Application.GetResourceStream(uri);
            using var reader = new BinaryReader(resource.Stream);
            var data = reader.ReadBytes((int)resource.Stream.Length);
            var imageSource = (BitmapSource)new ImageSourceConverter().ConvertFrom(data);
            imageSource.Freeze();
            var brush = new ImageBrush(imageSource);
            brush.Transform = new ScaleTransform(1, -1, 0.5, 0.5);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Freeze();
            _textures.Add(brush);
        }
    }

    private void OnTexture_CheckBox_Click(object sender, RoutedEventArgs e)
    {
        Brush brush = Brushes.White;
        if ((sender as CheckBox).IsChecked == true)
        {
            brush = _textures[(int)PrimTypeComboBox.SelectedItem];
        }

        var vm = DataContext as GeometryEditor;
        foreach (var mesh in vm.MeshRenderer.Meshes)
        {
            mesh.Diffuse = brush;
        }
    }

    private void OnSave_Button_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            InitialDirectory = Project.Current.ContentPath,
            Filter = "Asset file (*.asset)|*.asset",
        };

        if (dialog.ShowDialog() == true)
        {
            Debug.Assert(!string.IsNullOrEmpty(dialog.FileName));
            var asset = (DataContext as IAssetEditor).Asset;
            Debug.Assert(asset != null);
            asset.Save(dialog.FileName);
        }
    }
}