using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using IADEditor.Content.Unpacking;

namespace IADEditor.Editors.GeometryEditor;

public partial class GeometryView : UserControl
{
    private Point _clickedPosition;
    private bool _capturedLeft;
    private bool _capturedRight;

    public GeometryView()
    {
        InitializeComponent();
        DataContextChanged += (s, e) => SetGeometry();
    }

    private void SetGeometry(int index = -1)
    {
        if (!(DataContext is MeshRenderer vm)) { return; }

        if (vm.Meshes.Any() && Viewport.Children.Count == 2)
        {
            Viewport.Children.RemoveAt(1);
        }

        var meshIndex = 0;
        var modelGroup = new Model3DGroup();
        foreach (var mesh in vm.Meshes)
        {
            // Skip over meshes that we don't want to display
            if (index != -1 && meshIndex != index)
            {
                ++meshIndex;
                continue;
            }

            var mesh3D = new MeshGeometry3D
            {
                Positions = mesh.Positions,
                Normals = mesh.Normals,
                TriangleIndices = mesh.Indices,
                TextureCoordinates = mesh.UVs
            };

            var diffuse = new DiffuseMaterial(mesh.Diffuse);
            var specular = new SpecularMaterial(mesh.Specular, 50);
            var matGroup = new MaterialGroup();
            matGroup.Children.Add(diffuse);
            matGroup.Children.Add(specular);

            var model = new GeometryModel3D(mesh3D, matGroup);
            modelGroup.Children.Add(model);
            
            var binding = new Binding(nameof(mesh.Diffuse)){ Source = mesh};
            BindingOperations.SetBinding(diffuse, DiffuseMaterial.BrushProperty, binding);

            if (meshIndex == index) { break; }
        }

        var visual = new ModelVisual3D() { Content = modelGroup };
        Viewport.Children.Add(visual);
    }

    private void OnGrid_Mouse_LBD(object sender, MouseButtonEventArgs e)
    {
        _clickedPosition = e.GetPosition(this);
        _capturedLeft = true;
        Mouse.Capture(sender as UIElement);
    }

    private void OnGrid_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_capturedLeft && !_capturedRight) { return; }

        var pos = e.GetPosition(this);
        var distance = pos - _clickedPosition;

        if (!_capturedLeft && !_capturedRight)
        {
            //MoveCamera(distance.X, distance.Y);
        }
        else if (!_capturedLeft && _capturedRight)
        {
            var vm = DataContext as MeshRenderer;
            var cameraPos = vm.CameraPosition;
            var yOffset = distance.Y * 0.001 * Math.Sqrt(cameraPos.X * cameraPos.X + cameraPos.Y * cameraPos.Y);
            vm.CameraTarget = new Point3D(vm.CameraTarget.X, vm.CameraTarget.Y + yOffset, vm.CameraTarget.Z);
        }
    }

    private void OnGrid_Mouse_LBU(object sender, MouseButtonEventArgs e)
    {
        _capturedLeft = false;
        if (!_capturedRight) { Mouse.Capture(null); }
    }

    private void OnGrid_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        
    }

    private void OnGrid_Mouse_RBD(object sender, MouseButtonEventArgs e)
    {
        
    }

    private void OnGrid_Mouse_RBU(object sender, MouseButtonEventArgs e)
    {
        
    }
}