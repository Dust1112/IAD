using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace IADEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialog.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        private readonly CubicEase ease = new CubicEase() { EasingMode = EasingMode.EaseInOut };

        public static bool GotoNewProjectTab { get; set; }

        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserDialogLoaded;
        }

        private void OnProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserDialogLoaded;
            if (!OpenProject.Projects.Any() || GotoNewProjectTab)
            {
                if (!GotoNewProjectTab)
                {
                    openProjectButton.IsEnabled = false;
                    openProjectView.Visibility = Visibility.Hidden;
                }

                OnToggleButton_Click(createProjectButton, new RoutedEventArgs());
            }

            GotoNewProjectTab = false;
        }

        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == openProjectButton)
            {
                if (createProjectButton.IsChecked == true) 
                {
                    createProjectButton.IsChecked = false;
                    AnimateToOpenProject();
                    openProjectView.IsEnabled = true;
                    newProjectView.IsEnabled = false;
                }
                openProjectButton.IsChecked = true;
            }
            else
            {
                if (openProjectButton.IsChecked == true)
                {
                    openProjectButton.IsChecked = false;
                    AnimateToCreateProject();
                    openProjectView.IsEnabled = false;
                    newProjectView.IsEnabled = true;
                }
                createProjectButton.IsChecked = true;
            }
        }

        private void AnimateToCreateProject()
        {
            DoubleAnimation highlightAnimation = new DoubleAnimation(210, 385, new Duration(TimeSpan.FromSeconds(0.2)));
            highlightAnimation.EasingFunction = ease;
            highlightAnimation.Completed += (s, e) =>
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0), new Thickness(-1600, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = ease;
                browserContent.BeginAnimation(MarginProperty, animation);
            };
            hightlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }

        private void AnimateToOpenProject()
        {
            DoubleAnimation highlightAnimation = new DoubleAnimation(385, 210, new Duration(TimeSpan.FromSeconds(0.2)));
            highlightAnimation.EasingFunction = ease;
            highlightAnimation.Completed += (s, e) =>
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(-1600, 0, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = ease;
                browserContent.BeginAnimation(MarginProperty, animation);
            };
            hightlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }
    }
}
