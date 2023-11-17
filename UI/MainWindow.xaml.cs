using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AI_neural.Algorithm;

namespace AI_neural.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Style _defaultButtonStyle;
        private readonly Style _selectedButtonStyle;

        private readonly HebbNetwork _hebb = new();
        private bool _hebbSelected;
        
        public MainWindow()
        {
            InitializeComponent();
            _defaultButtonStyle = EraseButton.Style;
            _selectedButtonStyle = DrawButton.Style;

            // load all samples
            foreach (var sample in SampleFile.LoadAll())
            {
                SampleList.Items.Add(new SampleListItem(SampleList, sample.SampleName, sample.Image));
            }
        }

        private void OnActionDraw(object sender, RoutedEventArgs e)
        {
            DrawButton.Style = _selectedButtonStyle;
            EraseButton.Style = _defaultButtonStyle;
            Canvas.StartDrawing();
        }

        private void OnActionErase(object sender, RoutedEventArgs e)
        {
            DrawButton.Style = _defaultButtonStyle;
            EraseButton.Style = _selectedButtonStyle;
            Canvas.StartErasing();
        }

        private void OnActionCreate(object sender, RoutedEventArgs e)
        {
            SampleList.UnselectAll();
            Canvas.Clear();
        }

        private void OnActionSave(object sender, RoutedEventArgs e)
        {
            var name = SampleNameDialog.Prompt();
            if (name == null) return;
            
            // replace existing item
            var replacedExisting = false;
            foreach (var listItem in SampleList.Items)
            {
                var sample = (listItem as SampleListItem)!;
                if (sample.Text.Text != name) continue;
                sample.Image = Canvas.Image;
                replacedExisting = true;
            }

            if (!replacedExisting) // create new item
            {
                var item = new SampleListItem(SampleList, name, Canvas.Image);
                SampleList.Items.Add(item);
            }

            // save sample to file
            var sampleFile = new SampleFile(name, Canvas.Image);
            sampleFile.Save();
        }

        private void OnActionRun(object sender, RoutedEventArgs e)
        {
            NeutralNetwork network;
            if (_hebbSelected)
            {
                network = _hebb;
            }
            else
            {
                var samples = (from object? sample in SampleList.Items select (sample as SampleListItem)!.Image).ToList();
                network = new HammingNetwork(samples);
            }
            var result = network.FindBestMatch(Canvas.Image);
            BestMatch.Text = result == NeutralNetwork.NoMatch ? "No" : (SampleList.Items[result] as SampleListItem)!.Text.Text;
        }

        private void OnActionLearn(object sender, RoutedEventArgs e)
        {
            var samples = new List<BitImage8>();
            foreach (SampleListItem item in SampleList.Items)
            {
                if (item.AddedToHebb) continue;
                item.AddToHebb();
                samples.Add(item.Image);
            }
            _hebb.Learn(samples.ToArray());
        }

        private void OnSampleSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            Canvas.LoadImage((e.AddedItems[0] as SampleListItem)!.Image.Clone());
        }

        private void OnHammingSelected(object sender, RoutedEventArgs e)
        {
            _hebbSelected = false;
            HammingButton.Style = _selectedButtonStyle;
            HebbButton.Style = _defaultButtonStyle;
            LearnButton.IsEnabled = false;
        }

        private void OnHebbSelected(object sender, RoutedEventArgs e)
        {
            _hebbSelected = true;
            HebbButton.Style = _selectedButtonStyle;
            HammingButton.Style = _defaultButtonStyle;
            LearnButton.IsEnabled = true;
        }
    }
}