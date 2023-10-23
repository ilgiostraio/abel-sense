using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sense.Lib.FACELibrary;

namespace Sense.Vision.ShoreModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public partial class ViewShore : Window
    {

        public ViewShore()
        {
            InitializeComponent();
        }

        public void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        public void window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /* Check if it is a double click */
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                //do double click actions
           
                if (this.WindowState == WindowState.Maximized)
                        this.WindowState = WindowState.Normal;
                    else
                        this.WindowState = WindowState.Maximized;
            }

    

        }

        public void window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void draw(List<Shore> sList)
        {
            Canvas_Shore.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
               new Action(delegate ()
               {
                   Canvas_Shore.Children.Clear();
               }));


            Parallel.ForEach(sList, (shore) =>
            {
                Dictionary<string, float> expRatio = new Dictionary<string, float>()
                         {
                            { "Angry", shore.Anger_ratio },
                            { "Happy", shore.Happiness_ratio },
                            { "Sad", shore.Sadness_ratio },
                            { "Surprised", shore.Surprise_ratio }
                         };


                float ratioVal = (float)Math.Round((decimal)expRatio.Values.Max(), 1);
                string ratioName = expRatio.OrderByDescending(kvp => kvp.Value).First().Key;

                // Draw subject information: Gender, age +/- deviation, expression, expression rate
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(shore.Gender);
                sb.AppendLine("Age: " + shore.Age + " +/- " + shore.Age_deviation);
                sb.AppendLine(ratioName + ": " + ratioVal + "%");

                double width = Math.Abs(shore.Region_face.Left - shore.Region_face.Right);
                double height = Math.Abs(shore.Region_face.Top - shore.Region_face.Bottom);

                double left = shore.Region_face.Left;
                double top = shore.Region_face.Top;
                double bottom = shore.Region_face.Bottom;
                string gender = shore.Gender;



                //scala per mantenere le proporzioni del punto rispetto alla dimensione della cameraImage
                width *= (float)(CameraImage.ActualWidth / (float)1920);
                height *= (float)(CameraImage.ActualHeight / (float)1080);

                left *= (float)(CameraImage.ActualWidth / (float)1920);
                top *= (float)(CameraImage.ActualHeight / (float)1080);

                left += (float)(Canvas_Shore.ActualWidth - CameraImage.ActualWidth) / 2;
                top += (float)(Canvas_Shore.ActualHeight - CameraImage.ActualHeight) / 2;

                Canvas_Shore.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate ()
                    {
                        Rectangle rect = new Rectangle();
                         //rect.Name = sceneSubjectsCopy[j].id.ToString();
                         rect.Width = width;
                        rect.Height = height;
                        rect.StrokeThickness = 2;
                        rect.Stroke = (gender == "Female") ? Brushes.Fuchsia : Brushes.Cyan;
                        rect.Margin = new Thickness(left, top, 0, 0); //draw the rectangle

                        Label lab = new Label
                        {
                            Foreground = (gender == "Female") ? Brushes.Fuchsia : Brushes.Cyan,
                            FontSize = 22,
                            FontWeight = FontWeights.Bold,
                            Content = sb.ToString(),
                            Opacity = 1,
                            Margin = new Thickness(left, top + height, 0, 0) //draw under the rectangle
                        };


                        Canvas_Shore.Children.Add(rect);
                        Canvas_Shore.Children.Add(lab);


                    }));
            });
        }


    }
}
